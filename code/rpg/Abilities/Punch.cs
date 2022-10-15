namespace gm1.Abilities;

public class Punch : Ability
{
	public override void UseOn( Actor actor )
	{
		base.UseOn( actor );

		actor.InteractWith( Physical.Soft );

		actor.TakeDamage( 20.0f );
	}
}