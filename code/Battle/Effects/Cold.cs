using Sandbox;

namespace gm1.Battle.Effects;

public partial class Cold : Effect
{
	protected override void OnActivate()
	{
		var heat = Entity.Components.Get<Heat>();
		if ( heat != null )
			UpdateSeverityFromCounterEffect( heat, this );
	}
}