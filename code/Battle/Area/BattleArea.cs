using System;
using System.Linq;
using Sandbox;
using SandboxEditor;

namespace gm1.Battle.Area;

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
	public static BattleArea Random => All.OfType<BattleArea>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

	public PartySpotOne PartyOneArea => All.OfType<PartySpotOne>().Where( area => area.BattleAreaName == Name ).FirstOrDefault();
	public PartySpotTwo PartyTwoArea => All.OfType<PartySpotTwo>().Where( area => area.BattleAreaName == Name ).FirstOrDefault();
}