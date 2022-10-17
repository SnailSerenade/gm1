using Sandbox;

namespace gm1.Battle;

public partial class BattleMember : EntityComponent
{
	[Net] public Battle Battle { get; set; }
}

public partial class BattleActor : EntityComponent
{
	[Net] public Battle Battle { get; set; }
	[Net] public Action Action { get; set; } = null;
	[Net] public Entity Target { get; set; } = null;
	public bool Selected => Action != null && Target != null;
}

public partial class Battle : Entity
{
	[Net] public Party PartyOne { get; set; }
	[Net] public Party PartyTwo { get; set; }
}