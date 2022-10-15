using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Sandbox;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class MyGame : Sandbox.Game
{
	public MyGame()
	{
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		client.Pawn = new gm1.Kiji();
	}

	[ConCmd.Admin( "spawnrat" )]
	public static void SpawnRat()
	{
		_ = new gm1.Rat
		{
			Position = ConsoleSystem.Caller.Pawn.Position
		};
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		foreach ( var battle in Entity.All.OfType<gm1.Battle>() )
		{
			battle.Simulate( cl );
		}
	}

	[ConCmd.Admin( "benewrat" )]
	public static void BeNewRat()
	{
		ConsoleSystem.Caller.Pawn = new gm1.Rat
		{
			Position = ConsoleSystem.Caller.Pawn.Position
		};
	}

	[ConCmd.Admin( "kit" )]
	public static void GiveKit()
	{
		var self = ConsoleSystem.Caller.Pawn as gm1.Kiji;
		self.Abilities.Add( new gm1.Abilities.Inferno() );
		self.Abilities.Add( new gm1.Abilities.Punch() );
	}

	[ConCmd.Admin( "battle" )]
	public static void Battle()
	{
		var self = ConsoleSystem.Caller.Pawn as gm1.Kiji;

		var battle = new gm1.Battle( self )
		{
			PartyOne = new gm1.Party { self },
			PartyTwo = new gm1.Party { new gm1.Rat(), new gm1.Rat() }
		};
		battle.Start();
	}

	[ConCmd.Admin( "endbattle" )]
	public static void EndBattle()
	{
		foreach ( var battle in Entity.All.OfType<gm1.Battle>() )
		{
			battle.End();
		}
	}

	[ConCmd.Admin( "inferno" )]
	public static void Inferno()
	{
		foreach ( var actor in Entity.All.OfType<gm1.Actor>() )
		{
			new gm1.Abilities.Inferno().UseOn( actor );
		}
	}

	[ConCmd.Client( "cinferno" )]
	public static void CInferno()
	{
		foreach ( var actor in Entity.All.OfType<gm1.Actor>() )
		{
			foreach (var element in actor.Elements) {
				Log.Info( element );
			}
		}
	}

	[ConCmd.Server( "sinferno" )]
	public static void SInferno()
	{
		foreach ( var actor in Entity.All.OfType<gm1.Actor>() )
		{
			foreach (var element in actor.Elements) {
				Log.Info( element );
			}
		}
	}
}
