using System.Linq;
using gm1.Core;
using Sandbox;
using SandboxEditor;

namespace gm1.Overworld;

[Library( "gm1_cameratrigger" ), HammerEntity, Solid]
[Title( "Camera Trigger" ), Category( "Gameplay" ), Icon( "place" )]
[Description( "Camera trigger area" )]
public partial class CameraTrigger : Entity
{
	[Net]
	[Property( Title = "Camera Location Name" ), FGDType( "target_destination" )]
	public string CameraLocationName { get; set; } = null;

	private CameraLocation _cameraLocation;

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		_cameraLocation ??= All.OfType<CameraLocation>().FirstOrDefault( cfg => cfg.Name == CameraLocationName );

		if ( other is Character character )
		{
			character.Components.Add( new SpotCameraComponent( _cameraLocation ) );
		}
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		_cameraLocation ??= All.OfType<CameraLocation>().FirstOrDefault( cfg => cfg.Name == CameraLocationName );

		if ( other is not Character character )
		{
			return;
		}

		foreach ( var component in character.Components.GetAll<SpotCameraComponent>() )
		{
			if ( component.CameraLocation == _cameraLocation )
				component.Remove();
		}
	}
}
