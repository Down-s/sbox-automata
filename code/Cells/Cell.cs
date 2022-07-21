namespace Automata;
public struct Vec3
{
	public Vec3( int x, int y, int z )
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public int x;
	public int y;
	public int z;
}

public struct Cell
{
	public Cell(byte state, Color color)
	{
		State = state;
		Color = color;
	}

	public byte State;
	public Color Color;
}
