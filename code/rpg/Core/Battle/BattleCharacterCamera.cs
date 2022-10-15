using Sandbox;

namespace gm1;

public partial class BattleCharacterCamera : CameraMode
{
	public override void Update()
	{
		if ( Local.Pawn is not Character pawn ) return;
	}
}