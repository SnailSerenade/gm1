using Sandbox;

namespace gm1.Core;

public abstract class CameraComponent : CharacterComponent
{
	public abstract void PostCameraSetup( ref CameraSetup camSetup );
	public abstract void BuildInput( InputBuilder inputBuilder );
}
