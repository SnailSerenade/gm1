using System;
using Sandbox;
using Sandbox.UI;

namespace gm1.UI;

[UseTemplate]
public partial class Glyph : Panel
{
	public Image Image { get; private set; }
	public InputGlyphSize ButtonSize { get; private set; } = InputGlyphSize.Medium;
	public InputButton Button { get; private set; } = 0;
	public bool HasButton => Button != 0;

	public override void SetProperty( string name, string value )
	{
		base.SetProperty( name, value );

		if ( name == "input" )
		{
			try
			{
				Button = Enum.Parse<InputButton>( value, true );
			}
			catch ( ArgumentException )
			{
				// name not found, try number
				if ( ulong.TryParse( value, out ulong number ) )
				{
					Button = (InputButton)number;
				}
				else
				{
					throw new ArgumentException( $"Couldn't find input glyph for {value}" );
				}
			}
		}

		if ( name == "size" )
			ButtonSize = Enum.Parse<InputGlyphSize>( value, true );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !HasButton )
			return;
		Image.Texture = Input.GetGlyph( Button, ButtonSize, GlyphStyle.Knockout );
		if ( Image.ComputedStyle != null )
			Image.Style.Height = Image.ComputedStyle.FontSize ?? Image.Texture.Height;
		else
			Image.Style.Height = Image.Texture.Height;
		Image.Style.AspectRatio = (float)Image.Texture.Width / Image.Texture.Height;
	}
}
