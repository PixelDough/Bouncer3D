using UnityEngine;
using UnityEngine.Rendering;

namespace Lattice
{
	/// <summary>
	/// All lattice shader keywords and IDs
	/// </summary>
	public static class LatticeShaderProperties
	{
		public static readonly GlobalKeyword HighQualityKeyword     = GlobalKeyword.Create("LATTICE_HIGH_QUALITY");
		public static readonly GlobalKeyword ZeroOutsideKeyword     = GlobalKeyword.Create("LATTICE_ZERO_OUTSIDE");
		public static readonly GlobalKeyword NormalsKeyword		    = GlobalKeyword.Create("LATTICE_NORMALS"); 
		public static readonly GlobalKeyword ApplyStretchKeyword    = GlobalKeyword.Create("LATTICE_APPLY_STRETCH");
		public static readonly GlobalKeyword MultipleBuffersKeyword = GlobalKeyword.Create("LATTICE_MULTIPLE_BUFFERS");

		public static readonly int VertexCountId       = Shader.PropertyToID("VertexCount");
		public static readonly int BufferStrideId      = Shader.PropertyToID("BufferStride");
		public static readonly int StretchStrideId     = Shader.PropertyToID("StretchStride");
		public static readonly int PositionOffsetId    = Shader.PropertyToID("PositionOffset");
		public static readonly int NormalOffsetId      = Shader.PropertyToID("NormalOffset");
		public static readonly int TangentOffsetId     = Shader.PropertyToID("TangentOffset");
		public static readonly int StretchOffsetId     = Shader.PropertyToID("StretchOffset");
		public static readonly int VertexBufferId      = Shader.PropertyToID("VertexBuffer");
		public static readonly int StretchBufferId     = Shader.PropertyToID("StretchBuffer");
		public static readonly int LatticeBufferId     = Shader.PropertyToID("LatticeBuffer");
		public static readonly int ObjectToLatticeId   = Shader.PropertyToID("ObjectToLattice");
		public static readonly int LatticeToObjectId   = Shader.PropertyToID("LatticeToObject");
		public static readonly int LatticeResolutionId = Shader.PropertyToID("LatticeResolution");
	}
}