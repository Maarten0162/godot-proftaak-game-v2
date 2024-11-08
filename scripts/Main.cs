using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;



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
	Dice negdice = new Dice (-5,0);
	int diceRoll;
	Vector2[] Positions = new Vector2[42];
	private (Node2D Position, string Name)[] spacesInfo;


	public override void _Ready()
	{	spacesInfo = new (Node2D, string)[spacesAmount];
		for (int i = 1; i <= spacesAmount; i++)
		{	
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");

			var sprite = markerNode.GetChild<Sprite2D>(0);			
			spacesInfo[i -1] = ( markerNode, sprite.Name);
			GD.Print("plek " + i + " is gevuld en de kleur is" + sprite.Name);
			// spaces.Add(GetNode<Node2D>($"spaces/Marker2D{i}"));
		}
		for (int i = 0; i < spacesAmount; i++)
		{
			Positions[i] = spacesInfo[i].Item1.Position;
		}

   

		Vector2 topLeft = Positions[0];
		Vector2 topRight = Positions[9];
		Vector2 botLeft = Positions[21];
		Vector2 botRight = Positions[30];

		player1 = GetNode<Player>("player1");
		player1.Position = topLeft;
		player1.PositionSpace = 0;
		player2 = GetNode<Player>("player2");
		player2.Position = topRight;
		player2.PositionSpace = 9;
		player3 = GetNode<Player>("player3");
		player3.Position = botLeft;
		player3.PositionSpace = 21;
		player4 = GetNode<Player>("player4");
		player4.Position = botRight;
		player4.PositionSpace = 30;



		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");

		dobbelSprite.Play("0");


	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("test1") && !isRolling)
		{
			isRolling = true;
			diceRoll = negdice.diceroll();
		}
		if (Input.IsActionPressed("test2") && !isRolling)
		{
			isRolling = true;
			diceRoll = betterdice.diceroll();
		}
		if (Input.IsActionPressed("test3") && !isRolling)
		{
			isRolling = true;
			diceRoll = riskydice.diceroll();
		}
		if (Input.IsActionPressed("test4") && !isRolling)
		{
			isRolling = true;
			diceRoll = turbodice.diceroll();
		}


		if (Input.IsActionJustReleased("test1") ||
			Input.IsActionJustReleased("test2") ||
			Input.IsActionJustReleased("test3") ||
			Input.IsActionJustReleased("test4"))
		{
					
			updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			if(diceRoll >= 0)
			{
				Movement(player1, diceRoll);
			}
			else
			{
				NegMovement(player1, diceRoll);
			}
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
	async void Movement(Player player, int diceRoll)
	{

		for(int i = 0; i < diceRoll; i++)
		{ 
		 player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;
		 player.Position = Positions[player.PositionSpace];
		 await ToSignal(GetTree().CreateTimer(0.4), "timeout");	
		 GD.Print(player.PositionSpace);		
		}

		
		GD.Print(player.PositionSpace +  spacesInfo[player.PositionSpace].Item2 + " na movement");
		
	}
	async void NegMovement(Player player, int diceRoll)
		{
   			for(int i = 0; i > diceRoll; i--)
		{ 
		 player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.
		 player.Position = Positions[player.PositionSpace];
		 await ToSignal(GetTree().CreateTimer(0.4), "timeout");	
		 GD.Print(player.PositionSpace);		
		}

		
		GD.Print(player.PositionSpace +  spacesInfo[player.PositionSpace].Item2 + " na movement");
	}
}

