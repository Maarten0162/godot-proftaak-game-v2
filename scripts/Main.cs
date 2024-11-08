using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
//hello


public partial class Main : Node2D
{




	private AnimatedSprite2D dobbelSprite;



	private List<Node2D> spaces = new List<Node2D>();


	int spacesAmount = 42;
	Player player1;
	Player player2;
	Player player3;
	Player player4;


	private bool isRolling = false;
	Dice basicdice = new Dice(0, 4);
	Dice betterdice = new Dice(3, 7);
	Dice riskydice = new Dice(0, 7);
	Dice turbodice = new Dice(-3, 10);
	int diceRoll;
	Vector2[] Positions = new Vector2[42];
	public override void _Ready()
	{
		for (int i = 1; i <= spacesAmount; i++)
		{
			spaces.Add(GetNode<Node2D>($"spaces/Marker2D{i}"));

		}
		for (int i = 0; i < spacesAmount; i++)
		{
			Positions[i] = spaces[i].Position;

		}

		Vector2 topLeft = Positions[0];
		Vector2 topRight = Positions[9];
		Vector2 botLeft = Positions[21];
		Vector2 botRight = Positions[30];

		player1 = GetNode<Player>("player1");
		player1.Position = topLeft;
		player2 = GetNode<Player>("player2");
		player2.Position = topRight;
		player3 = GetNode<Player>("player3");
		player3.Position = botLeft;
		player4 = GetNode<Player>("player4");
		player4.Position = botRight;


		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");

		dobbelSprite.Play("0");


	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("test1") && !isRolling)
		{

			isRolling = true;
			diceRoll = basicdice.diceroll();
			GD.Print("Dice roll is = " + diceRoll);
			updateDobbelSprite(diceRoll);


		}
		if (Input.IsActionPressed("test2") && !isRolling)
		{

			isRolling = true;
			diceRoll = betterdice.diceroll();
			GD.Print("Dice roll is = " + diceRoll);

		}
		if (Input.IsActionPressed("test3") && !isRolling)
		{

			isRolling = true;
			diceRoll = riskydice.diceroll();
			GD.Print("Dice roll is = " + diceRoll);


		}
		if (Input.IsActionPressed("test4") && !isRolling)
		{

			isRolling = true;
			diceRoll = turbodice.diceroll();
			GD.Print("Dice roll is = " + diceRoll);


		}

		if (Input.IsActionJustReleased("test1"))
		{
			isRolling = false;
		}

		if (Input.IsActionJustReleased("test2"))
		{
			isRolling = false;
		}

		if (Input.IsActionJustReleased("test3"))
		{
			isRolling = false;
		}

		if (Input.IsActionJustReleased("test4"))
		{
			isRolling = false;
		}



	}

	void updateDobbelSprite(int inputDiceRoll)
	{
		for (int i = -3; i <= 9; i++)
		{
			if (i == inputDiceRoll)
			{
				dobbelSprite.Play($"{inputDiceRoll}");
			}

		}
	}
	void Movement()
	{
		
	}
}

