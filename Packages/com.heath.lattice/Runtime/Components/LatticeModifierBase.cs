using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

namespace Lattice
{
	abstract public class LatticeModifierBase : MonoBehaviour
	{
		#region Constants

		private const string TargetMeshTooltip = 
			"The mesh to apply deformations to and to render on this object.";

		private const string StretchTargetTooltip =
			"Which UV channel to apply stretching to. Will overwrite existing data.";

		private const string UpdateModeTooltip =
			"When deformations should update. Only applies to non skinning lattices. " +  
			"If set to Manual, you must call RequestUpdate().";

		private const string LatticesTooltip =
			"Lattices to apply to the target mesh. " + 
			"These will be applied in order and before skinning.";

		private const GraphicsBuffer.Target BufferTargets = GraphicsBuffer.Target.Raw
			| GraphicsBuffer.Target.CopySource | GraphicsBuffer.Target.CopyDestination;

		#endregion

		#region Enums

		[Serializable]
		private enum TexCoord : int
		{
			None = -1,
			TexCoord0 = 0,
			TexCoord1 = 1,
			TexCoord2 = 2,
			TexCoord3 = 3,
			TexCoord4 = 4,
			TexCoord5 = 5,
			TexCoord6 = 6,
			TexCoord7 = 7,
		}

		#endregion

		#region Fields

		[SerializeField, NotKeyable, Tooltip(TargetMeshTooltip)] 
		private Mesh _targetMesh;

		[SerializeField, NotKeyable, Tooltip(StretchTargetTooltip)] 
		private TexCoord _stretchTarget = TexCoord.None;

		[SerializeField, Tooltip(UpdateModeTooltip)]
		private UpdateMode _updateMode = UpdateMode.WhenVisible;

		[SerializeField, Tooltip(LatticesTooltip)] 
		private List<LatticeItem> _lattices = new();

		private Mesh _mesh;
		private MeshInfo _meshInfo;

		private ApplyMethod _applyMethod;

		private GraphicsBuffer _copyBuffer;
		private GraphicsBuffer _vertexBuffer;
		private GraphicsBuffer _stretchBuffer;

		private bool _ranThisFrame = false;
		private Mesh _currentTargetMesh;
		private TexCoord _currentStretchTarget;

		#endregion

		#region Properties

		/// <summary>
		/// Lattices to apply to this mesh.
		/// </summary>
		public List<LatticeItem> Lattices => _lattices;

		/// <summary>
		/// Method of updating and applying the lattices.
		/// </summary>
		public UpdateMode UpdateMode { get => _updateMode; set => _updateMode = value; }

		/// <summary>
		/// Vertex information about this mesh.
		/// </summary>
		internal MeshInfo MeshInfo => _meshInfo;

		/// <summary>
		/// Settings for how the deformation will be applied.
		/// </summary>
		internal ApplyMethod ApplyMethod => _applyMethod;

		/// <summary>
		/// A copy of this mesh's vertex buffer.
		/// </summary>
		internal GraphicsBuffer CopyBuffer => _copyBuffer;

		/// <summary>
		/// The mesh's vertex buffer.
		/// </summary>
		internal GraphicsBuffer VertexBuffer => _vertexBuffer;

		/// <summary>
		/// The mesh's stretch buffer.
		/// </summary>
		internal GraphicsBuffer StretchBuffer => _stretchBuffer;

		/// <summary>
		/// The mesh's local to world transform matrix.
		/// </summary>
		internal Matrix4x4 LocalToWorld => transform.localToWorldMatrix;

		/// <summary>
		/// Whether the component is valid and can be applied without errors.
		/// </summary>
		internal bool IsValid => _vertexBuffer != null && _copyBuffer != null;

		#endregion

		#region Public Methods

		/// <summary>
		/// Will force an update of the Lattice deformations this frame.
		/// </summary>
		public void RequestUpdate()
		{
			EnqueueIfNeeded(true);
		}

		/// <summary>
		/// Applies the lattice effect mesh to the renderer.
		/// </summary>
		internal void ApplyMesh()
		{
			SetMesh(_mesh);
		}

		/// <summary>
		/// Applies the original target mesh to the renderer.
		/// </summary>
		internal void ResetMesh()
		{
			SetMesh(_targetMesh);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Releases buffers used for lattice deforming.
		/// </summary>
		protected virtual void Release()
		{
			_copyBuffer?.Release();
			_copyBuffer = null;

			_vertexBuffer?.Release();
			_vertexBuffer = null;

			_stretchBuffer?.Release();
			_stretchBuffer = null;

			_mesh = null;
		}

		/// <summary>
		/// Gets the current mesh, either from a mesh filter or skinned mesh renderer.
		/// </summary>
		protected abstract Mesh GetMesh();

		/// <summary>
		/// Sets a mesh, either on a mesh filter or skinned mesh renderer.
		/// </summary>
		protected abstract void SetMesh(Mesh mesh);

		/// <summary>
		/// Queues the modifier to be run this frame.
		/// </summary>
		protected abstract void Enqueue(bool ignoreMode);

		#endregion

		#region Private Methods

		private void EnqueueIfNeeded(bool ignoreMode)
		{
			if (!_ranThisFrame && IsValid)
			{
				Enqueue(ignoreMode);
				_ranThisFrame = true;
			}
		}

		private void Initialise()
		{
			// Try get target mesh if one is not set
			if (_targetMesh == null)
			{
				_targetMesh = GetMesh();
			}

			_currentStretchTarget = _stretchTarget;
			_currentTargetMesh = _targetMesh;

			// If still no target mesh, log warning and exit early
			if (_targetMesh == null)
			{
				Debug.LogWarning("No target mesh set. Can not initialise lattice modifier.", this);
				return;
			}

			// If not readable, log error and exit early
			if (!_targetMesh.isReadable)
			{
				Debug.LogError("Target does not have read/write enabled. Enable it in the model import settings.", _targetMesh);
				return;
			}

			_applyMethod = ApplyMethod.PositionOnly;

			// Create a copy which the lattice will be applied to
			_mesh = Instantiate(_targetMesh);
			_mesh.hideFlags |= HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
			_mesh.name = _targetMesh.name + " (Lattice)";
			_mesh.vertexBufferTarget |= BufferTargets;

			bool hasNormals = _mesh.HasVertexAttribute(VertexAttribute.Normal);
			bool hasTangents = _mesh.HasVertexAttribute(VertexAttribute.Tangent);
			bool applyStretch = _stretchTarget != TexCoord.None;

			if (hasNormals && hasTangents)
			{
				_applyMethod = ApplyMethod.PositionNormalTangent;
			}
			else if (applyStretch)
			{
				applyStretch = false;
				Debug.LogWarning("Cannot apply stretch if the mesh does not have normals and tangents.");
			}

			// Add stretch and squish vertex channel
			if (applyStretch)
			{
				// Create stretch array of all ones
				Vector2[] stretch = new Vector2[_mesh.vertexCount];
				Array.Fill(stretch, Vector2.one);

				// Add to mesh
				int stretchChannel = (int)_stretchTarget;
				_mesh.SetUVs(stretchChannel, stretch);

				// Get mesh stretch information
				VertexAttribute stretchAttribute = VertexAttribute.TexCoord0 + stretchChannel;
				int stretchStream = _mesh.GetVertexAttributeStream(stretchAttribute);
				_meshInfo.StretchStride = _mesh.GetVertexBufferStride(stretchStream);
				_meshInfo.StretchOffset = _mesh.GetVertexAttributeOffset(stretchAttribute);

				// Get stretch buffer
				_applyMethod = stretchStream != 0 ? ApplyMethod.StretchBuffer : ApplyMethod.Stretch;
				_stretchBuffer = _mesh.GetVertexBuffer(stretchStream);
			}

			// Get mesh information
			_meshInfo.VertexCount = _mesh.vertexCount;
			_meshInfo.BufferStride = _mesh.GetVertexBufferStride(0);
			_meshInfo.PositionOffset = _mesh.GetVertexAttributeOffset(VertexAttribute.Position);
			_meshInfo.NormalOffset = _mesh.GetVertexAttributeOffset(VertexAttribute.Normal);
			_meshInfo.TangentOffset = _mesh.GetVertexAttributeOffset(VertexAttribute.Tangent);

			// Get vertex buffer
			_vertexBuffer = _mesh.GetVertexBuffer(0);

			// Create copy of vertex buffer
			// Used for resetting to original every frame
			_copyBuffer = new GraphicsBuffer(
				BufferTargets,
				_meshInfo.VertexCount,
				_meshInfo.BufferStride
			);
			Graphics.CopyBuffer(_vertexBuffer, _copyBuffer);
		}

		#endregion

		#region Unity Methods

#if UNITY_EDITOR
		private void Update()
		{
			// If target mesh or stretch target have been changed in inspector
			if ((_targetMesh != _currentTargetMesh) || (_stretchTarget != _currentStretchTarget))
			{
				// Reset component
				OnDisable();
				OnEnable();
			}

			// Ensure mesh on the renderer is the lattice affected one
			if (_mesh != null && _mesh != GetMesh())
			{
				ApplyMesh();
			}
		}
#endif

		private void LateUpdate()
		{
			EnqueueIfNeeded(false);
			_ranThisFrame = false;
		}

		private void OnEnable()
		{
			// Initialise everything
			Initialise();
			ApplyMesh();
			EnqueueIfNeeded(true);
		}

		private void OnDisable()
		{
			// Deinitialise everything
			ResetMesh();
			Release();
		}

		#endregion
	}
}
