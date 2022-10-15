namespace gm1;

public class Rat : Character
{
	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( "models/citizen/citizen.vmdl" );
	}
}