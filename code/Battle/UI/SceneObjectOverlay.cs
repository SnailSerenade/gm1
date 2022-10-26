using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace gm1.Battle.UI;

[UseTemplate]
public partial class SceneObjectOverlay : Panel
{
	public override bool HasContent => true;

	public SceneObject ExistingSceneObject { get; set; }

	private SceneModel _sceneModel;
	private SceneCamera _sceneCamera;
	private SceneWorld _sceneWorld;
	private SceneLight _mainSceneLight;
	private Texture _renderTexture;

	public SceneCamera ReplicateCamera( SceneCamera existing, SceneCamera old = null )
	{
		var output = old ?? new SceneCamera( "SceneObjectOverlay" )
		{
			Rotation = existing.Rotation,
			Ortho = existing.Ortho,
			Position = existing.Position,
			AntiAliasing = existing.AntiAliasing,
			AmbientLightColor = existing.AmbientLightColor,
			BackgroundColor = existing.BackgroundColor,
			EnablePostProcessing = existing.EnablePostProcessing,
			ZNear = existing.ZNear,
			ZFar = existing.ZFar,
			OrthoHeight = existing.OrthoHeight,
			FieldOfView = existing.FieldOfView
		};

		return output;
	}

	public SceneWorld ReplicateSceneWorld( SceneWorld existing, SceneWorld old = null )
	{
		var output = old ?? new SceneWorld(); // { AmbientLightColor = existing.AmbientLightColor, };

		_mainSceneLight = new SceneLight( output, Map.Camera.Position, 1024, Color.White );

		return output;
	}

	public SceneModel ReplicateSceneModel( SceneObject existing, SceneModel old = null )
	{
		var output =
			old ?? new SceneModel( _sceneWorld, existing.Model,
				existing.Transform ); // { AmbientLightColor = existing.AmbientLightColor, };

		if ( old == null )
			output.SetAnimGraph( "models/citizen/citizen.vanmgrph" );

		return output;
	}

	public override void DrawContent( ref RenderState state )
	{
		base.DrawContent( ref state );

		if ( ExistingSceneObject == null )
			return;

		_sceneWorld ??= ReplicateSceneWorld( Map.Scene, _sceneWorld );

		_sceneModel ??= ReplicateSceneModel( ExistingSceneObject, _sceneModel );

		_sceneModel.Transform = ExistingSceneObject.Transform;

		ExistingSceneObject.RenderingEnabled = false;

		_sceneModel.Update( RealTime.Delta );

		_renderTexture =
			Texture.CreateRenderTarget( "__sceneObjectOverlay", ImageFormat.RGBA8888, Screen.Size, _renderTexture );

		_sceneCamera = ReplicateCamera( Map.Camera, _sceneCamera );
		_sceneCamera.BackgroundColor = Color.Transparent;
		_sceneCamera.Position = Map.Camera.Position;
		_sceneCamera.Rotation = Map.Camera.Rotation;
		_sceneCamera.World = _sceneWorld;
		_sceneCamera.AmbientLightColor = Color.Green;
		_sceneCamera.AntiAliasing = true;

		_mainSceneLight.Position = _sceneCamera.Position;

		Graphics.RenderToTexture( _sceneCamera, _renderTexture );

		RenderAttributes attributes = new();
		Graphics.RenderTarget = null;
		attributes.Set( "Texture", _renderTexture );
		attributes.Set( "VrMonitor", true );
		Graphics.DrawQuad( new Rect( 0, 0, Screen.Width, Screen.Height ), Material.UI.Basic, Color.White, attributes );
	}
}
