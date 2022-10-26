using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace gm1.Core;

/// <summary>
/// Component to show that the <see cref="Character"/> parent is currently a member of a <see cref="Party"/>.
/// </summary>
public class PartyMember : CharacterComponent
{
	[Net] public Party Party { get; init; }
	[Net] public int OrderIndex { get; init; }
}

/// <summary>
/// Container keeping track of <see cref="PartyMember"/> components.
/// </summary>
public class Party : Entity, IEnumerable<PartyMember>
{
	public List<PartyMember> Members =>
		CharacterComponent.GetAllOfType<PartyMember>().Where( member => member.Party == this ).ToList();

	public Party() => Transmit = TransmitType.Always;

	/// <summary>
	/// Add provided <see cref="Character"/> to the Party as a <see cref="PartyMember"/>.
	/// </summary>
	/// <param name="character">Character to add to party</param>
	/// <param name="orderIndex"><see cref="PartyMember.OrderIndex"/> for new member</param>
	/// <exception cref="InvalidOperationException">Thrown when character can't be added</exception>
	public void Add( Character character, int? orderIndex = null )
	{
		if ( Host.IsClient )
		{
			Log.Warning( "Skipping Party.Add on client" );
			return;
		}

		// Make sure entity isn't in a party
		if ( character.Components.Get<PartyMember>() != null )
			throw new InvalidOperationException( "Character is already in a party" );

		// Make sure that index doesn't exist already
		if ( orderIndex != null && ContainsIndex( orderIndex.Value ) )
			throw new InvalidOperationException( $"Order index {orderIndex} already exists in party" );

		// Create component
		var component = new PartyMember { Party = this, OrderIndex = orderIndex ?? GetUnusedOrderIndex() };

		// Add to entity
		character.Components.Add( component );
	}

	/// <summary>
	/// Add multiple <see cref="Character"/> entities to the Party.
	/// </summary>
	/// <param name="characters">Characters to add to party</param>
	public void Add( IEnumerable<Character> characters )
	{
		foreach ( var character in characters )
			Add( character );
	}

	/// <summary>
	/// Get member after provided member by order index
	/// </summary>
	/// <param name="current">Member to compare to</param>
	/// <param name="aliveOnly">Should dead players be skipped?</param>
	/// <returns>Next member (or null for no members after)</returns>
	public PartyMember Next( PartyMember current, bool aliveOnly = false )
	{
		if ( current == null )
			return null;

		// we want to get the lowest index ABOVE the provided member
		// if none found then return null
		int memberOrderIndex = int.MaxValue;
		int memberIndex = -1;
		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];

			if ( member.OrderIndex <= current.OrderIndex )
				continue;

			if ( member.OrderIndex > memberOrderIndex )
				continue;

			if ( aliveOnly && member.Entity != null && member.Entity.Health <= 0 )
				continue;

			memberIndex = i;
			memberOrderIndex = member.OrderIndex;
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	/// <summary>
	/// Get member before provided member by order index
	/// </summary>
	/// <param name="current">Member to compare to</param>
	/// <param name="aliveOnly">Should dead players be skipped?</param>
	/// <returns>Previous member (or null for no members before)</returns>
	public PartyMember Previous( PartyMember current, bool aliveOnly = false )
	{
		if ( current == null )
			return null;

		// we want to get the highest index below the provided member
		// if none found then return null
		var memberOrderIndex = int.MinValue;
		int memberIndex = -1;
		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];

			if ( member.OrderIndex >= current.OrderIndex )
				continue;

			if ( member.OrderIndex < memberOrderIndex )
				continue;

			if ( aliveOnly && member.Entity is { Health: <= 0 } )
				continue;

			memberIndex = i;
			memberOrderIndex = member.OrderIndex;
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	/// <summary>
	/// Returns unused order index (1 above the current max order index)
	/// </summary>
	/// <returns>Order index</returns>
	private int GetUnusedOrderIndex() => Members.Select( member => member.OrderIndex ).Prepend( -1 ).Max() + 1;

	public PartyMember First( bool aliveOnly = false )
	{
		int memberOrderIndex = int.MaxValue;
		int memberIndex = -1;

		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];

			if ( aliveOnly && member.Entity is { Health: <= 0 } )
				continue;

			if ( member.OrderIndex < memberOrderIndex )
			{
				memberOrderIndex = member.OrderIndex;
				memberIndex = i;
			}
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public PartyMember Last( bool aliveOnly = false )
	{
		int memberOrderIndex = int.MinValue;
		int memberIndex = -1;

		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];

			if ( aliveOnly && member.Entity != null && member.Entity.Health <= 0 )
				continue;

			if ( member.OrderIndex > memberOrderIndex )
			{
				memberOrderIndex = member.OrderIndex;
				memberIndex = i;
			}
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public int Count => Members.Count;
	public bool Contains( PartyMember member ) => Members.Contains( member );
	public bool Contains( Character character ) => Members.Any( member => member.Entity == character );
	public bool ContainsIndex( int orderIndex ) => Members.Any( member => member.OrderIndex == orderIndex );

	public IEnumerator<PartyMember> GetEnumerator() => Members.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
