using gm1.Battle.UI;
using gm1.Core;
using Sandbox;

namespace gm1.Battle;

public partial class BattleActor
{
	[Net] public bool LockedIn { get; private set; }
	[Net, Predicted] private string InternalAction { get; set; }

	public Action Action
	{
		get => Action.Get( InternalAction );
		set
		{
			InternalAction = value.Name;
			if ( Host.IsClient )
				ClientActionToServer( InternalAction );
		}
	}

	[ConCmd.Server]
	private static void ClientActionToServer( string action )
	{
		var battleActor = ConsoleSystem.Caller.Pawn.Components.Get<BattleActor>();
		if ( battleActor == null )
			return;

		if ( battleActor.LockedIn )
			return;

		if ( battleActor.Character.ActionNames.Contains( action ) )
			battleActor.InternalAction = action;
	}

	[Net, Predicted] private PartyMember InternalTarget { get; set; }

	public PartyMember Target
	{
		get => InternalTarget;
		set
		{
			InternalTarget = value;
			if ( Host.IsClient && value != null )
				ClientTargetToServer( value.Entity.NetworkIdent );
		}
	}

	[ConCmd.Server]
	private static void ClientTargetToServer( int entityNetworkId )
	{
		PartyMember partyMember = null;

		foreach ( var component in GetAllOfType<PartyMember>() )
		{
			if ( component.Entity != null && component.Entity.NetworkIdent == entityNetworkId )
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

		Log.Info( $"new target {partyMember}" );
		battleActor.Target = partyMember;
	}

	[ConCmd.Server]
	private static void ClientAttemptLockInToServer()
	{
		var battleActor = ConsoleSystem.Caller.Pawn.Components.Get<BattleActor>();

		battleActor?.AttemptLockIn();
	}

	/// <summary>
	/// Attempt to lock in current Action / Target selection.
	/// </summary>
	public void AttemptLockIn()
	{
		if ( Host.IsClient )
		{
			ClientAttemptLockInToServer();
			return;
		}

		if ( Action == null )
		{
			Log.Error( "Client requested lock in without action" );
			return;
		}

		if ( Target == null )
		{
			Log.Error( "Client requested lock in without target" );
			return;
		}

		LockedIn = true;
		Battle.Update();
	}

	public bool Selected => Action != null && Target != null;
}
