namespace gm1.BattleSys.Abilities;

public class Punch : Ability
{
	public Punch( BattleActor actor ) : base( actor )
	{
	}

	public override void Perform( Character target )
	{
		base.Perform( target );

		target.Components.Add( new Effects.Physical() { Severity = 3 } );

		target.Health -= 1.0f;
	}
}