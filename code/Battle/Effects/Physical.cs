using Sandbox;

namespace gm1.BattleSys.Effects;

public partial class Physical : Effect
{
	protected override void OnActivate()
	{
		if ( Cause == null )
			return;

		if ( Cause is not Ability ability )
			return;

		var cold = Entity.Components.Get<Cold>();
		if ( cold != null && cold.Frozen )
		{
			cold.Severity = cold.Neutral;

			Entity.Health -= ability.DamageBase * Severity;
		}

		// Remove self after interactions
		Remove();
	}
}