namespace gm1;

public enum AbilityTarget
{
	ANY = 0,
	ALL,
	ENVIRONMENT_ONLY,
	CHARACTER_ONLY
}

public abstract class Ability
	: Sandbox.BaseNetworkable // : (
{
	/// <summary>
	/// Get the name of the ability
	/// </summary>
	public string Name => GetType().Name;

	/// <summary>
	/// Get targets of ability
	/// </summary>
	public virtual AbilityTarget Target => AbilityTarget.ANY;

	public virtual void UseOn( Actor actor ) { }
}