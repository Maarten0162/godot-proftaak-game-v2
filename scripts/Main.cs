using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;



public partial class Main : Node2D
{
	private int TurnCount;
	private AnimatedSprite2D dobbelSprite;

	int spacesAmount = 42;
	public Player player1;
	public Player player2;
	public Player player3;
	public Player player4;	
	Player [] Playerlist;
	
	
	[Signal]
	public delegate void PlayersReadyEventHandler(Player player);




	Random rnd = new Random();


	Dice basicdice = new Dice(0, 4);
	Dice betterdice = new Dice(3, 7);
	Dice riskydice = new Dice(0, 7);
	Dice turbodice = new Dice(-3, 10);
	Dice negdice = new Dice(-5, 0);
	Dice onedice = new Dice(1, 2);
	int diceRoll;
	private (Node2D Space, string Name, string OriginalName)[] spacesInfo;

	bool waitingforbuttonpress = true;
	bool ContinueLoop = true;
	int WhatPlayer;

	public override void _Ready()
	{
		spacesInfo = new (Node2D, string, string)[spacesAmount];
		for (int i = 1; i <= spacesAmount; i++)
		{
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");

			var sprite = markerNode.GetChild<Sprite2D>(0);
			spacesInfo[i - 1] = (markerNode, sprite.Name, sprite.Name);
			GD.Print("plek " + i + " is gevuld en de kleur is" + sprite.Name);

		}

		Vector2 topLeft = spacesInfo[0].Space.Position;
		Vector2 topRight = spacesInfo[9].Space.Position;
		Vector2 botLeft = spacesInfo[21].Space.Position;
		Vector2 botRight = spacesInfo[30].Space.Position;

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

			Playerlist = new Player[4] {player1, player2, player3, player4};



		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");

		dobbelSprite.Play("0");

		TurnLoop();
	}

	public override void _Process(double delta)
	{


	}
	//movement
	async Task StartMovement(Player player, int diceRoll)
	{	
		if (diceRoll >= 0)
		{
			await Movement(player, diceRoll);
		}
		else
		{
			await NegMovement(player, diceRoll);
		}
	}
	async Task Movement(Player player, int diceRoll)
	{	

		for (int i = 0; i < diceRoll && ContinueLoop; i++)
		{
			
						if(player.HasCap) //checkt of current speler de cap heeft
			{ GD.Print("in playerhascap check");
				for(int x = 0; x < Playerlist.Length; x++) // cycled door elke speler heen
				{
					if(player.PositionSpace + 2 == Playerlist[x].PositionSpace && Playerlist[x] != player) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{						
						RazorCapAttack(player, Playerlist[x]);

						ContinueLoop = false;
					}
				}
			}
			player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;
			
			player.Position = spacesInfo[player.PositionSpace].Space.Position;

			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}

	ContinueLoop = true;
	}
	async Task NegMovement(Player player, int diceRoll)
	{
		for (int i = 0; i > diceRoll; i--)
		{
			player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.
			player.Position = spacesInfo[player.PositionSpace].Space.Position;
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}


	}
	async Task Placedetection(string typeOfSpace, Player player)
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
		else if (typeOfSpace == "getRobbedSpace")
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
		else if (typeOfSpace == "robSpace")
		{
			int robbedAmount = RobSomeone(player);
			GD.Print("You just robbed someone! you gained " + robbedAmount + " Pounds!");
		}
		else if (typeOfSpace == "Razorcap_Space")
		{
			if (player.Currency >= 50)
			{
				await RazorcapPurchase(player);
			}
			else GD.Print("sorry " + player.Name + " you don't have enough pounds.");
		}

	}

	//TURNS
	async Task Turn()
	{

		await Turn_Test(player1);
		await Turn_Test(player2);
		await Turn_Test(player3);
		await Turn_Test(player4);
		TurnCount++;
		if (TurnCount % 5 == 0 || TurnCount == 1)
		{
			SpawnRazorCap();
		}
		GD.Print("einde van turn " + TurnCount + ". " + (TurnCount + 1) + " begint nu!");

	}
	async Task Turn_Test(Player player)
	{		WhatPlayer ++;
		GD.Print(player.Name + " Its your turn!");
		if (player.SkipTurn != true) // dit checkt of de speler zen beurt moet overslaan
		{	
			//choose wich dice, hiervoor hebben we de shop mechanic + een shop menu nodig

			diceRoll = await AwaitButtonPress(WhatPlayer); // ik kies nu betterdice maar dit moet dus eigenlijk gedaan worden via buttons in het menu? idk wrs kunenn we gwn doen A is dice 1, B is dice 2, X is dice 3 met kleine animatie.
			await StartMovement(player, diceRoll);
			if (diceRoll != 0)
			{       // zorgt ervoor dat als iemand 0  gooit de space niet nog een keer geactivate word, dat willen we niet.
				await Placedetection(spacesInfo[player.PositionSpace].Name, player);
			}
			EmitSignal("PlayersReady", player);
			GD.Print(player.Name + " staat op " +  spacesInfo[player.PositionSpace].Name + " na " + diceRoll + " te hebben gegooid.");

		}

		else
		{
			player.SkipTurn = false;// dit zorgt ervoor dat next turn deze speler wel dingen mag doen
			GD.Print(player.Name + " Had to skip his turn!");
		}
	}
	async Task<int> AwaitButtonPress(int PlayerNumber)
	{
		waitingforbuttonpress = true;
		while (waitingforbuttonpress)
		{		
 
			if (Input.IsActionJustPressed($"A_{PlayerNumber}"))
			{
				diceRoll = negdice.diceroll();
				updateDobbelSprite(diceRoll);

				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed($"B_{PlayerNumber}"))
			{
				diceRoll = betterdice.diceroll();
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"X_{PlayerNumber}"))
			{
				diceRoll = riskydice.diceroll();
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"Y_{PlayerNumber}"))
			{
				diceRoll = turbodice.diceroll();
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed("1"))
			{
				diceRoll = onedice.diceroll();
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
		GD.Print("GEEN BUTTON GEPRESSED, ERROR ERROR ERROR");
		return 0;
	}
	private async void TurnLoop()
	{

		while (true)
		{

			await Turn();

		}
	}

	//EVENTSPACES
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
		player.Position = spacesInfo[21].Space.Position;
		player.PositionSpace = 21;
	}
	void TopRightshortcut(Player player)
	{
		player.Position = spacesInfo[30].Space.Position;
		player.PositionSpace = 30;
	}
	void BottomLeftShortcut(Player player)
	{
		player.Position = spacesInfo[0].Space.Position;
		player.PositionSpace = 0;
	}
	void BottomRightShortcut(Player player)
	{
		player.Position = spacesInfo[9].Space.Position;
		player.PositionSpace = 9;
	}

	async Task RazorcapPurchase(Player player)
	{
		bool waitingforbuttonpressRazorcap = true;
		while (waitingforbuttonpressRazorcap)
		{
			if (Input.IsActionJustPressed("y"))
			{
				player.Currency -= 50;
				player.HasCap = true;
				waitingforbuttonpressRazorcap = false;
				spacesInfo[player.PositionSpace].Name = spacesInfo[player.PositionSpace].OriginalName;
				Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{player.PositionSpace + 1}");
				var sprite = markerNode.GetChild<Sprite2D>(0);
				sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[player.PositionSpace].OriginalName}.png");
				GD.Print(player.Name + " just bought the razor cap");

			}
			else if (Input.IsActionJustPressed("n"))
			{
				GD.Print("you don't want it? fuck off then");
				waitingforbuttonpressRazorcap = false;
			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");

		}


	}
	//misc
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

	void HorseRace(Player player)
	{
		if (player.Currency <= 5)
		{
			// Pop up text that says, sorry you dont have enough, better luck next time!!!
			GetNode<Label>("Control/HorseraceNomoney").Show();

		}
	}

	void RazorCapAttack(Player attacker, Player victim)
	{	
		int attackdamage = rnd.Next(40, 61);
		victim.Health -= attackdamage;
		attacker.HasCap = false;
		GD.Print(attacker.Name + " hit " + victim.Name + " for " + attackdamage + ". " + victim.Name + " has " + victim.Health + " health remaining");
	}
	void SpawnRazorCap()
	{

		int rndRazorCapSpace = rnd.Next(0, 42);
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{rndRazorCapSpace + 1}"); // het is + 1 omdat de markers 1 voorop lopen met de spaces tellen dan we in de index hebben staan

		var sprite = markerNode.GetChild<Sprite2D>(0);
		sprite.Texture = GD.Load<Texture2D>("res://assets/Spaces/RazorCap_Space.png");
		spacesInfo[rndRazorCapSpace].Name = "Razorcap_Space";
		GD.Print("razorcap ligt op vak " + rndRazorCapSpace);

	}
}

