using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace gm1;

public partial struct PartyMember
{
	public long ActorId { get; set; }
	public int OrderIndex { get; set; }
}

public partial class Party : BaseNetworkable, IEnumerable<PartyMember>
{
	[Net]
	private List<PartyMember> Members { get; set; } = new List<PartyMember>();

	public IEnumerable<Actor> Actors => Entity.All.OfType<Actor>().Where( actor => GetMember( actor ) != null );

	public PartyMember? GetMember( Actor actor )
	{
		var members = Members.Where( data => data.ActorId == actor.ActorId );
		if ( !members.Any() )
			return null;
		return members.Single();
	}
	public PartyMember? GetMemberByOrderIndex( int index )
	{
		var members = Members.Where( data => data.OrderIndex == index );
		if ( !members.Any() )
			return null;
		return members.Single();
	}
	public static Actor GetActor( PartyMember member ) => Entity.All.OfType<Actor>().FirstOrDefault( actor => actor.ActorId == member.ActorId );

	public PartyMember? First()
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

	public PartyMember? Last()
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

	public PartyMember? Next( PartyMember? current )
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

			if ( member.OrderIndex <= current.Value.OrderIndex )
				continue;

			if ( member.OrderIndex > memberOrderIndex )
				continue;

			memberIndex = i;
			memberOrderIndex = member.OrderIndex;
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

	public PartyMember? Previous( PartyMember? current )
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

			if ( member.OrderIndex >= current.Value.OrderIndex )
				continue;

			if ( member.OrderIndex < memberOrderIndex )
				continue;

			memberIndex = i;
			memberOrderIndex = member.OrderIndex;
		}

		return (memberIndex == -1) ? null : Members[memberIndex];
	}

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

	public void Add( Actor actor, int? orderIndex = null )
	{
		if ( Host.IsClient )
		{
			Log.Warning( "Skipping Party.Add on client" );
			return;
		}

		if ( Contains( actor ) )
			throw new System.InvalidOperationException( "Can't add actor to party already containing actor" );

		// Make sure that index doesn't exist already
		if ( orderIndex != null && GetMemberByOrderIndex( orderIndex.Value ) != null )
			throw new System.InvalidOperationException( $"Order index {orderIndex} already exists in party" );

		Members.Add( new PartyMember
		{
			ActorId = actor.ActorId,
			OrderIndex = orderIndex ?? GetUnusedOrderIndex()
		} );
	}

	public void Add( IEnumerable<Actor> actors )
	{
		if ( Host.IsClient )
		{
			Log.Warning( "Skipping Party.Add on client" );
			return;
		}

		foreach ( var actor in actors )
		{
			if ( Contains( actor ) )
				throw new System.InvalidOperationException( "Can't add actor to party already containing actor" );

			Members.Add( new PartyMember
			{
				ActorId = actor.ActorId,
				OrderIndex = GetUnusedOrderIndex()
			} );
		}
	}

	public bool Contains( Actor actor ) => Members.Any( member => member.ActorId == actor.ActorId );
	public bool ContainsIndex( int orderIndex ) => Members.Any( member => member.OrderIndex == orderIndex );

	public int Count => Members.Count;

	public IEnumerator<PartyMember> GetEnumerator() => Members.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}