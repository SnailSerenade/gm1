using System.Collections.Generic;
using System.Linq;
using gm1.Battle;
using Sandbox;

namespace gm1.Core;

/// <summary>
/// Playable (or non-playable) RPG character.  
/// </summary>
public partial class Character : AnimatedEntity
{
	public PartyMember PartyMember => Components.Get<PartyMember>();
	public BattleMember BattleMember => Components.Get<BattleMember>();
	public BattleActor BattleActor => Components.Get<BattleActor>();

	public virtual float MaxHealth => 100.0f;

	[Net] public List<string> ActionNames { get; private set; } = new();
	public List<Action> Actions => ActionNames.Select( Action.Get ).ToList();

	public override void Spawn()
	{
		base.Spawn();

		Health = MaxHealth;

		Transmit = TransmitType.Always;

		ActionNames.Add( "Punch" );
		ActionNames.Add( "Breeze" );
	}

	/// <summary>
	/// Inflicts an <see cref="Effect"/> on this character.
	/// </summary>
	/// <param name="effect">Effect to inflict</param>
	public void Inflict<T>( T effect ) where T : Effect
	{
		// First get existing effect of that type
		var existing = Components.Get<T>();

		// Add new effect to run effect activation code
		Components.Add( effect );

		if ( existing == null )
		{
			// No existing effect, just return
			return;
		}

		// Combine severities
		existing.Severity += effect.Severity;

		// Remove newly added effect
		Components.Remove( effect );
	}
}
