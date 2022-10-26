namespace gm1.Battle.Abilities;

[IncludeAction]
public class Punch : Ability
{
	public override void Perform( BattleActor actor, Core.Character target )
	{
		base.Perform( actor, target );

		target.Components.Add( new Effects.Physical() { Severity = 3 } );

		target.Health -= 1.0f;
	}
}
