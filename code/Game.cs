using gm1.Core;
using Sandbox;
using Action = gm1.Battle.Action;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace gm1;

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
		Action.LoadKnownActions();

		if ( Host.IsClient )
		{
			Local.Hud = new Sandbox.UI.RootPanel();
			//Local.Hud = new Test();
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );
		client.Pawn = new Pawn();
	}

	[ConCmd.Admin( "ccmd02" )]
	public static void Ccmd02()
	{
		Log.Info( Action.Get( "Punch" ) );
	}

	[ConCmd.Admin( "ccmd01" )]
	public static void Ccmd01()
	{
		Party party = new()
		{
			ConsoleSystem.Caller.Pawn as Pawn
		};

		var battle = new Battle.Battle(
			partyOne: party,
			partyTwo: new Party {
				new Pawn(), new Pawn(), new Pawn()
			}
		);

		battle.Start();
	}
}
