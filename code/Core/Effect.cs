using Sandbox;

namespace gm1.Core;

public partial class Effect : CharacterComponent
{
	public string Name => GetType().Name;
	public virtual string DisplayName => Name;

	public virtual int MinimumSeverity { get; private set; } = 0;
	public virtual int NeutralSeverity { get; private set; } = 0;
	public virtual int MaximumSeverity { get; private set; } = 3;

	public int Neutral => NeutralSeverity;

	/// <summary>
	/// Cause of effect creation (action for example)
	/// </summary>
	[Net]
	public Action Cause { get; init; }

	[Net] private int InternalSeverity { get; set; }

	public int Severity
	{
		get => InternalSeverity;
		set
		{
			if ( value > MaximumSeverity )
				InternalSeverity = MaximumSeverity;
			else if ( value < MinimumSeverity )
				InternalSeverity = MinimumSeverity;
			else
				InternalSeverity = value;
		}
	}

	protected static void UpdateSeverityFromCounterEffect( Effect target, Effect counter )
	{
		var targetSeveritySave = target.Severity;
		target.Severity -= counter.Severity;
		counter.Severity -= targetSeveritySave - target.Severity;
	}
}
