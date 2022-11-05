using Sandbox;
using SandboxEditor;

namespace gm1.Overworld;

[Library( "gm1_cameralocation" ), HammerEntity]
[Title( "Camera Location" ), Category( "Gameplay" ), Icon( "place" )]
[Description( "Camera object to be used in events / trigger spots" )]
[EditorModel( "models/editor/camera.vmdl" )]
public partial class CameraLocation : Entity
{
	[Net]
	[Property( Title = "Field of View" )]
	public float FieldOfView { get; set; } = 80.0f;
}
