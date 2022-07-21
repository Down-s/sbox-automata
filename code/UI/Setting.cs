using Sandbox;
using Sandbox.Html;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace Automata;
[UseTemplate( "setting" )]
public class Setting : Panel
{
	public string ConVar { get; set; }

	[Property]
	public string Value { get; set; }

	private string LastValue;

	public Setting()
	{

	}

	public override bool OnTemplateElement( INode node )
	{
		ConVar = node.GetAttribute( "convar" );
		string type = node.GetAttribute( "type" );

		switch (type)
		{
			case "slider":
				{
					var control = Add.SliderWithEntry( node.GetAttributeFloat( "min", 1.0f ), node.GetAttributeFloat( "max", 1.0f ), node.GetAttributeFloat( "step", 1.0f ) );
					control.SetClass( "slider", true );
					control.Format = node.GetAttribute( "format", control.Format );
					control.Bind( "value", this, "Value" );
					control.AddEventListener( "value.changed", () => CreateValueEvent( "value", control.Value ) );
					control.AddEventListener( "value.changed", () => CreateEvent( "setting-changed" ) );
					break;
				}

			case "dropdown":
				{
					var control = AddChild<DropDown>();
					control.OnTemplateElement( node );
					control.Bind( "value", this, "Value" );
					control.AddEventListener( "value.changed", () => CreateValueEvent( "value", control.Selected?.Value ) );
					control.AddEventListener( "value.changed", () => CreateEvent( "setting-changed" ) );
					break;
				}

			default:
				{
					SetClass( "panel", true );
					var control = Add.TextEntry( "4" );
					control.SetClass( "entry", true );
					control.Bind( "value", this, "Value" );
					control.AddEventListener( "value.changed", () => CreateValueEvent( "value", control.Text ) );
					break;
				}

		}

		UpdateValue();
		return false;
	}

	public override void Tick()
	{
		base.Tick();
		string cvar = ConsoleSystem.GetValue( ConVar );
		if ( Value == null || cvar == null ) return;

		if ( Value != LastValue )
		{
			ApplySetting();
		}
		else if (Value != cvar)
		{
			UpdateValue();
		}

		LastValue = Value;
	}

	public void UpdateValue()
	{
		if ( ConVar == null ) return;

		var value = ConsoleSystem.GetValue( ConVar );
		if ( value == null ) return;

		Value = value;
		CreateValueEvent( "value", Value );
	}

	public void ApplySetting()
	{
		if ( ConVar != null )
		{
			Log.Info( $"Set {ConVar} to {Value}" );
			ConsoleSystem.Run( ConVar, Value );
		}
	}
}
