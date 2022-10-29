namespace gm1.Core.Abilities;

[IncludeAction]
public class Breeze : Ability
{
	public override void Perform( Battle.BattleActor actor, Character target )
	{
		base.Perform( actor, target );

		target.Inflict( new Effects.Cold() { Severity = 1 } );

		target.Health -= 1.0f;
	}
}
