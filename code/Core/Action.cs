using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using gm1.Battle;

namespace gm1.Core;

public class Action
{
	public virtual string Name => GetType().Name;
	public virtual string DisplayName => Name;

	/// <summary>
	/// Check if provided character is allowed to be targeted by this action
	/// </summary>
	/// <param name="target">Entity</param>
	/// <returns>True if provided character can be targeted</returns>
	public virtual bool CheckTarget( Core.Character target ) { return false; }

	public virtual void Perform( BattleActor actor, Core.Character target )
	{
		if ( !CheckTarget( target ) )
			throw new Exception( "Action can't be performed on provided target" );
	}

	private static readonly List<Action> KnownActions = new();

	public static Action Get<T>() => KnownActions.FirstOrDefault( action => action.GetType() == typeof(T) );
	public static Action Get( string name ) => KnownActions.FirstOrDefault( action => action.Name == name );
	public static ReadOnlyCollection<Action> GetAll() => KnownActions.AsReadOnly();
	
	public static void LoadKnownActions()
	{
		if ( !Sandbox.Host.IsServer && !Sandbox.Host.IsClient )
			return; // ???
		foreach ( var description in TypeLibrary.GetDescriptions() )
		{
			if ( description.GetAttribute<IncludeActionAttribute>() == null )
			{
				continue;
			}

			var action = description.Create<Action>();
			if ( action != null )
				KnownActions.Add( action );
		}
	}
}

public class Ability : Action
{
	public virtual float DamageBase => 1.0f;

	public override bool CheckTarget( Core.Character target ) => target is not null;
}

[AttributeUsage( AttributeTargets.Class )]
public class IncludeActionAttribute : Attribute
{
}
