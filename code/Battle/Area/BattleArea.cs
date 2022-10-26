using System;
using System.Linq;
using Sandbox;
using SandboxEditor;

namespace gm1.Battle.Area;

/// <summary>
/// Configuration map entity and 
/// </summary>
[Library( "gm1_battlearea" ), HammerEntity]
[Title( "Battle Area Configuration" ), Category( "Gameplay" ), Icon( "place" )]
[Description( "Battle area configuration. Needs to be referenced by other battle area entities" )]
public partial class BattleArea : Entity
{
	[Net]
	[Property( Title = "Party One Max" )]
	public int PartyOneMax { get; set; } = 4;

	[Net]
	[Property( Title = "Party Two Max" )]
	public int PartyTwoMax { get; set; } = 4;

	/// <summary>
	/// Get random BattleAreaConfig on map
	/// </summary>
	public static BattleArea Random => All.OfType<BattleArea>().MinBy( x => Guid.NewGuid() );

	public PartySpotOne PartyOneArea =>
		All.OfType<PartySpotOne>().FirstOrDefault( area => area.BattleAreaName == Name );

	public PartySpotTwo PartyTwoArea =>
		All.OfType<PartySpotTwo>().FirstOrDefault( area => area.BattleAreaName == Name );
}
