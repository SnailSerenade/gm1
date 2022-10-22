using Sandbox;

namespace gm1;

public partial class Character : AnimatedEntity
{
	public PartyMember PartyMember => Components.Get<PartyMember>();
	public BattleSys.BattleMember BattleMember => Components.Get<BattleSys.BattleMember>();
	public BattleSys.BattleActor BattleActor => Components.Get<BattleSys.BattleActor>();

	public virtual float MaxHealth => 100.0f;

	public override void Spawn()
	{
		base.Spawn();

		Health = MaxHealth;

		Transmit = TransmitType.Always;
	}
}