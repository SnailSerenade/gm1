using gm1.Core;
using Sandbox;

namespace gm1.Battle;

/// <summary>
/// Component to show that the <see cref="Character"/> parent is a member of a <see cref="Battle"/>.
/// </summary>
public partial class BattleMember : CharacterComponent
{
	[Net] public Battle Battle { get; set; }

	/// <summary>
	/// Enemy party of current <see cref="Battle"/>.
	/// </summary>
	[Net]
	public Party Enemies { get; set; }

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( !Host.IsClient )
			return;

		if ( Local.Pawn != null )
			Local.Pawn.Components.RemoveAny<BattleCamera>();
	}

	[Event.Tick]
	public void Tick()
	{
		if ( Battle == null )
			return;

		if ( Host.IsClient && Entity == Local.Pawn ) Local.Pawn.Components.Add( new BattleCamera() );

		Event.Unregister( this );
	}
}
