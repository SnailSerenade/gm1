using gm1.Core;
using gm1.UI;
using Sandbox;

namespace gm1.Battle;

/// <summary>
/// Component to show that the <see cref="Character"/> parent is able to pick a Target / Action in a
/// <see cref="Battle"/>.
/// </summary>
public partial class BattleActor : CharacterComponent
{
	private readonly ContainedComponentPanel<
		BattleActor, UI.DebugBattleActorUi> _debugUi;

	public BattleActor()
	{
		if ( Host.IsClient )
		{
			_debugUi = new(this);
		}
	}

	public Battle Battle => Entity?.Components.Get<BattleMember>()?.Battle;

	public Party Enemies =>
		Entity?.Components.Get<BattleMember>().Enemies ?? Battle?.InactiveParty; // quick way to get enemy party!

	[Event.Tick]
	public void Tick()
	{
		if ( Host.IsClient )
		{
			_debugUi.Tick();
		}

		if ( Enemies != null && Host.IsServer )
		{
			Target ??= Enemies.First( aliveOnly: true );

			if ( Host.IsServer && Entity.Client == null )
			{
				// Bot logic
				// Just select random target / action for now
				if ( Entity is Core.Character character )
				{
					if ( character.Actions.Count == 0 )
						return;

					Action = character.Actions[Rand.Int( character.Actions.Count - 1 )];

					foreach ( var enemy in Enemies )
					{
						if ( Action.CheckTarget( enemy.Character ) )
							Target = enemy;

						if ( Target == null && Action.CheckTarget( Entity as Core.Character ) )
							Target = Entity.Components.Get<Core.PartyMember>();
					}
				}

				AttemptLockIn();
			}
		}
	}
}
