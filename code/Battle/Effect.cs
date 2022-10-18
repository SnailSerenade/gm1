using Sandbox;

namespace gm1.BattleSys;

public partial class Effect : EntityComponent
{
	public string Name => GetType().Name;

	public virtual int MinimumSeverity { get; private set; } = 0;
	public virtual int NeutralSeverity { get; private set; } = 0;
	public virtual int MaximumSeverity { get; private set; } = 3;

	public int Neutral => NeutralSeverity;

	/// <summary>
	/// Cause of effect creation (action for example)
	/// </summary>
	[Net] public Action Cause { get; set; }

	[Net] private int InternalSeverity { get; set; } = 0;
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

	public static void UpdateSeverityFromCounterEffect( Effect target, Effect counter )
	{
		var targetSeveritySave = target.Severity;
		target.Severity -= counter.Severity;
		counter.Severity -= targetSeveritySave - target.Severity;
	}
}