using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace gm1;

public abstract partial class Actor : AnimatedEntity
{
	public Actor()
	{
		Health = MaxHealth;
		ActorId = Random.Shared.NextInt64();
	}

	[Net]
	public long ActorId { get; private set; }
	public static Actor FromActorId( long actorId ) => All.OfType<Actor>().FirstOrDefault( actor => actor.ActorId == actorId );

	[Net, Change]
	private Battle InternalBattle { get; set; } = null;
	void OnInternalBattleChanged( Battle o, Battle n )
	{
		if ( n != null )
			OnBattleEntered( n );
		else
			OnBattleLeft();
	}

	public Battle Battle
	{
		get => InternalBattle;
		set
		{
			if ( value == null )
			{
				if ( InternalBattle != null )
					OnBattleLeft();
				InternalBattle = null;
				return;
			}

			if ( InternalBattle != null )
			{
				Log.Warning( "Actor is already in battle!" );
				return;
			}

			InternalBattle = value;
			OnBattleEntered( InternalBattle );
		}
	}

	public virtual float MaxHealth { get; set; } = 100.0f;

	public virtual void InteractWith( Element element )
	{
		// Update states
		var current = GetElementOrNull( element.GetType() );
		if ( current != null )
		{
			current.UpdateState( element );
			if ( element.State != ElementState.NONE )
				HandleNewState( current );
		}
		else
		{
			SetElement( element );
			HandleNewState( current );
		}
	}

	/// <summary>
	/// Called when actor joins a battle
	/// </summary>
	/// <param name="battle">Battle</param>
	public virtual void OnBattleEntered( Battle battle ) { }

	/// <summary>
	/// Called when actor leaves a battle
	/// </summary>
	public virtual void OnBattleLeft() { }

	/// <summary>
	/// Take damage (float, non percentage)
	/// </summary>
	/// <param name="damage">Damage as float</param>
	public virtual void TakeDamage( float damage ) => Health -= damage;

	/// <summary>
	/// Take damage (float 0-1)
	/// </summary>
	/// <param name="percentage">Damage as float percentage (0-1)</param>
	public void TakeDamageAsPercentage( float percentage ) => TakeDamage( MaxHealth * percentage );

	/// <summary>
	/// Handle new element state
	/// </summary>
	/// <param name="element">Element with changed state</param>
	public virtual void HandleNewState( Element element ) { }

	[Net]
	public List<Element> Elements { get; private set; } = new List<Element>();

	/// <summary>
	/// Find element by type
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <returns>Element</returns>
	/// <exception cref="KeyNotFoundException">When no element found</exception>
	public T GetElement<T>() where T : Element
	{
		T element = GetElementOrNull<T>();
		if ( element == null )
			throw new KeyNotFoundException( $"Element of type {typeof( T ).Name} not found." );
		return element;
	}

	/// <summary>
	/// Find element by type
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <returns>Element (or null)</returns>
	public T GetElementOrNull<T>() where T : Element
	{
		foreach ( var element in Elements )
		{
			if ( element.GetType() == typeof( T ) )
				return element as T;
		}
		return null;
	}

	/// <summary>
	/// Find element by type
	/// </summary>
	/// <param name="type">Type</param>
	/// <returns>Element</returns>
	/// <exception cref="KeyNotFoundException">When no element found</exception>
	public Element GetElement( System.Type type )
	{
		Element element = GetElementOrNull( type );
		if ( element == null )
			throw new KeyNotFoundException( $"Element of type {type.Name} not found." );
		return element;
	}

	/// <summary>
	/// Find element by type
	/// </summary>
	/// <param name="type">Type</param>
	/// <returns>Element (or null)</returns>
	public Element GetElementOrNull( System.Type type )
	{
		foreach ( var element in Elements )
		{
			if ( element.GetType() == type )
				return element;
		}
		return null;
	}

	/// <summary>
	/// Set element (replace existing elements of same type)
	/// </summary>
	/// <param name="element">Element</param>
	public void SetElement( Element element )
	{
		for ( int i = 0; i < Elements.Count; i++ )
		{
			if ( Elements[i].GetType() == element.GetType() )
			{
				Elements[i] = element;
				return;
			}
		}
		Elements.Add( element );
	}
}