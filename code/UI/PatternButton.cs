using Sandbox;
using Sandbox.Html;
using Sandbox.UI;

namespace Automata;
[UseTemplate( "patternbutton" )]
public partial class PatternButton : Panel
{
	private string SurviveRequirement;
	private string SpawnRequirement;
	private string States;
	private string CheckType;
	private string SpawnChance;
	private string Shape;

	public PatternButton()
	{
		Log.Info( "Created pattern button" );
	}

	public override bool OnTemplateElement( INode node )
	{
		// Kinda messy but whatever
		SurviveRequirement = node.GetAttribute( "survive" );
		SpawnRequirement = node.GetAttribute( "spawn" );
		States = node.GetAttribute( "states" );
		CheckType = node.GetAttribute( "checktype" );
		SpawnChance = node.GetAttribute( "spawnchance" );
		Shape = node.GetAttribute( "shape" );

		if ( string.IsNullOrEmpty( SpawnChance ) )
			SpawnChance = "50";
		if ( string.IsNullOrEmpty( Shape ) )
			Shape = "0";

		return false;
	}

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		Log.Info( "Clicked" );
		ConsoleSystem.Run( "automata.survive", SurviveRequirement );
		ConsoleSystem.Run( "automata.birth", SpawnRequirement );
		ConsoleSystem.Run( "automata.states", States );
		ConsoleSystem.Run( "automata.check_type", CheckType );
		ConsoleSystem.Run( "automata.spawn_chance", SpawnChance );
		ConsoleSystem.Run( "automata.shape", Shape );
		Automata.SpawnChance = int.Parse( SpawnChance );
		Automata.InitialShape = int.Parse(Shape);
	}
}
