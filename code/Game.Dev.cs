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
	public override void RenderHud()
	{
		foreach ( var actor in Entity.All.OfType<gm1.Actor>() )
		{
			int offset = 0;
			DebugOverlay.Text( actor.GetType().Name, actor.Position, offset++, Color.Orange );
			DebugOverlay.Text( $"hp {actor.Health}", actor.Position, offset++, Color.Cyan );
			if ( actor.Elements != null )
				foreach ( var element in actor.Elements )
				{
					DebugOverlay.Text( $"* {element.Name} {element.State}", actor.Position, offset++, Color.Cyan );
				}
		}
	}
}
