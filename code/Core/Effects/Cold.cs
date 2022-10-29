namespace gm1.Core.Effects;

public class Cold : Effect
{
	public bool Frozen => Severity == MaximumSeverity;

	protected override void OnActivate()
	{
		var heat = Entity.Components.Get<Heat>();
		if ( heat != null )
			UpdateSeverityFromCounterEffect( heat, this );
	}
}
