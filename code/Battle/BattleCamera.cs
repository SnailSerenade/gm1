using Sandbox;

namespace gm1.Battle;

/// <summary>
/// Main camera mode for the <see cref="Battle"/> system.
/// To find the camera position (during player turn), the directionality & target points are used.
/// The steps in order:
/// (1.) The camera starts at the position of the <see cref="BattleActor"/>'s character's eyes.
/// (2.) The camera looks at the Directionality Reference Point.
/// (3.) The camera moves right.
/// (4.) The camera looks at the Target Reference Point.
/// (5.) The camera moves backwards.
/// (6.) The offset is added to the camera position.
/// </summary>
public class BattleCamera : CameraMode
{
	/// <summary>
	/// Reference point to calculate sideways offset of the camera
	/// </summary>
	private Vector3 _directionalityReferencePoint;

	/// <summary>
	/// Reference point to calculate look direction & final position of the camera
	/// </summary>
	private Vector3 _targetReferencePoint;

	/// <summary>
	/// End goal of camera position
	/// </summary>
	Vector3 _wishPosition = Vector3.One;

	/// <summary>
	/// End goal of camera rotation
	/// </summary>
	private Rotation _wishRotation = Rotation.Identity;

	/// <summary>
	/// Offset added to final calculated position
	/// </summary>
	private readonly Vector3 _offset = Vector3.Up * 50.0f;

	/// <summary>
	/// Multiplier for Time.<see cref="Time.Delta"/>  when lerping to final position / rotation
	/// </summary>
	private const float LerpMultiplier = 5.0f;

	/// <summary>
	/// Called by <see cref="Update"/> when the local Pawn has a <see cref="BattleActor"/> component.
	/// </summary>
	private void UpdateForCurrentActor( BattleActor battleActor )
	{
		if ( battleActor.Target != null )
			_directionalityReferencePoint = battleActor.Target.Character.EyePosition;

		// (1.) The camera starts at the position of the <see cref="BattleActor"/>'s character's eyes
		_wishPosition = battleActor.Character.EyePosition;

		// (2.) The camera looks at the Directionality Reference Point.
		_wishRotation = Rotation.LookAt( _directionalityReferencePoint - _wishPosition );

		// (3.) The camera moves right.
		_wishPosition += _wishRotation.Right * 31.0f;

		// (4.) The camera looks at the Target Reference Point.
		_wishRotation = Rotation.LookAt( _targetReferencePoint - _wishPosition );

		// (5.) The camera moves backwards.
		_wishPosition += _wishRotation.Backward * 115.0f;

		// (6.) The offset is added to the camera position.
		_wishPosition += _offset;

		DebugOverlay.Circle( _directionalityReferencePoint, _wishRotation, 5.0f, Color.FromBytes( 0xB6, 0xFF, 0x00 ) );
		DebugOverlay.Circle( _targetReferencePoint, _wishRotation, 5.0f, Color.FromBytes( 0x00, 0x94, 0xFF ) );
		DebugOverlay.Line( _directionalityReferencePoint, _targetReferencePoint, Color.Gray );
	}

	public override void Update()
	{
		// Lerp to end goals
		Position = Position.LerpTo( _wishPosition, Time.Delta * LerpMultiplier );
		Rotation = Rotation.Lerp( Rotation, _wishRotation, Time.Delta * LerpMultiplier );

		var battleActor = Local.Pawn.Components.Get<BattleActor>();
		if ( battleActor != null )
			UpdateForCurrentActor( battleActor );
	}

	public override void Activated()
	{
		base.Activated();

		var battleMember = Local.Pawn.Components.Get<BattleMember>();

		if ( battleMember != null )
		{
			var firstEnemy = battleMember.Enemies.Last();
			var lastEnemy = battleMember.Enemies.First();

			if ( firstEnemy == lastEnemy )
			{
				_targetReferencePoint = firstEnemy.Character.EyePosition;
				_directionalityReferencePoint = _targetReferencePoint + firstEnemy.Character.Rotation.Left * 10.0f;
			}
			else
			{
				var delta = lastEnemy.Character.EyePosition - firstEnemy.Character.EyePosition;
				_targetReferencePoint = firstEnemy.Character.EyePosition + (delta * 0.22f);
				_directionalityReferencePoint = firstEnemy.Character.EyePosition + (delta * 0.45f);
			}
		}
	}
}
