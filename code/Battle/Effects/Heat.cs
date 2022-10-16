using Sandbox;

namespace gm1.Battle.Effects;

public partial class Heat : Effect
{
	protected override void OnActivate()
	{
		var cold = Entity.Components.Get<Cold>();
		if ( cold != null )
			UpdateSeverityFromCounterEffect( cold, this );
	}
}