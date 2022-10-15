using System;
using Sandbox;

namespace gm1;

public abstract partial class Character : Actor
{
	[Net, Predicted]
	public PawnController Controller { get; set; }

	[Net, Predicted]
	public PawnAnimator Animator { get; set; }

	public override void Spawn()
	{
		EnableAllCollisions = true;

		Trace trace = Trace.Ray( EyePosition, EyePosition + Vector3.Down * 1024 )
						.Ignore( this )
						.Size( 4 );
		var result = trace.Run();
		Position = result.EndPosition;

		Controller ??= new OverworldCharacterController();
		Camera ??= new OverworldCharacterCamera();
	}

	public CameraMode Camera
	{
		get => Components.Get<CameraMode>();
		set
		{
			if ( Camera == value ) return;
			Components.RemoveAny<CameraMode>();
			Components.Add( value );
		}
	}

	public override void Simulate( Client client )
	{
		if ( Host.IsServer )
			UpdateEyePosition();

		Controller?.Simulate( client, this, Animator );
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( input.StopProcessing )
			return;

		Controller?.BuildInput( input );

		if ( input.StopProcessing )
			return;

		Animator?.BuildInput( input );

		if ( input.StopProcessing )
			return;

		Camera?.BuildInput( input );
	}

	/// <summary>
	/// Input.Rotation on last call
	/// </summary>
	private Rotation FrameSimulatePreviousInputRotation = Rotation.Identity;
	protected Rotation FrameSimulateInputRotationDelta { get => FrameSimulatePreviousInputRotation.Inverse * Input.Rotation; }
	public override void FrameSimulate( Client client )
	{
		UpdateEyePosition();

		Controller?.FrameSimulate( client, this, Animator );

		if ( Controller != null && Controller is OverworldCharacterController controller )
		{
			(Camera as OverworldCharacterCamera).FrameSimulate(
				FrameSimulateInputRotationDelta, controller.LastMoveDirection.Yaw() );
		}

		FrameSimulatePreviousInputRotation = Input.Rotation;
	}

	/// <summary>
	/// Update pawn eye position
	/// (8 units below their top bounds)
	/// </summary>
	public void UpdateEyePosition()
	{
		EyeLocalPosition = Vector3.Up * (CollisionBounds.Maxs.z - 8);
		EyePosition = Position + EyeLocalPosition;
	}
}