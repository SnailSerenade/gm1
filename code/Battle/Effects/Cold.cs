using Sandbox;

namespace gm1.BattleSys.Effects;

public partial class Cold : Effect
{
	public bool Frozen => Severity == MaximumSeverity;

	protected override void OnActivate()
	{
		var heat = Entity.Components.Get<Heat>();
		if ( heat != null )
			UpdateSeverityFromCounterEffect( heat, this );
	}
}