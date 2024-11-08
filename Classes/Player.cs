using Godot;
using System;

public partial class Player : CharacterBody2D
{

	public Player()
	{
		currency = 2;
		health = 100;
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
}

