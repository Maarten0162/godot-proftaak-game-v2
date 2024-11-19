using Godot;
using System;
using System.Dynamic;

public partial class Player : CharacterBody2D
{

	public Player()
	{
		currency = 100;
		health = 20;
		inventory = new string[3] {"Whiskey", "TripleDice", "PoisonMushroom"};
		rollAdjustment = 0;
		inventory = new string[3] {"0", "TripleDice", "PoisonMushroom"};

	}

	private int currency;

	public int Currency
	{
		get
		{
			return currency;
		}
		set
		{
			currency = value;
			if(currency < 0)
			{
				currency = 0;
			}
		}
	}
	private int health;

	public int Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if(health < 0)
			{
				health = 0;
			}
		}
	}

	private int positionSpace;

	public int PositionSpace
	{
		get
		{
			return positionSpace;
		}
		set
		{
			positionSpace = value;			
		}
	}
	private bool skipTurn;

	public bool SkipTurn
	{
		get
		{
			return skipTurn;
		}
		set
		{
			skipTurn = value;
		}
	}
		private bool hasCap;

	public bool HasCap
	{
		get
		{
			return hasCap;
		}
		set
		{
			hasCap = value;
		}
	}
	private string[] inventory;
	
	public string[] Inventory
	{
		get
		{
			return inventory;
		}
		set
		{
			inventory = value;
		}
	}
	private int rollAdjustment;
	public int RollAdjustment
	{
		get
		{
			return rollAdjustment;
		}
		set
		{
			rollAdjustment = value;
		}
	}
			private bool hasknuckles;

	public bool HasKnuckles
	{
		get
		{
			return hasknuckles;
		}
		set
		{
			hasknuckles = value;
		}
	}
				private bool hasgoldenknuckles;

	public bool HasGoldenKnuckles
	{
		get
		{
			return hasgoldenknuckles;
		}
		set
		{
			hasgoldenknuckles = value;
		}
	}

   public PlayerState SavePlayerState()
    {
        return new PlayerState(this);
    }

    // You can also implement a load method to restore the state
    public void LoadPlayerState(PlayerState state)
    {
        Position = state.Position;
        PositionSpace = state.PositionSpace;
        Health = state.Health;
        Currency = state.Currency;
        SkipTurn = state.SkipTurn;
        HasCap = state.HasCap;
        HasKnuckles = state.HasKnuckles;
        HasGoldenKnuckles = state.HasGoldenKnuckles;
        Inventory = state.Inventory;
        RollAdjustment = state.RollAdjustment;

       
    }}

