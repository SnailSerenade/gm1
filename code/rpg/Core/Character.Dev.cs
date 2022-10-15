using System.Collections.Generic;
using Sandbox;

namespace gm1;

public abstract partial class Character : Actor
{
	[ConCmd.Admin( "letmeout" )]
	public static void LetMeOut( float x = 10, float y = 10, float z = 10 )
	{
		ConsoleSystem.Caller.Pawn.Position += new Vector3( x, y, z );
		Trace trace = Trace.Ray( ConsoleSystem.Caller.Pawn.Position, ConsoleSystem.Caller.Pawn.Position + Vector3.Down * 1024 )
						.Ignore( ConsoleSystem.Caller.Pawn )
						.Size( 4 );
		var result = trace.Run();
		ConsoleSystem.Caller.Pawn.Position = result.EndPosition;
	}
}