namespace gm1.Core.Abilities;

[IncludeAction]
public class HeatUp : Ability
{
	public override string DisplayName => "Heat Up";

	public override void Perform( Battle.BattleActor actor, Character target )
	{
		base.Perform( actor, target );

		target.Inflict( new Effects.Heat() { Severity = 1, Cause = this } );

		target.Health -= 1.0f;
	}
}
