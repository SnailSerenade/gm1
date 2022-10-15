using Sandbox;

namespace gm1;

public class Kiji : Character
{
	public ClothingContainer Clothing { get; protected set; }

	private bool ClothingCreated = false;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );
	}

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		if ( !ClothingCreated )
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Clothing ??= new();
			Clothing.LoadFromClient( client );

			// Dress player
			Clothing.DressEntity( this );

			ClothingCreated = true;
		}
	}
}