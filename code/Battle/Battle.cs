using Sandbox;

namespace gm1.BattleSys;

public partial class BattleMember : EntityComponent
{
	[Net] public Battle Battle { get; set; }
}

public partial class BattleActor : EntityComponent
{
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
	[Net] public BattleSys.Action Action { get; set; } = null;
	[Net] public Entity Target { get; set; } = null;
	public bool Selected => Action != null && Target != null;
}

public partial class Battle : Entity
{
	[Net] public Party PartyOne { get; set; }
	[Net] public Party PartyTwo { get; set; }

	[Net] public Party CurrentParty { get; protected set; } = null;
	[Net] public PartyMember CurrentActor { get; protected set; } = null;

	[Net] public BattleArea BattleArea { get; protected set; }

	public Battle( BattleArea battleArea = null, PartyMember advantage = null, Party partyOne = null, Party partyTwo = null )
	{
		PartyOne = partyOne;
		PartyTwo = partyTwo;
		CurrentActor = advantage;
		BattleArea = battleArea;
	}
	public Battle() { }

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
		}

		// Prep party two
		foreach ( var member in partyTwoMembers )
		{
			var component = member.Entity.Components.Create<BattleMember>();
			component.Battle = this;
		}

		// Move parties to their areas
		MovePartyToArea( PartyOne, BattleArea.PartyOneArea );
		MovePartyToArea( PartyTwo, BattleArea.PartyTwoArea );

		// Set first actor / party
		CurrentParty = PartyOne;
		CurrentActor ??= CurrentParty.First();
	}

	protected static void MovePartyToArea( Party party, PartySpot location )
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
}