using Sandbox;

namespace gm1.BattleSys;

public partial class BattleMember : EntityComponent
{
	[Net] public Battle Battle { get; set; }

	UI.BattleMemberUi Panel;

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsClient && Entity == Local.Pawn )
		{
			Panel = new();

			Local.Hud.AddChild( Panel );
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient && Panel != null )
		{
			Panel.Delete();
		}
	}
}