using System;
using System.Linq;
using Sandbox;
using SandboxEditor;

namespace gm1.Entities;

[Library( "srpg_battle_area_cfg" ), HammerEntity]
[Title( "Battle Area Configuration" ), Category( "Gameplay" ), Icon( "place" )]
[Description( "Battle area configuration. Needs to be set per-map" )]
public partial class BattleAreaConfig : Entity
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
	public static BattleAreaConfig Random => All.OfType<BattleAreaConfig>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

	public BattleAreaParty PartyOneArea => All.OfType<BattleAreaPartyOne>().Where( area => area.BattleAreaName == Name ).FirstOrDefault();
	public BattleAreaParty PartyTwoArea => All.OfType<BattleAreaPartyTwo>().Where( area => area.BattleAreaName == Name ).FirstOrDefault();
}

public abstract partial class BattleAreaParty : Entity
{
	public override void Spawn()
	{
		// Confirm properties
		if ( BattleAreaName == null )
			throw new ArgumentNullException( "BattleAreaName", "BattleAreaParty not linked to a BattleAreaConfig!" );
	}

	/// <summary>
	/// Battle area
	/// </summary>
	[Net]
	[Property( Title = "Battle Area" ), FGDType( "target_destination" )]
	public string BattleAreaName { get; set; } = null;
	public BattleAreaConfig BattleArea
		=> All.OfType<BattleAreaConfig>().Where( ( cfg ) => cfg.Name == BattleAreaName ).FirstOrDefault();

	/// <summary>
	/// Spacing (in units) between actors.
	/// </summary>
	[Net]
	[Property( Title = "Actor Spacing" )]
	public int ActorSpacing { get; set; } = 32;

	[Net]
	[Property( Title = "Invert Direction" )]
	public bool InvertDirection { get; set; } = true;

	[Net]
	[Property( Title = "Invert Order" )]
	public bool InvertOrder { get; set; } = false;
}

[Library( "srpg_battle_area_party_one" ), HammerEntity]
[Title( "Battle Area Party One" ), Category( "Gameplay" ), Icon( "place" ), EditorModel( "models/light_arrow.vmdl" )]
[Description( "Area for party one to stand in" )]
public class BattleAreaPartyOne : BattleAreaParty
{}

[Library( "srpg_battle_area_party_two" ), HammerEntity]
[Title( "Battle Area Party One" ), Category( "Gameplay" ), Icon( "place" ), EditorModel( "models/light_arrow.vmdl" )]
[Description( "Area for party one to stand in" )]
public class BattleAreaPartyTwo : BattleAreaParty
{}