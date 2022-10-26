namespace gm1.Battle.Effects;

public class Physical : Effect
{
	protected override void OnActivate()
	{
		if ( Cause is not Ability ability )
			return;

		var cold = Entity.Components.Get<Cold>();
		if ( cold is { Frozen: true } )
		{
			cold.Severity = cold.Neutral;

			Entity.Health -= ability.DamageBase * Severity;
		}

		// Remove self after interactions
		Remove();
	}
}
