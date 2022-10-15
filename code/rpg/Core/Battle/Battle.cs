using System.Collections.Generic;

namespace gm1;

public partial class Battle : Sandbox.Entity
{
	[Sandbox.Net]
	public Party PartyOne { get; set; }

	[Sandbox.Net]
	public Party PartyTwo { get; set; }

	[Sandbox.Net]
	public Actor Initiator { get; private set; }

	// Current battle states
	[Sandbox.Net]
	public PartyMember? CurrentPartyMember { get; private set; }

	[Sandbox.Net]
	public Party CurrentParty { get; private set; }
	public Party InactiveParty => (CurrentParty != PartyOne) ? PartyOne : PartyTwo;

	/// <summary>
	/// Battle area configuration
	/// </summary>
	[Sandbox.Net]
	public Entities.BattleAreaConfig BattleArea { get; private set; }

	public Battle( Actor initiator, Party partyOne = null, Party partyTwo = null )
	{
		BattleArea = Entities.BattleAreaConfig.Random;
		Initiator = initiator;
		PartyOne = partyOne ?? null;
		PartyTwo = partyTwo ?? null;
	}

	public Battle() { }

	protected void MovePartyToArea( Party party, Entities.BattleAreaParty location )
	{
		PartyMember? current = location.InvertOrder ? party.Last() : party.First();
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

			var actor = Party.GetActor( current.Value );
			actor.Position = location.Position + (direction * location.ActorSpacing * i);

			current = location.InvertOrder ? party.Previous( current ) : party.Next( current );
		}
	}

	public void Start()
	{
		if ( BattleArea == null )
		{
			BattleArea = Entities.BattleAreaConfig.Random;
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

		// Prep party one
		foreach ( var player in PartyOne.Actors )
		{
			player.Battle = this;
		}

		// Prep party two
		foreach ( var player in PartyTwo.Actors )
		{
			player.Battle = this;
		}

		// Move parties to their areas
		MovePartyToArea( PartyOne, BattleArea.PartyOneArea );
		MovePartyToArea( PartyTwo, BattleArea.PartyTwoArea );

		// Set first actor / party
		CurrentParty = PartyOne;
		CurrentPartyMember = CurrentParty.First();
	}

	public void MoveOn()
	{
		// Move to next party member in current party
		CurrentPartyMember = CurrentParty.Next( CurrentPartyMember );

		// If that party member doesn't exist...
		if ( CurrentPartyMember == null )
		{
			CurrentParty = InactiveParty; // switch current party,
			CurrentPartyMember = CurrentParty.First(); // and use the first member in that.
		}
	}

	public void End()
	{
		foreach ( var player in PartyOne.Actors )
		{
			player.Battle = null;
		}

		foreach ( var player in PartyTwo.Actors )
		{
			player.Battle = null;
		}

		Delete();
	}
}