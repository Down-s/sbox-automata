using Sandbox;
using System;
using System.Collections.Generic;

namespace Automata;
public class CellGrid : ModelEntity
{
	private byte[] Cells;
	private byte[] LastCells;

	private float LastSimulate;

	public int BoundsSize;

	public bool Simulating;

	SceneObject SceneObj;

	public CellGrid()
	{
		BoundsSize = Automata.BoundsSize;
		Cells = new byte[BoundsSize * BoundsSize * BoundsSize];
		LastCells = new byte[BoundsSize * BoundsSize * BoundsSize];

		GenerateGrid();
	}

	public void GenerateGrid()
	{
		GameRules Rules = Automata.Rules;
		float SpawnChance = Automata.SpawnChance / 100.0f;
		for ( int i = 0; i < Cells.Length; i++ )
			Cells[i] = 0;

		System.Random random = new System.Random();
		int Radius = (int) MathX.Floor(BoundsSize * Automata.SpawnRadius * 0.5f );

		switch ( Automata.SelectedShape )
		{
			case Shape.Cube:
				{
					for ( int x = BoundsSize / 2 - Radius; x <= BoundsSize / 2 + Radius; x++ )
					{
						for ( int y = BoundsSize / 2 - Radius; y <= BoundsSize / 2 + Radius; y++ )
						{
							for ( int z = BoundsSize / 2 - Radius; z <= BoundsSize / 2 + Radius; z++ )
							{
								if ( SpawnChance >= random.Float() )
								{
									int index = GetCellAtPoint( x, y, z );
									Cells[index] = (byte) Rules.State;
								}
							}
						}
					}

					break;
				}
			case Shape.Sphere:
				{
					for ( int x = BoundsSize / 2 - Radius; x <= BoundsSize / 2 + Radius; x++ )
					{
						for ( int y = BoundsSize / 2 - Radius; y <= BoundsSize / 2 + Radius; y++ )
						{
							for ( int z = BoundsSize / 2 - Radius; z <= BoundsSize / 2 + Radius; z++ )
							{
								if ( SpawnChance >= random.Float() )
								{
									int index = GetCellAtPoint( x, y, z );

									float x1 = (x - BoundsSize * 0.5f);
									float y1 = (y - BoundsSize * 0.5f);
									float z1 = (z - BoundsSize * 0.5f);

									if ( SpawnChance >= random.Float() && x1 * x1 + y1 * y1 + z1 * z1 < Radius * Radius)
										Cells[index] = (byte) Rules.State;
								}
							}
						}
					}

					break;
				}
			case Shape.Diamond:
				{
					for ( int x = 0; x <= BoundsSize; x++ )
					{
						for ( int y = 0; y <= BoundsSize; y++ )
						{
							for ( int z = 0; z <= BoundsSize; z++ )
							{
								if ( SpawnChance >= random.Float() )
								{
									int index = GetCellAtPoint( x, y, z );

									float dx = Math.Abs(x - BoundsSize * 0.5f);
									float dy = Math.Abs( y - BoundsSize * 0.5f);
									float dz = Math.Abs( z - BoundsSize * 0.5f);
									float Size = BoundsSize;

									if ( SpawnChance >= random.Float() && (dx / BoundsSize + dy / BoundsSize + dz / BoundsSize <= (0.5f * Automata.SpawnRadius)) )
										Cells[index] = (byte) Rules.State;
								}
							}
						}
					}

					break;
				}
		}
	}

	private int GetCellAtPoint( int x, int y, int z)
	{
		// Make the cells wrap around the bounds
		x = x % BoundsSize;
		y = y % BoundsSize;
		z = z % BoundsSize;
		if ( x < 0 ) x += BoundsSize;
		if ( y < 0 ) y += BoundsSize;
		if ( z < 0 ) z += BoundsSize;

		return x + y * BoundsSize + z * BoundsSize * BoundsSize;
	}

	private static void CreateQuad(List<CellVertex> verts, Ray origin, Vector3 width, Vector3 height, Color color)
	{
		Vector3 normal = origin.Direction;
		Vector4 tangent = new Vector4( width.Normal, 1 );

		CellVertex a = new CellVertex( origin.Origin - width - height, normal, tangent, new Vector2( 0, 0 ), color );
		CellVertex b = new CellVertex( origin.Origin + width - height, normal, tangent, new Vector2( 1, 0 ), color );
		CellVertex c = new CellVertex( origin.Origin + width + height, normal, tangent, new Vector2( 1, 1 ), color );
		CellVertex d = new CellVertex( origin.Origin - width + height, normal, tangent, new Vector2( 0, 1 ), color );

		verts.Add( a );
		verts.Add( b );
		verts.Add( c );

		verts.Add( c );
		verts.Add( d );
		verts.Add( a );
	}

	private readonly Color[] StateColors = new Color[]
	{
		Color.Black, // 0
		new Color(1.0f, 0.9f, 0.8f), // 1
		new Color(1.0f, 0.8f, 0.0f), // 2
		new Color(1.0f, 0.5f, 0.0f), // 3
		new Color(1.0f, 0.2f, 0.0f), // 4
		new Color(1.0f, 0.0f, 0.0f), // 5
		new Color(1.0f, 0.0f, 0.0f),
		new Color(1.0f, 0.0f, 0.1f),
		new Color(1.0f, 0.0f, 0.2f),
		new Color(1.0f, 0.0f, 0.3f),
		new Color(1.0f, 0.0f, 0.4f),
		new Color(1.0f, 0.0f, 0.5f),
		new Color(1.0f, 0.0f, 0.6f),
		new Color(1.0f, 0.0f, 0.7f),
		new Color(1.0f, 0.0f, 0.8f),
		new Color(1.0f, 0.0f, 0.9f),
		new Color(1.0f, 0.0f, 1.0f),
		new Color(1.0f, 0.1f, 1.0f),
		new Color(1.0f, 0.2f, 1.0f),
		new Color(1.0f, 0.3f, 1.0f),
		new Color(1.0f, 0.4f, 1.0f)
	};

	private List<CellVertex> GenerateMesh()
	{
		List<CellVertex> verts = new List<CellVertex>();

		for ( int x = 0; x < BoundsSize; x++ )
		{
			for ( int y = 0; y < BoundsSize; y++ )
			{
				for ( int z = 0; z < BoundsSize; z++ )
				{
					int index = GetCellAtPoint( x, y, z);
					if ( Cells[index] <= 0 ) continue;

					Vector3 pos = new Vector3( x, y, z );
					Rotation rot = Rotation.Identity;

					var f = rot.Forward * 0.5f;
					var l = rot.Left * 0.5f;
					var u = rot.Up * 0.5f;

					Color col;
					switch (Automata.SelectedColorMap)
					{
						case ColorMap.Position:
							{
								col = new Color( (float) x / BoundsSize, (float) y / BoundsSize, (float) z / BoundsSize );
								break;
							}
						case ColorMap.State:
							{
								col = StateColors[Cells[index]];
								break;
							}
						default:
							{
								col = StateColors[Cells[index]];
								break;
							}
					}

					CreateQuad( verts, new Ray( pos + f, f.Normal ), l, u, col );
					CreateQuad( verts, new Ray( pos - f, f.Normal ), l, -u, col );
					CreateQuad( verts, new Ray( pos + l, l.Normal ), -f, u, col );
					CreateQuad( verts, new Ray( pos - l, -l.Normal ), f, u, col );
					CreateQuad( verts, new Ray( pos + u, u.Normal ), f, l, col );
					CreateQuad( verts, new Ray( pos - u, -u.Normal ), f, -l, col );
				}
			}
		}
		
		return verts;
	}

	private static readonly Dictionary<NeighborCheck, Vec3[]> CheckPoints = new()
	{
		[NeighborCheck.Moore] = new Vec3[]
		{
			new Vec3(-1, -1, -1),
			new Vec3(0, -1, -1),
			new Vec3(1, -1, -1),

			new Vec3(-1, 0, -1),
			new Vec3(0, 0, -1),
			new Vec3(1, 0, -1),

			new Vec3(-1, 1, -1),
			new Vec3(0, 1, -1),
			new Vec3(1, 1, -1),

			new Vec3(-1, -1, 0),
			new Vec3(0, -1, 0),
			new Vec3(1, -1, 0),

			new Vec3(-1, 0, 0),
			//new Vec3(0, 0, 0),
			new Vec3(1, 0, 0),

			new Vec3(-1, 1, 0),
			new Vec3(0, 1, 0),
			new Vec3(1, 1, 0),

			new Vec3(-1, -1, 1),
			new Vec3(0, -1, 1),
			new Vec3(1, -1, 1),

			new Vec3(-1, 0, 1),
			new Vec3(0, 0, 1),
			new Vec3(1, 0, 1),

			new Vec3(-1, 1, 1),
			new Vec3(0, 1, 1),
			new Vec3(1, 1, 1),
		},
		[NeighborCheck.VonNeuman] = new Vec3[]
		{
			new Vec3(1, 0, 0),
			new Vec3(0, 1, 0),
			new Vec3(0, 0, 1),
			new Vec3(0, 0, -1),
			new Vec3(0, -1, 0),
			new Vec3(-1, 0, 0),
		}
	};

	private int GetNeighborsAtPoint(int x, int y, int z)
	{
		GameRules Rules = Automata.Rules;
		int neighbors = 0;
		int state = Rules.State;
		Vec3[] CheckPositions = CheckPoints[Rules.CheckType];
		for (int i = 0; i < CheckPositions.Length; i++)
		{
			Vec3 checkPos = CheckPositions[i];
			if ( LastCells[GetCellAtPoint( x + checkPos.x, y + checkPos.y, z + checkPos.z )] == state )
			{
				neighbors++;
			}
		}

		return neighbors;

		/*int neighbors = 0;
		int state = Rules.State;
		for ( int i = -1; i <= 1; i++ )
		{
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int k = -1; k <= 1; k++ )
				{
					if ( i == 0 && j == 0 && k == 0 )
						continue;

					if ( LastCells[GetCellAtPoint( x + i, y + j, z + k )] == state )
						neighbors++;
				}
			}
		}

		return neighbors;*/
	}

	private void UpdateGame()
	{
		GameRules Rules = Automata.Rules;
		for ( int i = 0; i < Cells.Length; i++ )
			LastCells[i] = Cells[i];

		HashSet<int> Survive = Rules.Survive;
		HashSet<int> Birth = Rules.Birth;
		int State = Rules.State;
		for ( int x = 0; x < BoundsSize; x++ )
		{
			for ( int y = 0; y < BoundsSize; y++ )
			{
				for ( int z = 0; z < BoundsSize; z++ )
				{
					int neighbors = GetNeighborsAtPoint( x, y, z );
					int index = GetCellAtPoint( x, y, z );
					
					if ( LastCells[index] <= 0 )
					{
						if ( Birth.Contains(neighbors) )
						{
							Cells[index] = (byte) State;
						}
					}
					else
					{
						if ( !Survive.Contains(neighbors) || LastCells[index] != State )
						{
							Cells[index]--;
						}
					}
				}
			}
		}
	}

	[Event.Tick.Client]
	public void Tick()
	{
		if ( !Simulating ) return;

		if ( LastSimulate > Time.Now ) return;
		LastSimulate = Time.Now + Automata.Interval;

		UpdateGame();
		ReloadMesh();
	}

	public void ReloadMesh()
	{
		List<CellVertex> verts = GenerateMesh();
		Vector3 bounds = new Vector3( BoundsSize, BoundsSize, BoundsSize );

		Mesh mesh = new Mesh( Material.Load( "materials/default/vertex_color.vmat" ) );
		mesh.CreateVertexBuffer( verts.Count, CellVertex.Layout, verts );
		mesh.SetBounds( -bounds, bounds );

		ModelBuilder builder = new ModelBuilder();
		builder.AddMesh( mesh );

		var transform = new Transform( Vector3.Zero );

		if ( SceneObj != null )
			SceneObj.Delete();

		SceneObj = new SceneObject( Map.Scene, builder.Create(), transform );
	}

	public void ClearGrid()
	{
		Array.Clear( Cells );
		Array.Clear( LastCells );

		ReloadMesh();
	}
}
