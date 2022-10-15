namespace gm1.Abilities;

public class Inferno : Ability
{
	public override void UseOn( Actor actor )
	{
		base.UseOn( actor );

		actor.InteractWith( Temperature.Burning );

		actor.TakeDamageAsPercentage( 0.02f );
	}
}