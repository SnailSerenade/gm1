using System.Collections.Generic;

namespace gm1;

/// <summary>
/// State of element instance
/// In general this means the severity of the element infliction
/// </summary>
public enum ElementState : int
{
	NEGATIVE_THREE = -3,
	NEGATIVE_TWO = -2,
	NEGATIVE_ONE = -1,
	NONE = 0,
	POSITIVE_ONE = 1,
	POSITIVE_TWO = 2,
	POSITIVE_THREE = 3
}

public partial class Element : Sandbox.BaseNetworkable
{
	public Element( ElementState state ) => State = state;
	public Element() => State = ElementState.NONE;

	/// <summary>
	/// Get the name of the element
	/// </summary>
	public string Name => GetType().Name;

	[Sandbox.Net]
	private ElementState InternalState { get; set; } = ElementState.NONE;

	/// <summary>
	/// State of element (read ElementState comment)
	/// </summary>
	public ElementState State
	{
		get => InternalState;
		set
		{
			if ( (int)value > 3 )
			{
				InternalState = (ElementState)3;
			}
			else if ( (int)value < -3 )
			{
				InternalState = (ElementState)(-3);
			}
			else
			{
				InternalState = value;
			}
		}
	}

	public Element UpdateState( Element element )
	{
		State += (int)element.State;
		return this;
	}

	/// <summary>
	/// Return as different element type
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <returns>This as element type</returns>
	public T To<T>() where T : Element, new() { return new T { State = State }; }
}

/// <summary>
/// Temperature element
/// Negative values == colder,
/// Positive values == hotter
/// </summary>
public class Temperature : Element
{
	public Temperature() : base()
	{
	}

	public Temperature( ElementState state ) : base( state )
	{
	}

	public bool IsFrozen => State == ElementState.NEGATIVE_THREE;
	public bool IsIcy => State <= ElementState.NEGATIVE_TWO ;
	public bool IsCold => State <= ElementState.NEGATIVE_ONE;

	public bool IsWarm => State >= ElementState.POSITIVE_ONE;
	public bool IsBurning => State >= ElementState.POSITIVE_TWO;
	public bool IsAblaze => State >= ElementState.POSITIVE_THREE;

	public static Temperature Frozen => new( ElementState.NEGATIVE_THREE );
	public static Temperature Icy => new( ElementState.NEGATIVE_TWO );
	public static Temperature Cold => new( ElementState.NEGATIVE_ONE );

	public static Temperature Warm => new( ElementState.POSITIVE_ONE );
	public static Temperature Burning => new( ElementState.POSITIVE_TWO );
	public static Temperature Ablaze => new( ElementState.POSITIVE_THREE );
}

/// <summary>
/// Physical element
/// Positive values == harder
/// </summary>
public class Physical : Element
{
	public Physical() : base()
	{
	}

	public Physical( ElementState state ) : base( state )
	{
	}

	public bool IsSoft => State >= ElementState.POSITIVE_ONE;
	public bool IsMedium => State >= ElementState.POSITIVE_TWO;
	public bool IsHard => State >= ElementState.POSITIVE_THREE;

	public static Temperature Soft => new( ElementState.POSITIVE_ONE );
	public static Temperature Medium => new( ElementState.POSITIVE_TWO );
	public static Temperature Hard => new( ElementState.POSITIVE_THREE );
}
