using Sandbox;

namespace gm1;

public partial class Character : AnimatedEntity
{
	public PartyMember PartyMember => Components.Get<PartyMember>();
}