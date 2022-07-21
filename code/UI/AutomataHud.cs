using Sandbox;
using Sandbox.UI;

namespace Automata;
public partial class HudEntity : Sandbox.HudEntity<RootPanel>
{
	public static HudEntity Instance; // Ugh
	public HudEntity()
	{
		if ( IsClient )
		{
			RootPanel.SetTemplate( "/UI/AutomataHud.html" );
			Instance = this;
		}
	}

	[Event.Tick.Client]
	public void CheckCapture()
	{
		bool Pressed = Input.Down( InputButton.Jump );
		RootPanel.SetClass( "mouseup", !Pressed );
		RootPanel.SetClass( "mousedown", Pressed );
	}

	[ConCmd.Client( "automata.simulate_toggle" )]
	public static void ToggleSimulation()
	{
		CellGrid Grid = Automata.Grid;
		Grid.Simulating = !Grid.Simulating;
		Instance.RootPanel.GetChild(1).SetClass("playing", Grid.Simulating);
	}

	public void ChangeRules(string rules)
	{
		Log.Info( rules );
	}
}
