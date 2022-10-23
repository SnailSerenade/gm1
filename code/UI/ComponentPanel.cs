using Sandbox;
using Sandbox.UI;

namespace gm1.SharedUI;

/// <summary>
/// Panel with component related lifetime
/// </summary>
/// <typeparam name="T">Component type</typeparam>
public partial class ComponentPanel<T> : Panel where T : EntityComponent
{
	protected T Component;
	public ComponentPanel( T component ) => Component = component;
	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn == null )
			return;

		if ( Local.Pawn.Components.Get<T>() != Component )
			Delete();
	}
}

/// <summary>
/// Container for ComponentPanel that creates / removes panel based on component state
/// </summary>
/// <typeparam name="TC">Component type</typeparam>
/// <typeparam name="T1">ComponentPanel type</typeparam>
public partial class ContainedComponentPanel<TC, T1> where TC : EntityComponent where T1 : ComponentPanel<TC>
{
	protected T1 ComponentPanel;
	protected TC Component;

	public ContainedComponentPanel( TC component ) => Component = component;

	public void Tick()
	{
		if ( Local.Pawn == null )
			return;

		var current = Local.Pawn.Components.Get<TC>();
		if ( current == Component && ComponentPanel == null )
		{
			ComponentPanel = TypeLibrary.Create<T1>( typeof( T1 ), new object[] { Component } );
			Local.Hud.AddChild( ComponentPanel );
		}
	}
}