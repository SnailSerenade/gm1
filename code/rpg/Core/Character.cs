using System.Collections.Generic;
using Sandbox;

namespace gm1;

public abstract partial class Character : Actor
{
	[Net]
	public List<Ability> Abilities { get; set; } = new List<Ability>();

	public override void OnBattleLeft()
	{
		base.OnBattleLeft();

		// Change to overworld setup
		Controller = new OverworldCharacterController();
		Camera = new OverworldCharacterCamera();
	}

	public override void OnBattleEntered( Battle battle )
	{
		base.OnBattleEntered( battle );

		// Change to battle setup
		Controller = new BattleCharacterController();
		Camera = new BattleCharacterCamera();
	}
}