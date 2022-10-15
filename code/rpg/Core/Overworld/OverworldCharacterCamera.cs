using Sandbox;

namespace gm1;

public partial class OverworldCharacterCamera : CameraMode
{
	private Rotation InterimCameraRotation = Rotation.Identity;
	private Rotation CameraRotation = Rotation.Identity;

	/// <summary>
	/// Max distance from camera to player
	/// </summary>
	public float CameraDistance { get; protected set; } = 104f;

	private float InterimCameraDistance;

	public Vector3 PostOffset = Vector3.Up * 3;

	// hack(backwards fix), lotuspar
	private bool IsMovingBackwards = false;

	public override void Update()
	{
		if ( Local.Pawn is not Character pawn ) return;

		var rotationNoRoll = CameraRotation.Angles().WithRoll( 0 ).ToRotation();

		InterimCameraDistance = MathX.LerpTo( InterimCameraDistance, CameraDistance, Time.Delta * 13.0f );
		var tr = Trace.Ray( new Ray( pawn.EyePosition, rotationNoRoll.Backward ), InterimCameraDistance )
			.Ignore( pawn )
			.WithoutTags( "player" )
			.Radius( 4f )
			.IncludeClientside()
			.Run();

		InterimCameraDistance = tr.Distance;

		Position = tr.EndPosition + PostOffset;
		Rotation = rotationNoRoll;
		Viewer = null;
		ZNear = 4f;
		FieldOfView = 70;

		var alpha = MathX.Clamp( CurrentView.Position.Distance( pawn.EyePosition ) / CameraDistance, 0f, 1.1f ) - 0.1f;
		pawn.RenderColor = Color.White.WithAlpha( alpha );
		foreach ( var child in pawn.Children )
			if ( child is ModelEntity ent ) ent.RenderColor = pawn.RenderColor;
	}

	public void FrameSimulate( Rotation inputRotationDelta, float moveDirectionYaw )
	{
		/*
			This function is a bit messy and confusing.
		*/

		// : (
		var ProposedInterimCameraRotation = InterimCameraRotation.Angles().WithYaw( moveDirectionYaw ).ToRotation();

		if ( Input.Forward != 0 || Input.Left != 0 )
			if ( !IsMovingBackwards ) // hack(backwards fix), lotuspar
				InterimCameraRotation = Rotation.Slerp(
					InterimCameraRotation,
					ProposedInterimCameraRotation,
					Time.Delta * ((inputRotationDelta == Rotation.Identity) ? 1.55f : 0.4f)
				);

		InterimCameraRotation *= inputRotationDelta;

		CameraRotation = Rotation.Slerp( CameraRotation, InterimCameraRotation, Time.Delta * 15.5f );

		if ( Input.Forward != 0 || Input.Left != 0 )
			DebugOverlay.ScreenText( $"Left stick input: Forward {Input.Forward}, Left {Input.Left}", Vector2.One * 20, 0, Color.Cyan );
		else
			DebugOverlay.ScreenText( $"No left stick input", Vector2.One * 20, 0, Color.Red );

		if ( inputRotationDelta != Rotation.Identity )
			DebugOverlay.ScreenText( $"Right stick input: {Input.Rotation.Angles()}", Vector2.One * 20, 1, Color.Cyan );
		else
			DebugOverlay.ScreenText( $"No right stick input", Vector2.One * 20, 1, Color.Red );
	}

	public override void BuildInput( InputBuilder input )
	{
		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -15, 15 );
		input.ViewAngles.roll = 0;

		// hack(backwards fix), lotuspar
		IsMovingBackwards = input.AnalogMove.Normal.x < -0.8;
		input.InputDirection = (input.AnalogMove.x * CameraRotation.Forward.Normal) + -(input.AnalogMove.y * CameraRotation.Right.Normal);
	}
}