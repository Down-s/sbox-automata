using System.Runtime.InteropServices;
using Sandbox;

[StructLayout(LayoutKind.Sequential)]
struct CellVertex
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector3 Tangent;
	public Vector2 TexCoord;
	public Color Color;

	public CellVertex(Vector3 pos, Vector3 norm, Vector3 tan, Vector2 texCoord, Color col)
	{
		Position = pos;
		Normal = norm;
		Tangent = tan;
		TexCoord = texCoord;
		Color = col;
	}

	public static readonly VertexAttribute[] Layout =
	{
		new VertexAttribute( VertexAttributeType.Position, VertexAttributeFormat.Float32, 3),
		new VertexAttribute( VertexAttributeType.Normal, VertexAttributeFormat.Float32, 3),
		new VertexAttribute( VertexAttributeType.Tangent, VertexAttributeFormat.Float32, 3),
		new VertexAttribute( VertexAttributeType.TexCoord, VertexAttributeFormat.Float32, 2),
		new VertexAttribute( VertexAttributeType.Color, VertexAttributeFormat.Float32, 4)
	};
}
