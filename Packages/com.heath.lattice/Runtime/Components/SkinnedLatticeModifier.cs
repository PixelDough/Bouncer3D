using System.Collections.Generic;
using UnityEngine;

namespace Lattice
{
	[ExecuteAlways] 
	[DisallowMultipleComponent] 
	[RequireComponent(typeof(SkinnedMeshRenderer))]
	public class SkinnedLatticeModifier : LatticeModifierBase
	{
		#region Constants

		private const string SkinnedLatticesTooltip =
			"Lattices to apply to the target mesh. " +
			"These will be applied in order and after skinning.";

		#endregion

		#region Fields

		[SerializeField, Tooltip(SkinnedLatticesTooltip)] 
		private List<LatticeItem> _skinnedLattices = new();

		private SkinnedMeshRenderer _skinnedMeshRenderer;
		private GraphicsBuffer _skinnedVertexBuffer;
		private Matrix4x4 _skinnedLocalToWorld;

		#endregion

		#region Properties

		/// <summary>
		/// Skinned lattices to apply.
		/// </summary>
		public List<LatticeItem> SkinnedLattices => _skinnedLattices;

		/// <summary>
		/// Gets the current skinned vertex buffer.
		/// </summary>
		internal GraphicsBuffer SkinnedVertexBuffer => _skinnedVertexBuffer;

		/// <summary>
		/// Gets the current skinned local to world matrix.
		/// </summary>
		internal Matrix4x4 SkinnedLocalToWorld => _skinnedLocalToWorld;

		/// <inheritdoc cref="LatticeModifierBase.IsValid"/>
		internal bool IsSkinnedValid => IsValid && _skinnedVertexBuffer != null;

		/// <summary>
		/// Retrieves the skinned mesh renderer.
		/// </summary>
		private SkinnedMeshRenderer MeshRenderer => (_skinnedMeshRenderer == null)
			? _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>()
			: _skinnedMeshRenderer;

		#endregion

		#region Protected Methods

		/// <inheritdoc cref="LatticeModifierBase.GetMesh"/>
		protected override Mesh GetMesh()
		{
			return MeshRenderer.sharedMesh;
		}

		/// <inheritdoc cref="LatticeModifierBase.SetMesh"/>
		protected override void SetMesh(Mesh mesh)
		{
			MeshRenderer.sharedMesh = mesh;
		}

		/// <inheritdoc cref="LatticeModifierBase.Release"/>
		protected override void Release()
		{
			base.Release();

			_skinnedVertexBuffer?.Release();
			_skinnedVertexBuffer = null;
		}

		/// <inheritdoc cref="LatticeModifierBase.Enqueue"/>
		protected override void Enqueue(bool ignoreMode)
		{
			bool isVisible = MeshRenderer.isVisible;

#if UNITY_EDITOR
			// Update when in editor mode and visible
			ignoreMode |= !Application.isPlaying && isVisible;
#endif

			if (ignoreMode || (UpdateMode == UpdateMode.Always) ||
				(isVisible && (UpdateMode == UpdateMode.WhenVisible)))
			{
				LatticeFeature.Enqueue(this);
			}


			if (ignoreMode || isVisible || MeshRenderer.updateWhenOffscreen)
			{
				// Ideally you cache the vertex buffer without releasing it every frame
				// But the skin renderer may swap to a new vertex buffer without disposing the previous
				// So no way to tell if it swapped within code :(
				_skinnedVertexBuffer?.Release();
				_skinnedVertexBuffer = null;

				// Update skinned vertex buffer
				_skinnedMeshRenderer.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
				_skinnedVertexBuffer = _skinnedMeshRenderer.GetVertexBuffer();

				// Update skinned local to world matrix
				if (MeshRenderer.rootBone != null)
				{
					// Skinning will apply transformations relative to root bone,
					// so we need to create a post skinned local to world 
					_skinnedLocalToWorld = Matrix4x4.TRS(MeshRenderer.rootBone.position,
						MeshRenderer.rootBone.rotation, Vector3.one);
				}
				else
				{
					_skinnedLocalToWorld = transform.localToWorldMatrix;
				}

				LatticeFeature.EnqueueSkinned(this);
			}
		}

		#endregion
	}
}
