using Sandbox;
using System.Collections.Generic;
using System.Text;

namespace Automata;
public enum NeighborCheck
{
	Moore,
	VonNeuman
}

public enum Shape
{
	Cube,
	Sphere,
	Diamond
}

public enum ColorMap
{
	Position,
	State
}

public struct GameRules
{
	public HashSet<int> Survive = new();
	public HashSet<int> Birth = new();
	public int State;
	public NeighborCheck CheckType;

	public GameRules(int survive, int birth, int state, NeighborCheck check = NeighborCheck.Moore)
	{
		Survive.Add(survive);
		Birth.Add(birth);
		State = state;
		CheckType = check;
	}

	public GameRules(HashSet<int> survive, HashSet<int> birth, int state, NeighborCheck check = NeighborCheck.Moore)
	{
		Survive = survive;
		Birth = birth;
		State = state;
		CheckType = check;
	}
}

public partial class Automata : Sandbox.Game
{
	public static CellGrid Grid;
	public static GameRules Rules = new GameRules( 4, 4, 5 );

#region ConVars
	[ConVar.Client(Name = "automata.survive")]
	public static string Survive {
		get
		{
			if ( Rules.Survive == null ) return "4";

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < 27; i++)
			{
				if (Rules.Survive.Contains(i))
				{
					builder.Append(i);
					builder.Append( ',' );
				}
			}

			if ( builder.Length == 0 ) return "";
			builder.Remove(builder.Length - 1, 1);
			return builder.ToString();
		}
		set
		{
			Log.Info( Rules.Survive );
			if ( Rules.Survive == null ) return;

			Rules.Survive.Clear();
			string[] values = value.Split(',' );
			foreach ( string val in values)
			{
				string[] range = val.Split("-");
				if (range.Length == 1)
				{
					if ( string.IsNullOrEmpty( range[0] ) ) return;
					Rules.Survive.Add(int.Parse(range[0]));
				}
				else
				{
					if ( string.IsNullOrEmpty( range[1] ) ) return;
					int start = int.Parse(range[0]);
					int end = int.Parse(range[1]);
					for (int i = start; i <= end; i++)
						Rules.Survive.Add(i);
				}
			}
		}
	}

	[ConVar.Client( Name = "automata.birth")]
	public static string Birth
	{
		get
		{
			if ( Rules.Birth == null ) return "4";

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < 27; i++)
			{
				if (Rules.Birth.Contains(i))
				{
					builder.Append(i);
					builder.Append( ',' );
				}
			}

			if ( builder.Length == 0 ) return "";
			builder.Remove(builder.Length - 1, 1);
			return builder.ToString();
		}
		set
		{
			if ( Rules.Birth == null ) return;

			Rules.Birth.Clear();
			string[] values = value.Split(',');
			foreach (string val in values)
			{
				string[] range = val.Split("-");
				if (range.Length == 1)
				{
					if ( string.IsNullOrEmpty( range[0] ) ) return;
					Rules.Birth.Add(int.Parse(range[0]));
				}
				else
				{
					if ( string.IsNullOrEmpty( range[1] ) ) return;
					int start = int.Parse(range[0]);
					int end = int.Parse(range[1]);
					for (int i = start; i <= end; i++)
						Rules.Birth.Add(i);
				}
			}
		}
	}

	[ConVar.Client( Name = "automata.states", Max = 20, Min = 0 )]
	public static int States
	{
		get
		{
			return Rules.State;
		}
		set
		{
			Rules.State = value - 1;
		}
	}

	[ConVar.Client( Name = "automata.check_type", Min = 0 )]
	public static int CheckType
	{
		get
		{
			return (int) Rules.CheckType;
		}
		set
		{
			Rules.CheckType = (NeighborCheck) value;
		}
	}

	private static int _BoundsSize = 25;
	[ConVar.Client( Name = "automata.bounds_size", Min = 1 )]
	public static int BoundsSize
	{
		get
		{
			return _BoundsSize;
		}
		set
		{
			_BoundsSize = value;
			ReloadGrid();
		}
	}

	public static Shape SelectedShape = Shape.Cube;
	[ConVar.Client( Name = "automata.shape", Min = 0 )]
	public static int InitialShape
	{
		get
		{
			return (int)SelectedShape;
		}
		set
		{
			SelectedShape = (Shape)value;
		}
	}

	public static ColorMap SelectedColorMap = ColorMap.Position;
	[ConVar.Client( Name = "automata.color_map", Min = 0 )]
	public static int ColorMapping
	{
		get
		{
			return (int)SelectedColorMap;
		}
		set
		{
			SelectedColorMap = (ColorMap)value;
		}
	}

	[ConVar.Client( Name = "automata.spawn_chance", Min = 0, Max = 100 )]
	public static int SpawnChance { get; set; } = 50;

	[ConVar.Client( Name = "automata.interval", Min = 0, Max = 1 )]
	public static float Interval { get; set; } = 0.1f;

	[ConVar.Client( Name = "automata.spawn_radius", Min = 0, Max = 1 )]
	public static float SpawnRadius { get; set; } = 0.3f;
	#endregion

	public Automata()
	{
		ReloadGrid();
	}

	public override void Spawn()
	{
		base.Spawn();

		new HudEntity();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var pawn = new Pawn();
		client.Pawn = pawn;

		ReloadGrid();
	}

	[Event.Hotload]
	public static void ReloadGrid()
	{
		if ( Grid != null )
		{
			Grid.ClearGrid();
			Grid.Delete();
		}

		Grid = new CellGrid();
		Grid.Spawn();
	}

	[Event.Frame]
	public static void DrawGrid()
	{
		if ( Grid == null ) return;
		Vector3 box = new Vector3( BoundsSize, BoundsSize, BoundsSize );
		DebugOverlay.Box(Vector3.Zero, box);
	}

	[ConCmd.Client( "automata.generate" )]
	public static void Generate()
	{
		Grid.GenerateGrid();
		Grid.ReloadMesh();
	}

	[ConCmd.Client( "automata.clear" )]
	public static void Clear()
	{
		Grid.ClearGrid();
	}
}
