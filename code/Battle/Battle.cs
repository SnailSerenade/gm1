using gm1.Battle.Area;
using gm1.Core;
using Sandbox;

namespace gm1.Battle;

public partial class Battle : Entity
{
	[Net] public Party PartyOne { get; set; }
	[Net] public Party PartyTwo { get; set; }

	[Net] public Party CurrentParty { get; protected set; }
	public Party InactiveParty => (CurrentParty != PartyOne) ? PartyOne : PartyTwo;
	[Net] public PartyMember CurrentActor { get; protected set; }

	[Net] public BattleArea BattleArea { get; protected set; }

	public Battle( BattleArea battleArea = null, PartyMember advantage = null, Party partyOne = null,
		Party partyTwo = null )
	{
		Event.Register( this );
		Transmit = TransmitType.Always;
		PartyOne = partyOne;
		PartyTwo = partyTwo;
		CurrentActor = advantage;
		BattleArea = battleArea;
	}

	public Battle() { Event.Register( this ); }
	~Battle() { Event.Unregister( this ); }

	public void Start()
	{
		if ( BattleArea == null )
		{
			BattleArea = BattleArea.Random;
			if ( BattleArea == null )
				throw new System.Exception( "No battle area found on map" );
		}

		if ( PartyOne == null )
			throw new System.Exception( "Party One is null" );

		if ( PartyTwo == null )
			throw new System.Exception( "Party Two is null" );

		// Make sure player count is correct
		if ( PartyOne.Count == 0 )
			throw new System.Exception( $"No players in Party One" );

		if ( PartyTwo.Count == 0 )
			throw new System.Exception( $"No players in Party Two" );

		if ( PartyOne.Count > BattleArea.PartyOneMax )
			throw new System.Exception( $"Too many players in Party One ({PartyOne.Count}>{BattleArea.PartyOneMax})" );

		if ( PartyTwo.Count > BattleArea.PartyTwoMax )
			throw new System.Exception( $"Too many players in Party Two ({PartyTwo.Count}>{BattleArea.PartyTwoMax})" );

		var partyOneMembers = PartyOne.Members;
		var partyTwoMembers = PartyTwo.Members;

		// Validate party one
		foreach ( var member in partyOneMembers )
			if ( member.Entity.Components.Get<BattleMember>() != null )
				throw new System.Exception( $"Player {member.Entity} already in a battle" );

		// Validate party two
		foreach ( var member in partyTwoMembers )
			if ( member.Entity.Components.Get<BattleMember>() != null )
				throw new System.Exception( $"Player {member.Entity} already in a battle" );

		// Prep party two
		foreach ( var member in partyOneMembers )
		{
			var component = member.Entity.Components.Create<BattleMember>();
			component.Battle = this;
			component.Enemies = PartyTwo;
		}

		// Prep party two
		foreach ( var member in partyTwoMembers )
		{
			var component = member.Entity.Components.Create<BattleMember>();
			component.Battle = this;
			component.Enemies = PartyOne;
		}

		// Move parties to their areas
		MovePartyToArea( PartyOne, BattleArea.PartyOneArea );
		MovePartyToArea( PartyTwo, BattleArea.PartyTwoArea );

		// Set first actor / party
		if ( CurrentActor != null )
		{
			CurrentParty = !PartyOne.Contains( CurrentActor ) ? PartyOne : PartyTwo;
		}
		else
		{
			CurrentParty = PartyOne;
		}

		CurrentActor ??= CurrentParty.First();

		BeginPartyTurn( CurrentParty );
	}

	private static void MovePartyToArea( Party party, PartySpot location )
	{
		PartyMember current = location.InvertOrder ? party.Last() : party.First();
		if ( current == null )
			throw new System.Exception( "Couldn't get first member of party!" );

		var direction = location.InvertDirection ? -location.Rotation.Forward : location.Rotation.Forward;

		for ( int i = 0; i < party.Count; i++ )
		{
			if ( current == null )
			{
				Log.Warning( "Failed to get member while order iterating over party" );
				continue;
			}

			var actor = current.Entity;
			actor.Position = location.Position + (direction * location.ActorSpacing * i);

			current = location.InvertOrder ? party.Previous( current ) : party.Next( current );
		}
	}

	private void DrawPartyInfo( Party party, Vector2 position, Color color )
	{
		var offset = 0;
		foreach ( var member in party )
		{
			DebugOverlay.ScreenText( $"{member.Entity.Name} Order:{member.OrderIndex}", position, offset++, color );

			DebugOverlay.ScreenText( $"  *  Health {member.Entity.Health}", position, offset++, color );

			if ( member == CurrentActor )
				DebugOverlay.ScreenText( $"  *  CurrentActor", position, offset++, color );

			var battleMember = member.Entity.Components.Get<BattleMember>();
			if ( battleMember != null )
				DebugOverlay.ScreenText( $"  *  BattleMember", position, offset++, color );

			var battleActor = member.Entity.Components.Get<BattleActor>();
			if ( battleActor != null )
				DebugOverlay.ScreenText(
					$"  -> BattleActor:{battleActor.Battle} Action:{battleActor.Action} Target:{battleActor.Target} Selected:{battleActor.Selected}",
					position, offset++, color );
		}
	}

	/// <summary>
	/// Start turn for party
	/// </summary>
	private void BeginPartyTurn( Party party )
	{
		CurrentParty = party;

		foreach ( var member in CurrentParty )
		{
			member.Entity.Components.RemoveAny<BattleActor>();
			member.Entity.Components.Create<BattleActor>();
		}
	}

	/// <summary>
	/// Start turn for party
	/// </summary>
	private void FinalizePartyTurn()
	{
		foreach ( var member in CurrentParty )
		{
			member.Entity.Components.RemoveAny<BattleActor>();
		}
	}

	/// <summary>
	/// Update state based on current information
	/// Should be called by other components after they finish
	/// </summary>
	public void Update()
	{
		// Check if everyone on the current party has picked a target...
		var allPlayersLockedIn = true;
		foreach ( var member in CurrentParty )
		{
			if ( !allPlayersLockedIn )
				continue;

			var battleActor = member.Entity.Components.Get<BattleActor>();

			if ( battleActor == null )
			{
				allPlayersLockedIn = false;
				continue;
			}

			if ( !battleActor.LockedIn || battleActor.Target == null || battleActor.Action == null )
				allPlayersLockedIn = false;
		}

		if ( allPlayersLockedIn )
		{
			Log.Info( $"All players in {CurrentParty} have picked a target" );
			// run attacks
			foreach ( var member in CurrentParty )
			{
				var battleActor = member.Entity.Components.Get<BattleActor>();

				var preHealth = battleActor.Target.Character.Health;
				battleActor.Action.Perform( battleActor, battleActor.Target.Character );
				Log.Info(
					$"{member.Character.Name} used {battleActor.Action.Name} on {battleActor.Target.Character.Name} for {preHealth - battleActor.Target.Character.Health}hp" );
			}

			FinalizePartyTurn();

			BeginPartyTurn( InactiveParty );
		}
	}

	[Event.Tick]
	public void Tick()
	{
		if ( CurrentActor == null )
			return;

		DrawPartyInfo( PartyOne, Vector2.One * 20, Color.Cyan );

		DrawPartyInfo( PartyTwo, new Vector2( 250, 20 ), Color.Yellow );
	}
}
