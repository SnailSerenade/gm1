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
	}
}
