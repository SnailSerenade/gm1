using System;
using Sandbox;

namespace gm1;

public partial class OverworldCharacterController : PawnController
{
	public Rotation LastMoveDirection { get; private set; } = Rotation.Identity;

	// sorta hack(walking in wall fix), lotuspar
	private Vector3 WalkingFullMins = Vector3.Zero;
	private Vector3 WalkingFullMaxs = Vector3.Zero;

	private Vector3 WishAnimationVelocity = Vector3.Zero;
	private Vector3 UsedAnimationVelocity = Vector3.Zero;

	/// <summary>
	/// Update WalkingFullMins / WalkingFullMaxs
	/// </summary>
	protected virtual void UpdateWalkingBbox()
	{
		const float BodyGirth = 32.0f;
		const float BodyHeight = 72.0f;
		var girth = BodyGirth * 0.5f;
		WalkingFullMins = new Vector3( -girth, -girth, 0 ) * Pawn.Scale;
		WalkingFullMaxs = new Vector3( +girth, +girth, BodyHeight ) * Pawn.Scale;
	}

	public override void Simulate()
	{
		UpdateWalkingBbox();
        
		// Build movement from input values
		var movement = new Vector3( Input.Forward, Input.Left, 0 );

		// Set velocity
		Velocity = movement;

		Velocity *= Input.Down( InputButton.PrimaryAttack ) ? 230 : 150;

		if ( GroundEntity == null )
			Velocity += Vector3.Down * 450.0f * Time.Delta;

		// Try to move
		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = 46.0f
		};

		helper.Trace = helper.Trace.Size( WalkingFullMins, WalkingFullMaxs )
			.Ignore( Pawn )
			.IncludeClientside();

		helper.TryUnstuck();

		float result = helper.TryMoveWithStep( Time.Delta, 18.0f );

		// Prep animations		
		CitizenAnimationHelper animationHelper = new( Pawn as AnimatedEntity )
		{
			AimAngle = Rotation.LookAt( movement.Normal )
		};

		// Handle animation velocity
		WishAnimationVelocity = (result <= 0) ? Vector3.Zero : Velocity;

		if ( result > 1 )
		{
			// Impacting wall...
			// Slow animation velocity by a lot
			WishAnimationVelocity = Velocity / 4;
		}

		UsedAnimationVelocity = UsedAnimationVelocity.LerpTo( WishAnimationVelocity, Time.Delta * 27.0f );

		animationHelper.WithWishVelocity( UsedAnimationVelocity );
		animationHelper.WithVelocity( UsedAnimationVelocity );

		// Set new position / velocity
		Position = helper.Position;
		Velocity = helper.Velocity;

		LastMoveDirection = Rotation.LookAt( movement.Normal );

		// This part below is pretty much fully from Stud Jump...
		// https://github.com/Small-Fish-Dev/stud-jump/blob/4a4d6167fd719dbc5bcba67fe74347d132fefb1d/code/Player/Movement/PlayerController.cs#L84
		if ( Velocity.z <= 2f )
		{
			// Trace downwards to get GroundEntity
			var trace = helper.TraceDirection( Vector3.Down * 2.0f );

			GroundEntity = trace.Entity;

			if ( GroundEntity != null )
			{
				Position += trace.Distance * Vector3.Down;

				if ( Velocity.z < 0.0f )
					Velocity = Velocity.WithZ( 0 );
			}
		}
		else
		{
			GroundEntity = null;
		}

		Rotation = Rotation.Slerp( Rotation, LastMoveDirection, (Velocity.Length / 170) * Time.Delta * 4.0f );
		EyeRotation = Rotation;
	}
}