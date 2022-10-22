using Sandbox;

namespace gm1.BattleSys;

public partial class Action : BaseNetworkable
{
	public Action( BattleActor actor ) => Actor = actor;

	public virtual string Name => GetType().Name;

	[Net] public BattleActor Actor { get; protected set; } = null;

	/// <summary>
	/// Check if provided character is allowed to be targeted by this action
	/// </summary>
	/// <param name="target">Entity</param>
	/// <returns>True if provided character can be targeted</returns>
	public virtual bool CheckTarget( Character target ) { return false; }

	public virtual void Perform( Character target )
	{
		if ( !CheckTarget( target ) )
			throw new System.Exception( "Action can't be performed on provided target" );
	}
}

public class Ability : Action
{
	public Ability( BattleActor actor ) : base( actor ) { }

	public virtual float DamageBase => 1.0f;

	public override bool CheckTarget( Character target )
	{
		if ( target is not null )
			return true;

		return false;
	}
}