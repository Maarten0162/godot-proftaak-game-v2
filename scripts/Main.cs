using Godot;
using System;
using System.Collections.Generic;
//hello

public partial class Main : Node2D
{

	// Dobbelsteen dobbelDefault = new Dobbelsteen(0, 4);
	// Dobbelsteen dobbelUpgrade1 = new Dobbelsteen(1, 4);
	// Dobbelsteen dobbelUpgrade2 = new Dobbelsteen(1, 7);
	// Dobbelsteen dobbelUpgrade3 = new Dobbelsteen(-3, 10);
	
	private Node2D player1;
	private Node2D player2;
	private Node2D player3;
	private Node2D player4;

	private AnimatedSprite2D dobbelSprite;

	private Random rnd = new Random();

	private List<Node2D> spaces = new List<Node2D>(); 

	private int player1CurrentSpace = 0; 

	int loc_pl1 = 0;
	int loc_pl2 = 0;
	int loc_pl3 = 0;
	int loc_pl4 = 0;

	int spacesAmount = 42;


	private bool isRolling = false;

	public override void _Ready()
	{	
		
	

		
		
		player1 = GetNode<Node2D>("players/player1");
		player2 = GetNode<Node2D>("players/player2");
		player3 = GetNode<Node2D>("players/player3");
		player4 = GetNode<Node2D>("players/player4");

		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");

	

		
		dobbelSprite.Play("0");
		
		for (int i = 1; i <= spacesAmount; i++)
		{
			spaces.Add(GetNode<Node2D>($"spaces/Marker2D{i}"));
		}
	}

	public override void _Process(double delta)
	{   
		if (Input.IsActionPressed("test1") && !isRolling)
		{

			isRolling = true;
			int diceRoll = defaultDice();



			player1CurrentSpace = player1CurrentSpace + diceRoll;

			if (player1CurrentSpace < 0)
			{
				player1CurrentSpace = player1CurrentSpace = 0;
			}
			
			if (player1CurrentSpace >= spaces.Count) {
				player1CurrentSpace = player1CurrentSpace - spaces.Count;
			}
			// Move player to the next space
			Node2D nextSpace = spaces[player1CurrentSpace];
			player1.Position = nextSpace.Position;
		}
		
		if (Input.IsActionPressed("test2") && !isRolling)
		{

			isRolling = true;
			int diceRoll = level2Dice();



			player1CurrentSpace = player1CurrentSpace + diceRoll;

			if (player1CurrentSpace < 0)
			{
				player1CurrentSpace = player1CurrentSpace = 0;
			}
			
			if (player1CurrentSpace >= spaces.Count) {
				player1CurrentSpace = player1CurrentSpace - spaces.Count;
			}
			// Move player to the next space
			Node2D nextSpace = spaces[player1CurrentSpace];
			player1.Position = nextSpace.Position;
		}

		if (Input.IsActionPressed("test3") && !isRolling)
		{

			isRolling = true;
			int diceRoll = level3Dice();



			player1CurrentSpace = player1CurrentSpace + diceRoll;

			if (player1CurrentSpace < 0)
			{
				player1CurrentSpace = player1CurrentSpace = 0;
			}
			
			if (player1CurrentSpace >= spaces.Count) {
				player1CurrentSpace = player1CurrentSpace - spaces.Count;
			}
			// Move player to the next space
			Node2D nextSpace = spaces[player1CurrentSpace];
			player1.Position = nextSpace.Position;
		}

		if (Input.IsActionPressed("test4") && !isRolling)
		{

			isRolling = true;
			int diceRoll = level4Dice();



			player1CurrentSpace = player1CurrentSpace + diceRoll;

			if (player1CurrentSpace < 0)
			{
				player1CurrentSpace = player1CurrentSpace = 0;
			}
			
			if (player1CurrentSpace >= spaces.Count) {
				player1CurrentSpace = player1CurrentSpace - spaces.Count;
			}
			// Move player to the next space
			Node2D nextSpace = spaces[player1CurrentSpace];
			player1.Position = nextSpace.Position;
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

	int defaultDice(){
		int antOgen = rnd.Next(0, 4);
		GD.Print("Dice result: " + antOgen);

		for(int i = -3; i <= 9; i ++){
			if(i == antOgen){
				dobbelSprite.Play($"{antOgen}");
			}
		}
		return antOgen;
	}

	int level2Dice(){
		int antOgen = rnd.Next(1, 4);
		GD.Print("Dice result: " + antOgen);

		for(int i = -3; i <= 9; i ++){
			if(i == antOgen){
				dobbelSprite.Play($"{antOgen}");
			}
		}
		return antOgen;
	}

	int level3Dice(){
		int antOgen = rnd.Next(1, 7);
		GD.Print("Dice result: " + antOgen);

		for(int i = -3; i <= 9; i ++){
			if(i == antOgen){
				dobbelSprite.Play($"{antOgen}");
			}
		}
		return antOgen;
	}


//dobbelsteen level 4 generate getall en zet sprite
	int level4Dice(){
		int antOgen = rnd.Next(-3, 10);
		GD.Print("Dice result: " + antOgen);

		for(int i = -3; i <= 9; i ++){
			if(i == antOgen){
				dobbelSprite.Play($"{antOgen}");
			}
		}
		return antOgen;
	}
}
