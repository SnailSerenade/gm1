using Sandbox;

namespace gm1.Core;

public partial class Character
{
	private CameraComponent _cameraComponent;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		_cameraComponent ??= Components.Get<CameraComponent>();

		_cameraComponent?.PostCameraSetup( ref camSetup );
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

		_cameraComponent ??= Components.Get<CameraComponent>();

		_cameraComponent?.BuildInput( inputBuilder );
	}
}
