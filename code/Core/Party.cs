using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace gm1;

public partial class PartyMember : EntityComponent
{
	public Character Character => Entity as Character;
	[Net] public Party Party { get; set; }
	[Net] public int OrderIndex { get; set; }
}

public partial class Party : Entity, IEnumerable<PartyMember>
{
	public List<PartyMember> Members => EntityComponent.GetAllOfType<PartyMember>().Where( member => member.Party == this ).ToList();

	public Party() => Transmit = TransmitType.Always;

	public void Add( Character entity, int? orderIndex = null )
	{
		if ( Host.IsClient )
		{
			Log.Warning( "Skipping Party.Add on client" );
			return;
		}

		// Make sure entity isn't in a party
		if ( entity.Components.Get<PartyMember>() != null )
			throw new System.InvalidOperationException( "Entity is already in a party" );

		// Make sure that index doesn't exist already
		if ( orderIndex != null && ContainsIndex( orderIndex.Value ) )
			throw new System.InvalidOperationException( $"Order index {orderIndex} already exists in party" );

		// Create component
		var component = new PartyMember
		{
			Party = this,
			OrderIndex = orderIndex ?? GetUnusedOrderIndex()
		};

		// Add to entity
		entity.Components.Add( component );
	}

	public void Add( IEnumerable<Character> entities )
	{
		foreach ( var entity in entities )
			Add( entity );
	}

	public PartyMember Next( PartyMember current )
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

			memberIndex = i;
			memberOrderIndex = member.OrderIndex;
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public PartyMember Previous( PartyMember current )
	{
		if ( current == null )
			return null;

		// we want to get the highest index below the provided member
		// if none found then return null
		int memberOrderIndex = int.MinValue;
		int memberIndex = -1;
		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];

			if ( member.OrderIndex >= current.OrderIndex )
				continue;

			if ( member.OrderIndex < memberOrderIndex )
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
	protected int GetUnusedOrderIndex()
	{
		int highest = -1;
		foreach ( var member in Members )
		{
			if ( member.OrderIndex > highest )
				highest = member.OrderIndex;
		}
		return highest + 1;
	}

	public PartyMember First()
	{
		int memberOrderIndex = int.MaxValue;
		int memberIndex = -1;

		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];
			if ( member.OrderIndex < memberOrderIndex )
			{
				memberOrderIndex = member.OrderIndex;
				memberIndex = i;
			}
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public PartyMember Last()
	{
		int memberOrderIndex = int.MinValue;
		int memberIndex = -1;

		for ( int i = 0; i < Members.Count; i++ )
		{
			PartyMember member = Members[i];
			if ( member.OrderIndex > memberOrderIndex )
			{
				memberOrderIndex = member.OrderIndex;
				memberIndex = i;
			}
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public int Count => Members.Count;
	public bool Contains( Entity entity ) => Members.Where( member => member.Entity == entity ).Any();
	public bool ContainsIndex( int orderIndex ) => Members.Where( member => member.OrderIndex == orderIndex ).Any();

	public IEnumerator<PartyMember> GetEnumerator() => Members.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}