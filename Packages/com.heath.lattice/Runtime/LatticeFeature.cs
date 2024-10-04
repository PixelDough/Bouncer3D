using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

using static Lattice.LatticeShaderProperties;

namespace Lattice
{
	public static class LatticeFeature
	{
		/// <summary>
		/// Hardcoded max number of lattice handles supported. Can be changed.
		/// </summary>
		internal const int MaxHandles = 1024;

		/// <summary>
		/// The compute shader file name, relative to a Resources folder.
		/// </summary>
		internal const string ComputeShaderName = "LatticeCompute";

		private static bool _initialised = false;

		private static ComputeShader _compute;
		private static uint _resetGroupSize;
		private static uint _deformGroupSize;

		private static ComputeBuffer _latticeBuffer;
		private static readonly int[] _latticeResolution = new int[3];

		private static readonly CommandBuffer _cmd = new();

		private static readonly List<LatticeModifierBase> _modifiers = new();
		private static readonly List<SkinnedLatticeModifier> _skinnedModifiers = new();

		#region Public Methods

		/// <summary>
		/// Enqueues a mesh to be deformed this frame.
		/// </summary>
		internal static void Enqueue(LatticeModifierBase modifier)
		{
			_modifiers.Add(modifier);
		}

		/// <summary>
		/// Enqueues a skinned mesh to be deformed this frame.
		/// </summary>
		internal static void EnqueueSkinned(SkinnedLatticeModifier modifier)
		{
			_skinnedModifiers.Add(modifier);
		}

		/// <summary>
		/// Sets up the modifiers as part of the player loop.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		internal static void Initialise()
		{
			if (_initialised) return;

			// Load compute shader
			_compute = Resources.Load<ComputeShader>(ComputeShaderName);

			// If couldn't find compute shader, log error and exit early
			if (_compute == null)
			{
				Debug.LogError($"Could not load lattice compute. Make sure it's within a Resources folder and called {ComputeShaderName}");
				return;
			}

			if (!Application.isEditor) 
				Application.quitting += Cleanup;

			// Create the buffer for storing lattice information
			_latticeBuffer = new(MaxHandles, 3 * sizeof(float));

			// Setup compute
			_compute.GetKernelThreadGroupSizes(0, out _deformGroupSize, out uint _, out uint _);
			_compute.GetKernelThreadGroupSizes(1, out _resetGroupSize, out uint _, out uint _);
			_compute.SetBuffer(0, LatticeBufferId, _latticeBuffer);

			// Add to player loop
			AddToPlayerLoop();

			_initialised = true;
		}

		/// <summary>
		/// Performs cleanup of lattice related things.
		/// </summary>
		internal static void Cleanup()
		{
			if (!_initialised) return;

			if (!Application.isEditor) 
				Application.quitting -= Cleanup;

			// Release the lattice buffer
			_latticeBuffer?.Release();
			_latticeBuffer = null;

			// Clear existing modifiers
			_modifiers.Clear();
			_skinnedModifiers.Clear();

			// Remove from player loop
			RemoveFromPlayerLoop();

			_initialised = false;
		}

		#endregion

		#region Private Methods

		private static void ApplyModifiers()
		{
			if (_modifiers.Count == 0 || _latticeBuffer == null) return;

			// These two calls are only here because editing the compute shader will cause
			// the asset to refresh and these values will be lost.
			_compute.GetKernelThreadGroupSizes(0, out _deformGroupSize, out uint _, out uint _);
			_compute.GetKernelThreadGroupSizes(0, out _resetGroupSize, out uint _, out uint _);
			_compute.SetBuffer(0, LatticeBufferId, _latticeBuffer);

			// Setup command buffer
			_cmd.Clear();
			_cmd.name = "Lattice Modifiers";

			// Apply all modifiers
			for (int i = 0; i < _modifiers.Count; i++)
			{
				ApplyModifier(_cmd, _modifiers[i]);
			}

			// Execute
			Graphics.ExecuteCommandBuffer(_cmd);

			// Clear modifier queue
			_modifiers.Clear();
		}

		private static void ApplyModifier(CommandBuffer cmd, LatticeModifierBase modifier)
		{
			if (modifier == null || !modifier.IsValid) return;

			// Set modifier keywords
			SetKeywords(cmd, modifier.ApplyMethod);

			// Copy original buffer back onto vertex buffer
			cmd.CopyBuffer(modifier.CopyBuffer, modifier.VertexBuffer);

			// Set vertex buffers
			cmd.SetComputeBufferParam(_compute, 0, VertexBufferId, modifier.VertexBuffer);
			if (modifier.ApplyMethod == ApplyMethod.StretchBuffer)
			{
				cmd.SetComputeBufferParam(_compute, 0, StretchBufferId, modifier.StretchBuffer);
			}

			// Setup mesh info
			MeshInfo info = modifier.MeshInfo;
			SetMeshInfo(cmd, info);

			// Reset stretch
			if (modifier.ApplyMethod == ApplyMethod.StretchBuffer)
			{
				cmd.SetComputeBufferParam(_compute, 1, StretchBufferId, modifier.StretchBuffer);
				cmd.DispatchCompute(_compute, 1, info.VertexCount / (int)_resetGroupSize + 1, 1, 1);
			}

			// Apply lattices
			Matrix4x4 localToWorld = modifier.LocalToWorld;
			List<LatticeItem> lattices = modifier.Lattices;
			for (int i = 0; i < lattices.Count; i++)
			{
				LatticeItem latticeItem = lattices[i];
				Lattice lattice = latticeItem.Lattice;

				if (lattice == null || !lattice.isActiveAndEnabled) continue;

				// Set lattice parameters
				cmd.SetKeyword(HighQualityKeyword, latticeItem.HighQuality);
				cmd.SetKeyword(ZeroOutsideKeyword, !latticeItem.Global);

				Matrix4x4 objectToLattice = lattice.transform.worldToLocalMatrix * localToWorld;
				Matrix4x4 latticeToObject = objectToLattice.inverse;
				cmd.SetComputeMatrixParam(_compute, ObjectToLatticeId, objectToLattice);
				cmd.SetComputeMatrixParam(_compute, LatticeToObjectId, latticeToObject);

				_latticeResolution[0] = lattice.Resolution.x;
				_latticeResolution[1] = lattice.Resolution.y;
				_latticeResolution[2] = lattice.Resolution.z;
				cmd.SetComputeIntParams(_compute, LatticeResolutionId, _latticeResolution);

				// Set lattice offsets
				cmd.SetBufferData(_latticeBuffer, lattice.Offsets);

				// Apply lattice
				cmd.DispatchCompute(_compute, 0, info.VertexCount / (int)_deformGroupSize + 1, 1, 1);
			}
		}

		private static void ApplySkinnedModifiers()
		{
			if (_skinnedModifiers.Count == 0 || _latticeBuffer == null) return;

			// Setup command buffer
			_cmd.Clear();
			_cmd.name = "Skinned Lattice Modifiers";

			// Apply all modifiers
			for (int i = 0; i < _skinnedModifiers.Count; i++)
			{
				ApplySkinnedModifier(_cmd, _skinnedModifiers[i]);
			}

			// Execute
			Graphics.ExecuteCommandBuffer(_cmd);

			// Clear modifier queue
			_skinnedModifiers.Clear();
		}

		private static void ApplySkinnedModifier(CommandBuffer cmd, SkinnedLatticeModifier modifier)
		{
			if (modifier == null || !modifier.IsSkinnedValid) return;

			// Set modifier keywords
			SetKeywords(cmd, modifier.ApplyMethod);

			// Set vertex buffers
			cmd.SetComputeBufferParam(_compute, 0, VertexBufferId, modifier.SkinnedVertexBuffer);
			if (modifier.ApplyMethod == ApplyMethod.StretchBuffer)
			{
				cmd.SetComputeBufferParam(_compute, 0, StretchBufferId, modifier.StretchBuffer);
			}

			// Setup mesh info
			MeshInfo info = modifier.MeshInfo;
			SetMeshInfo(cmd, info);

			// Apply skinned lattices
			Matrix4x4 localToWorld = modifier.SkinnedLocalToWorld;
			List<LatticeItem> lattices = modifier.SkinnedLattices;
			for (int i = 0; i < lattices.Count; i++)
			{
				LatticeItem latticeItem = lattices[i];//
				Lattice lattice = latticeItem.Lattice;

				if (lattice == null || !lattice.isActiveAndEnabled) continue;

				// Set lattice parameters
				cmd.SetKeyword(HighQualityKeyword, latticeItem.HighQuality);
				cmd.SetKeyword(ZeroOutsideKeyword, !latticeItem.Global);

				Matrix4x4 objectToLattice = lattice.transform.worldToLocalMatrix * localToWorld;
				Matrix4x4 latticeToObject = objectToLattice.inverse;
				cmd.SetComputeMatrixParam(_compute, ObjectToLatticeId, objectToLattice);
				cmd.SetComputeMatrixParam(_compute, LatticeToObjectId, latticeToObject);

				_latticeResolution[0] = lattice.Resolution.x;
				_latticeResolution[1] = lattice.Resolution.y;
				_latticeResolution[2] = lattice.Resolution.z;
				cmd.SetComputeIntParams(_compute, LatticeResolutionId, _latticeResolution);

				// Set lattice offsets
				cmd.SetBufferData(_latticeBuffer, lattice.Offsets);

				// Apply lattice
				cmd.DispatchCompute(_compute, 0, info.VertexCount / (int)_deformGroupSize + 1, 1, 1);
			}
		}

		/// <summary>
		/// Sets the compute shader's keywords
		/// </summary>
		private static void SetKeywords(CommandBuffer cmd, ApplyMethod applyMethod)
		{
			cmd.SetKeyword(NormalsKeyword, applyMethod >= ApplyMethod.PositionNormalTangent);
			cmd.SetKeyword(ApplyStretchKeyword, applyMethod >= ApplyMethod.Stretch);
			cmd.SetKeyword(MultipleBuffersKeyword, applyMethod >= ApplyMethod.StretchBuffer);
		}

		/// <summary>
		/// Sets the compute shader's mesh related properties
		/// </summary>
		private static void SetMeshInfo(CommandBuffer cmd, MeshInfo info)
		{
			cmd.SetComputeIntParam(_compute, VertexCountId,    info.VertexCount);
			cmd.SetComputeIntParam(_compute, BufferStrideId,   info.BufferStride);
			cmd.SetComputeIntParam(_compute, StretchStrideId,  info.StretchStride);
			cmd.SetComputeIntParam(_compute, PositionOffsetId, info.PositionOffset);
			cmd.SetComputeIntParam(_compute, NormalOffsetId,   info.NormalOffset);
			cmd.SetComputeIntParam(_compute, TangentOffsetId,  info.TangentOffset);
			cmd.SetComputeIntParam(_compute, StretchOffsetId,  info.StretchOffset);
		}

		/// <summary>
		/// Updates the player loop to include the modifier systems.
		/// </summary>
		private static void AddToPlayerLoop()
		{
			// Get the player loop
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			// Get the PostLateUpdate system
			int postLateUpdateIndex = Array.FindIndex(loop.subSystemList, system => system.type == typeof(PostLateUpdate));
			var postLateUpdate = loop.subSystemList[postLateUpdateIndex];

			// Get the UpdateAllSkinnedMeshes system index
			var postLateSystems = new List<PlayerLoopSystem>(postLateUpdate.subSystemList);
			var skinned = postLateSystems.FindIndex(system => system.type == typeof(PostLateUpdate.UpdateAllSkinnedMeshes));

			// Insert the static modifier before the skinned mesh system,
			// This allows the skinning system to use the lattice modified meshes
			postLateSystems.Insert(skinned, new()
			{
				updateDelegate = ApplyModifiers,
				type = typeof(LatticeFeature)
			});

			// Insert the skinned modifier after the skinned mesh system
			// This allows the modifier system to use the skin modified meshes
			postLateSystems.Insert(skinned + 2, new()
			{
				updateDelegate = ApplySkinnedModifiers,
				type = typeof(LatticeFeature)
			});

			// Update the systems
			postLateUpdate.subSystemList = postLateSystems.ToArray();
			loop.subSystemList[postLateUpdateIndex] = postLateUpdate;

			// Set updated player loop
			PlayerLoop.SetPlayerLoop(loop);
		}

		/// <summary>
		/// Removes the modifier systems from the player loop.
		/// </summary>
		private static void RemoveFromPlayerLoop()
		{
			// Get the player loop
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			// Get the PostLateUpdate system
			int postLateUpdateIndex = Array.FindIndex(loop.subSystemList, system => system.type == typeof(PostLateUpdate));
			var postLateUpdate = loop.subSystemList[postLateUpdateIndex];

			// Remove all systems related to the lattice feature
			var postLateSystems = new List<PlayerLoopSystem>(postLateUpdate.subSystemList);
			postLateSystems.RemoveAll(system => system.type == typeof(LatticeFeature));

			// Update the systems
			postLateUpdate.subSystemList = postLateSystems.ToArray();
			loop.subSystemList[postLateUpdateIndex] = postLateUpdate;

			// Set updated player loop
			PlayerLoop.SetPlayerLoop(loop);
		}

		#endregion
	}
}