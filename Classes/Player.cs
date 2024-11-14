using Godot;
using System;
using System.Dynamic;

public partial class Player : CharacterBody2D
{

	public Player()
	{
		currency = 100;
		health = 100;
		inventory = new string[3] {"0", "DoubleDice", "0"};
		rollAdjustment = 0;
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

    public string[] Items { get; internal set; }

}

