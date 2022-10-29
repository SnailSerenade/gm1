namespace gm1.Core.Effects;

public class Physical : Effect
{
	protected override void OnActivate()
	{
		if ( Cause is Ability ability )
		{
			var cold = Entity.Components.Get<Cold>();
			if ( cold is { Frozen: true } )
			{
				cold.Severity = cold.Neutral;

				Entity.Health -= ability.DamageBase * Severity;
			}
		}

		// Remove self after interactions
		Remove();
	}
}
