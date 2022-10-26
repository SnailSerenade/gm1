using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;
using gm1.UI;

namespace gm1.Battle.UI;

[UseTemplate]
public partial class BattleActorActionPicker : ComponentPanel<BattleActor>
{
	public BattleActorActionPicker( BattleActor component ) : base( component )
	{
		Overlay = new();
		AddChild( Overlay );
	}

	public override bool HasContent => true;
	public Panel CardContainer { get; set; }
	public SceneObjectOverlay Overlay { get; set; }


	public override void Delete( bool immediate = false )
	{
		base.Delete( immediate );

		Overlay.Delete( immediate );
	}

	[Event.BuildInput]
	public void BuildInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Jump ) )
		{
			var newTarget = Component.Target != null
				? Component.Enemies.Next( Component.Target, true )
				: Component.Enemies.First( true );
			if ( newTarget == null )
				newTarget = Component.Enemies.First( true );
			Component.Target = newTarget;
			Component.Character.EnableDrawing = false;
			Overlay.ExistingSceneObject = Component.Character.SceneObject;
			//Log.Info( Overlay.SceneObject.MeshGroupMask );
			Log.Info( Component.Character.SceneObject.MeshGroupMask );
		}

		if ( Component.Target != null && input.Pressed( InputButton.Chat ) )
		{
			Component.Action = (Component.Entity as Core.Character).Actions[0];
			Component.AttemptLockIn();
		}

		var screen = Component.Entity.EyePosition.ToScreen();
		if ( screen.z > 0 )
		{
			var halfWidth = CardContainer.ComputedStyle.Width.Value.Value / 2;
			var halfHeight = CardContainer.ComputedStyle.Height.Value.Value / 2;

			CardContainer.Style.Left = Length.Percent( (screen.x * 100) - halfWidth );
			CardContainer.Style.Top = Length.Percent( (screen.y * 100) - halfHeight );
		}

		if ( input.Pressed( InputButton.Forward ) )
		{
			Card newCard = CardContainer.AddChild<Card>();
			newCard.Add.Label( "" );

			int i = 0;
			float delta = 30;
			foreach ( var card in CardContainer.Children )
			{
				i++;
				var transform = new PanelTransform();
				Log.Info( (360 / CardContainer.ChildrenCount) * i );
				transform.AddRotation( 0, 0, delta * i );
				transform.AddRotation( 0, 0, -((delta / 2) * CardContainer.ChildrenCount) );
				transform.AddTranslate( 50 * i + 0, 50 * i + 0, 0 );
				card.Style.Transform = transform;
			}
		}
	}
}
