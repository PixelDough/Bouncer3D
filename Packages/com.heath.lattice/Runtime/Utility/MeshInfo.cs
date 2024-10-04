namespace Lattice
{
	/// <summary>
	/// Information about the vertex buffer of a mesh, 
	/// including count, stride and offsets.
	/// </summary>
	public struct MeshInfo
	{
		public int VertexCount;
		public int BufferStride;
		public int StretchStride;
		public int PositionOffset;
		public int NormalOffset;
		public int TangentOffset;
		public int StretchOffset;
	}
}
