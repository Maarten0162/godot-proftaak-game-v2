using Godot;
using System;
//hello
public partial class Player : CharacterBody2D
{

	public Player()
	{
		currency = 10;
		health = 100;
		playerPosition = new Vector2(0, 0);
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
	private Vector2 playerPosition;
	public Vector2 PlayerPosition
	{
		get
		{
			return playerPosition;
		}
		set
		{
			playerPosition = value;
			// Update the actual node's position to reflect the new position
			this.Position = playerPosition;  // This updates the Position of the CharacterBody2D
		}

	}
	public void Move(Vector2 newPosition)
	{
		Position = newPosition;  // Update both the stored position and the node's position
	}
}

