namespace gm1.Core.Effects;

public class Heat : Effect
{
	protected override void OnActivate()
	{
		var cold = Entity.Components.Get<Cold>();
		if ( cold != null )
			UpdateSeverityFromCounterEffect( cold, this );
	}
}
