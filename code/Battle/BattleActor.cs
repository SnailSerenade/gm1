using Sandbox;

namespace gm1.BattleSys;

public partial class BattleActor : EntityComponent
{
	UI.BattleActorUi Panel;

	UI.BattleSelectorOverlay SelectorOverlay;

	public Battle Battle
	{
		get
		{
			var component = Entity.Components.Get<BattleMember>();
			if ( component == null )
				return null;
			return component.Battle;
		}
	}
	[Net] public bool LockedIn { get; set; } = false;
	[Net] public Action Action { get; set; } = null;
	[Net, Predicted] private PartyMember InternalTarget { get; set; } = null;
	public PartyMember Target
	{
		get => InternalTarget;
		set
		{
			InternalTarget = value;
			if ( Host.IsClient )
				ClientTargetToServer( value.NetworkIdent );
		}
	}
	[ConCmd.Server]
	public static void ClientTargetToServer( int entityNetworkId )
	{
		PartyMember partyMember = null;

		foreach ( var component in EntityComponent.GetAllOfType<PartyMember>() )
		{
			if ( component.NetworkIdent == entityNetworkId )
				partyMember = component;
		}

		if ( partyMember == null )
		{
			Log.Error( $"Couldn't find target with network ident {entityNetworkId}" );
			return;
		}

		var battleActor = ConsoleSystem.Caller.Pawn.Components.Get<BattleActor>();
		if ( battleActor == null )
			return;

		if ( battleActor.LockedIn )
			return;

		if ( battleActor.Action != null && !battleActor.Action.CheckTarget( partyMember.Character ) )
		{
			Log.Error( "Client requested invalid target" );
			battleActor.Target = battleActor.Target;
			return;
		}

		battleActor.Target = partyMember;
	}
	[ConCmd.Server]
	public static void AttemptLockIn()
	{
		var battleActor = ConsoleSystem.Caller.Pawn.Components.Get<BattleActor>();
		if ( battleActor == null )
			return;

		battleActor.Action = new Abilities.Punch( battleActor );

		if ( battleActor.Action == null )
		{
			Log.Error( "Client requested lock in without action" );
			return;
		}

		if ( battleActor.Target == null )
		{
			Log.Error( "Client requested lock in without target" );
			return;
		}

		battleActor.LockedIn = true;
		battleActor.Battle.Update();
	}

	[Net] public Party Enemies { get; set; } = null;
	public bool Selected => Action != null && Target != null;

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsClient && Entity == Local.Pawn )
		{
			Panel = new();
			SelectorOverlay = new();

			Local.Hud.AddChild( Panel );
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient && Panel != null )
		{
			Panel.Delete();
			SelectorOverlay.Delete();
		}
	}

	[Event.BuildInput]
	public void BuildInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Jump ) )
		{
			var newTarget = Target != null ? Enemies.Next( Target ) : Enemies.First();
			if ( newTarget == null )
				newTarget = Enemies.First();
			Target = newTarget;
		}

		if ( Target != null && input.Pressed( InputButton.Chat ) )
		{
			AttemptLockIn();
		}
	}

	[Event.Tick]
	public void Tick()
	{
		if ( Target == null && Enemies != null && Host.IsServer )
			Target = Enemies.First();

		if ( Host.IsServer )
			return;

		if ( Target == null || Target.Entity == null )
			return;

		if ( Entity == null )
			return;

		//if ( SelectorOverlay.Transform != null && Target.Entity.Transform != null )
		//SelectorOverlay.Transform = Target.Entity.Transform.WithRotation( Rotation.LookAt( SelectorOverlay.Transform.Position - Local.Pawn. ) );
		DebugOverlay.Text( "TARGET", Target.Entity.Position, 0, Color.Cyan );
		DebugOverlay.Text( $"(of {Entity.Name})", Target.Entity.Position, 1, Color.Cyan );
		Target.Entity.Components.Create<Sandbox.Component.Glow>();
	}
}