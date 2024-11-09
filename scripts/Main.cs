using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;



public partial class Main : Node2D
{

	private AnimatedSprite2D dobbelSprite;

	int spacesAmount = 42;
	Player player1;
	Player player2;
	Player player3;
	Player player4;

	Random rnd = new Random();

	private bool isRolling = false;
	Dice basicdice = new Dice(0, 4);
	Dice betterdice = new Dice(3, 7);
	Dice riskydice = new Dice(0, 7);
	Dice turbodice = new Dice(-3, 10);
	Dice negdice = new Dice(-5, 0);
	Dice onedice = new Dice(1, 2);
	int diceRoll;
	Vector2[] SpaceCoords = new Vector2[42];
	private (Node2D Position, string Name)[] spacesInfo;

	public override void _Ready()
	{
		spacesInfo = new (Node2D, string)[spacesAmount];
		for (int i = 1; i <= spacesAmount; i++)
		{
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");

			var sprite = markerNode.GetChild<Sprite2D>(0);
			spacesInfo[i - 1] = (markerNode, sprite.Name);
			GD.Print("plek " + i + " is gevuld en de kleur is" + sprite.Name);

		}
		for (int i = 0; i < spacesAmount; i++)
		{
			SpaceCoords[i] = spacesInfo[i].Item1.Position;
		}



		Vector2 topLeft = SpaceCoords[0];
		Vector2 topRight = SpaceCoords[9];
		Vector2 botLeft = SpaceCoords[21];
		Vector2 botRight = SpaceCoords[30];

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

	public override async void _Process(double delta)
	{	if(Input.IsActionJustPressed("test1"))
	{
		diceRoll = negdice.diceroll();
		updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			await StartMovement(player1, diceRoll);
	}
	if(Input.IsActionJustPressed("test2"))
	{
		diceRoll = betterdice.diceroll();
		updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			await StartMovement(player1, diceRoll);
	}
	if(Input.IsActionJustPressed("test3"))
	{
		diceRoll = riskydice.diceroll();
		updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			await StartMovement(player1, diceRoll);
	}
	if(Input.IsActionJustPressed("test4"))
	{
		diceRoll = turbodice.diceroll();
		updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			await StartMovement(player1, diceRoll);
	}
	if(Input.IsActionJustPressed("1"))
	{
		diceRoll = onedice.diceroll();
		updateDobbelSprite(diceRoll);
			GD.Print("Dice roll is = " + diceRoll);
			await StartMovement(player1, diceRoll);
	}

	}
	async Task StartMovement(Player player, int diceRoll)
	{
		if (diceRoll >= 0)
		{
			await Movement(player1, diceRoll);
		}
		else
		{
			await NegMovement(player1, diceRoll);
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
	async Task Movement(Player player, int diceRoll)
	{

		for (int i = 0; i < diceRoll; i++)
		{
			player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;
			player.Position = SpaceCoords[player.PositionSpace];
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}
		Placedetection(spacesInfo[player.PositionSpace].Item2, player);
		GD.Print(player.PositionSpace + spacesInfo[player.PositionSpace].Item2 + " na movement");
		GD.Print(player.Name + " currency is now: " + player.Currency);

	}
	async Task NegMovement(Player player, int diceRoll)
	{
		for (int i = 0; i > diceRoll; i--)
		{	
			player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.
			player.Position = SpaceCoords[player.PositionSpace];
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}
		Placedetection(spacesInfo[player.PositionSpace].Item2, player);
		GD.Print(player.PositionSpace + spacesInfo[player.PositionSpace].Item2 + " na movement");
		GD.Print(player.Name + " currency is now: " + player.Currency);

	}
	void Placedetection(string typeOfSpace, Player player)
	{
		if (typeOfSpace == "blueSpace")
		{
			BlueSpace(player);
		}
		else if (typeOfSpace == "redSpace")
		{
			RedSpace(player);
		}
		else if (typeOfSpace.Contains("sc"))
		{
			GD.Print("You found a shortcut!");
			if (typeOfSpace.Contains("top"))
			{
				if (typeOfSpace.Contains("Left"))
				{
					TopLeftshortcut(player);
				}
				else TopRightshortcut(player);
			}
			else if (typeOfSpace.Contains("Left"))
			{

				BottomLeftShortcut(player);

			}
			else BottomRightShortcut(player);
		}
		else if (typeOfSpace == "robSpace")
		{
			int robbedAmount = Robbery(player);
			if (robbedAmount > player.Currency)
			{
				GD.Print("They took every penny you had!");
			}
			else
				GD.Print("You just got robbed! You lost " + robbedAmount + " pounds!!");
		}
		else if (typeOfSpace == "knockoutSpace")
		{
			SkipNextTurn(player);
			GD.Print("You just got knocked out! you have to skip a turn");
		}
		else if (typeOfSpace == "getRobbedSpace")
		{
			int robbedAmount = RobSomeone(player);
			GD.Print("You just robbed someone! you gained " + robbedAmount + " Pounds!");
		}


	}

	void BlueSpace(Player player)
	{
		player.Currency += 3;
	}
	void RedSpace(Player player)
	{
		player.Currency -= 3;
	}
	int Robbery(Player player)
	{
		int lostcurrency = rnd.Next(8, 31);
		player.Currency -= lostcurrency;
		player.Health -= 10;
		return lostcurrency;
	}
	int RobSomeone(Player player)
	{
		int gainedCurrency = rnd.Next(5, 21);
		player.Currency += gainedCurrency;
		return gainedCurrency;
	}

	void SkipNextTurn(Player player)
	{
		player.SkipTurn = true;
	}

	void TopLeftshortcut(Player player)
	{
		player.Position = SpaceCoords[21];
		player.PositionSpace = 21;
	}
	void TopRightshortcut(Player player)
	{
		player.Position = SpaceCoords[30];
		player.PositionSpace = 30;
	}
	void BottomLeftShortcut(Player player)
	{
		player.Position = SpaceCoords[0];
		player.PositionSpace = 0;
	}
	void BottomRightShortcut(Player player)
	{
		player.Position = SpaceCoords[9];
		player.PositionSpace = 9;
	}

	void HorseRace(Player player)
	{
		if (player.Currency <= 5)
		{
			// Pop up text that says, sorry you dont have enough, better luck next time!!!
			GetNode<Label>("Control/HorseraceNomoney").Hide();

		}
	}
	void Turn_order_Test()
	{
		Turn_Test(player1);
		Turn_Test(player2);
		Turn_Test(player3);
		Turn_Test(player4);
	}
	async void Turn_Test(Player player)
	{
		if(player.SkipTurn != true) // dit checkt of de speler zen beurt moet overslaan
		{
			//choose wich dice, hiervoor hebben we de shop mechanic + een shop menu nodig
			diceRoll = betterdice.diceroll(); // ik kies nu betterdice maar dit moet dus eigenlijk gedaan worden via buttons in het menu? idk wrs kunenn we gwn doen A is dice 1, B is dice 2, X is dice 3 met kleine animatie.
			await StartMovement(player, diceRoll); // 
		}
		else player.SkipTurn = false; // dit zorgt ervoor dat next turn deze speler wel dingen mag doen
	}








}

