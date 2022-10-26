using System.Linq;
using Sandbox;

namespace gm1.Battle.PostProcess;

/// <summary>
/// Draws the highlight outline effect. You may know this effect from
/// such games as Garry's Mod.
/// </summary>
[SceneCamera.AutomaticRenderHook]
internal class HighlightRenderer : RenderHook
{
	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		if ( renderStage == Stage.AfterTransparent )
		{
			RenderEffect();
		}
	}

	public static void RenderEffect()
	{
		var actor = Local.Pawn.Components.Get<BattleActor>();
		if ( actor == null )
			return;

		if ( actor.Target == null )
			return;

		if ( actor.Target.Entity is not ModelEntity modelEntity )
			return;

		DrawGlow( Color.Blue, Color.Orange, 10, modelEntity );
	}

	private static void DrawGlow( Color color, Color occludedColor, float lineWidth, ModelEntity entity )
	{
		var shapeMat = Material.FromShader( "HighlightObject.vfx" );
		var screenMat = Material.FromShader( "HighlightPostProcess.vfx" );

		Graphics.GrabDepthTexture( "DepthTexture" );

		using var rt = RenderTarget.GetTemporary();

		//
		// we need to have a depth buffer here too
		//
		Graphics.RenderTarget = rt;

		Graphics.Clear( Color.Black, true, true );


		if (entity.EnableDrawing) {
			var sceneObject = entity.SceneObject;
			Graphics.Render( sceneObject, material: shapeMat );
		}

		foreach ( var ent in entity.Children.Distinct() )
		{
			if ( ent is not ModelEntity modelEnt ) continue;
			if ( !modelEnt.EnableDrawing ) continue;

			var so = modelEnt.SceneObject;
			if ( !so.IsValid() ) continue;

			Graphics.Render( so, material: shapeMat );
		}

		Graphics.RenderTarget = null;

		RenderAttributes materialAttributes = new();
		materialAttributes.Set( "ColorBuffer", rt.ColorTarget );
		materialAttributes.Set( "LineColor", color );
		materialAttributes.Set( "OccludeColor", occludedColor );
		materialAttributes.Set( "LineWidth", lineWidth );
		Graphics.Blit( screenMat, materialAttributes );
	}
}
