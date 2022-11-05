using gm1.Core;
using Sandbox;

namespace gm1.Overworld;

public class SpotCameraComponent : CameraComponent
{
	public CameraLocation CameraLocation { get; private set; }

	public SpotCameraComponent( CameraLocation cameraLocation )
	{
		CameraLocation = cameraLocation;
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		if ( Local.Pawn is not Character character ) return;

		if ( CameraLocation == null ) return;

		camSetup.Position = CameraLocation.Position;
		camSetup.Rotation = CameraLocation.Rotation;
		camSetup.FieldOfView = CameraLocation.FieldOfView;
		camSetup.ZNear = 4f;
		camSetup.Viewer = null;
	}

	public override void BuildInput( InputBuilder inputBuilder ) { }
}
