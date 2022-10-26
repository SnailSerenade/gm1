using Sandbox;
using Sandbox.UI;

namespace gm1.UI;

/// <summary>
/// Interface for element with component related lifetime
/// </summary>
/// <typeparam name="T">Component type</typeparam>
public partial interface IComponentPanelType<out T> where T : EntityComponent
{
	public T Component { get; }
}

/// <summary>
/// Panel with component related lifetime
/// </summary>
/// <typeparam name="T">Component type</typeparam>
public partial class ComponentPanel<T> : Panel, IComponentPanelType<T> where T : EntityComponent
{
	public T Component { get; set; }
	public ComponentPanel( T component ) => Component = component;
	public override void Tick()
	{
		base.Tick();
		if ( Local.Pawn == null && Local.Pawn.IsValid )
			return;
		if ( Local.Pawn.Components.Get<T>() != Component )
			Delete();
	}
}

/// <summary>
/// WorldPanel with component related lifetime
/// </summary>
/// <typeparam name="T">Component type</typeparam>
public partial class ComponentWorldPanel<T> : WorldPanel, IComponentPanelType<T> where T : EntityComponent
{
	public T Component { get; set; }
	public ComponentWorldPanel( T component ) => Component = component;
	public override void Tick()
	{
		base.Tick();
		if ( Local.Pawn == null && Local.Pawn.IsValid )
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
public partial class ContainedComponentPanel<TC, T1> where TC : EntityComponent where T1 : Panel, IComponentPanelType<TC>
{
	private T1 _componentPanel;
	private readonly TC _component;

	public ContainedComponentPanel( TC component ) => _component = component;

	public void Tick()
	{
		if ( Local.Pawn == null )
			return;

		var current = Local.Pawn.Components.Get<TC>();
		if ( current == _component && _componentPanel == null )
		{
			_componentPanel = TypeLibrary.Create<T1>( typeof( T1 ), new object[] { _component } );
			if ( _componentPanel is not RootPanel )
				Local.Hud.AddChild( _componentPanel );
		}
	}
}
