namespace gm1.Core.Abilities;

[IncludeAction]
public class Punch : Ability
{
	public override void Perform( Battle.BattleActor actor, Character target )
	{
		base.Perform( actor, target );

		target.Inflict( new Effects.Physical() { Severity = 3, Cause = this } );

		target.Health -= 1.0f;
	}
}
