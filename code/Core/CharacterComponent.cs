using Sandbox;

namespace gm1;

public partial class CharacterComponent : EntityComponent
{
	public Character Character => Entity as Character;
}