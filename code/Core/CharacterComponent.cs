using Sandbox;

namespace gm1.Core;

/// <summary>
/// Small helper to add Character to EntityComponent.
/// </summary>
public class CharacterComponent : EntityComponent
{
	public Character Character => Entity as Character;
}
