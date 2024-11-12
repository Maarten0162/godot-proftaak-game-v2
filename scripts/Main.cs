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
	Player[] Playerlist;


	[Signal]
	public delegate void updateplayeruiEventHandler(Player player);


	Random rnd = new Random();

	private AudioStreamPlayer dobbelgeluid;


	Dice basicdice;
	Dice betterdice;
	Dice riskydice;
	Dice turbodice;
	Dice negdice;
	Dice onedice;
	int diceRoll;

	private (Node2D Space, string Name, string OriginalName)[] spacesInfo;
	private (string Name, int Price)[] Iteminfo; // hier gaan de namen van alle items in.
	private (string Name, int Price)[] ShopInv;
	bool waitingforbuttonpress;
	bool ContinueLoop;
	bool useItem;
	string ChosenItem;
	string itemId;
	int WhatPlayer;
	public override void _Ready()
	{
		dobbelgeluid = GetNode<AudioStreamPlayer>("Dobbelgeluid");

		// Initialiseer de dobbelstenen met het dobbelgeluid
		basicdice = new Dice(0, 4, dobbelgeluid);
		betterdice = new Dice(3, 7, dobbelgeluid);
		riskydice = new Dice(0, 7, dobbelgeluid);
		turbodice = new Dice(-3, 10, dobbelgeluid);
		negdice = new Dice(-5, 0, dobbelgeluid);
		onedice = new Dice(1, 2, dobbelgeluid);

		spacesInfo = new (Node2D, string, string)[spacesAmount];
		for (int i = 1; i <= spacesAmount; i++)
		{
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");
			var sprite = markerNode.GetChild<Sprite2D>(0);
			spacesInfo[i - 1] = (markerNode, sprite.Name, sprite.Name);
			GD.Print("plek " + i + " is gevuld en de kleur is" + sprite.Name);
		}

		// Initieer de posities van de spelers
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

		Playerlist = new Player[4] { player1, player2, player3, player4 };

		Iteminfo = new (string Name, int Price)[15] { ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10) };
		ShopInv = new(string Name, int Price)[3] { ("test", 10), ("test", 10), ("test", 10)};
		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");
		dobbelSprite.Play("0");

		TurnLoop();
	}


	public override void _Process(double delta)
	{


	}
	//movement
	async Task StartMovement(Player player, int diceRoll, int PlayerNumber)
	{
		if (diceRoll >= 0)
		{
			await Movement(player, diceRoll, PlayerNumber);
		}
		else
		{
			await NegMovement(player, diceRoll, WhatPlayer);
		}
	}
	async Task Movement(Player player, int diceRoll, int PlayerNumber)
	{
		bool hasattacked = false;
		ContinueLoop = true;
		for (int i = 0; i < diceRoll && ContinueLoop; i++)
		{
			int spaceinfront = (player.PositionSpace + 1) % spacesInfo.Length;

			if (player.HasCap) //checkt of current speler de cap heeft
			{

				GD.Print("in playerhascap check");
				for (int x = 0; x < Playerlist.Length; x++) // cycled door elke speler heen zolang de speler nog dicerolls heeft 
				{

					//15 en 36 zijn hetzelfde vak
					if (spaceinfront == 15 || spaceinfront == 36)
					{
						int Otherspace;
						if (spaceinfront == 15)
						{
							Otherspace = 36;
						}
						else Otherspace = 15;
						if (Otherspace == Playerlist[x].PositionSpace && i != diceRoll) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
						{
							GD.Print("naar razorattack");

							RazorCapAttack(player, Playerlist[x]);
							hasattacked = true;

							ContinueLoop = false;
						}
					}
					if (spaceinfront == Playerlist[x].PositionSpace && i != diceRoll) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{
						GD.Print("naar razorattack");

						RazorCapAttack(player, Playerlist[x]);
						hasattacked = true;

						ContinueLoop = false;
					}

				}
			}
			if (spaceinfront == 3 && hasattacked == false || spaceinfront == 28 && hasattacked == false)
			{ 	bool hasitemspace = false;
				for(int x = 0; x < 3; x++){
				switch(player.Inventory[x])
				{
					case "0": 	
						hasitemspace = true;
						break;
					
				}}
				if(hasitemspace)
				{
					 await ShopAsk(player, PlayerNumber);
				}
			}
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}


	}
	async Task NegMovement(Player player, int diceRoll, int PlayerNumber)
	{
		bool hasattacked = false;
		ContinueLoop = true;
		int spaceBehind = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length;
		for (int i = 0; i > diceRoll && ContinueLoop; i--)
		{

			for (int x = 0; x < Playerlist.Length; x++) // cycled door elke speler heenzolang de speler nog dicerolls heeft
			{

				if (spaceBehind == 15 || spaceBehind == 36)
				{
					int Otherspace;
					if (spaceBehind == 15)
					{
						Otherspace = 36;
					}
					else Otherspace = 15;
					if (Otherspace == Playerlist[x].PositionSpace && Playerlist[x].HasCap && i != diceRoll) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{
						GD.Print("naar razorattack");

						RazorCapAttack(Playerlist[x], player);
						hasattacked = true;
						ContinueLoop = false;
					}
				}

				if (spaceBehind == Playerlist[x].PositionSpace && Playerlist[x].HasCap && i != diceRoll) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, en de andere speler heeft een cap hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
				{
					RazorCapAttack(Playerlist[x], player);
					hasattacked = true;
					ContinueLoop = false;
				}
			}

			if (spaceBehind == 2 && hasattacked == false || spaceBehind == 27 && hasattacked)
			{
				await ShopAsk(player, PlayerNumber);
			}
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}

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
				await RazorcapPurchase(player, WhatPlayer);
			}
			else GD.Print("sorry " + player.Name + " you don't have enough pounds.");
		}

	}

	//TURNS
	async Task Turn()
	{
		if (player1.Health != 0)
		{
			await Turn_Player(player1);
		}
		if (player2.Health != 0)
		{
			await Turn_Player(player2);
		}
		if (player3.Health != 0)
		{
			await Turn_Player(player3);
		}
		if (player4.Health != 0)
		{
			await Turn_Player(player4);
		}

		TurnCount++;
		if (CheckWinCondition()) //functie checkt of alle spelers op 1 na dood zijn, of dat er 15 turns voorbij zijn gegaan.
		{
			EndGame();
		}

		if (TurnCount >= 0) //dit zorgt ervoor dat de cap gaat spawnen
		{
			bool RunLoop = true;
			while (RunLoop)
			{
				for (int i = 0; i < Playerlist.Length; i++)
				{
					if (Playerlist[i].HasCap)
					{
						RunLoop = false; //checkt of iemand de cap heeft, zoja spawnt de cap niet
					}
				}
				for (int i = 0; i < spacesInfo.Length; i++)
				{
					if (spacesInfo[i].Name == "Razorcap_Space")
					{
						RunLoop = false; //checkt of de map nog te kopen is op de map, zoja spawnt de cap niet
					}
				}
				if (RunLoop)
				{
					SpawnRazorCap();
					RunLoop = false;
				}

			}
		}
		GD.Print("einde van turn " + TurnCount + ". " + (TurnCount + 1) + " begint nu!");
		WhatPlayer = 0;
	}
	async Task Turn_Player(Player player)
	{
		WhatPlayer++;
		GD.Print(player.Name + " Its your turn!");
		if (player.SkipTurn == false) // dit checkt of de speler zen beurt moet overslaan
		{
			//choose wich dice, hiervoor hebben we de shop mechanic + een shop menu nodig
			// await ChooseUseItem(player, WhatPlayer);
			diceRoll = await AwaitButtonPress(WhatPlayer); // ik kies nu betterdice maar dit moet dus eigenlijk gedaan worden via buttons in het menu? idk wrs kunenn we gwn doen A is dice 1, B is dice 2, X is dice 3 met kleine animatie.
			await StartMovement(player, diceRoll, WhatPlayer);
			if (diceRoll != 0)
			{       // zorgt ervoor dat als iemand 0  gooit de space niet nog een keer geactivate word, dat willen we niet.
				await Placedetection(spacesInfo[player.PositionSpace].Name, player);
			}
			EmitSignal("updateplayerui", player);
			if (player.Health > 0)
			{
				GD.Print(player.Name + " staat op " + spacesInfo[player.PositionSpace].Name + " na " + diceRoll + " te hebben gegooid.");
			}

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

			if (Input.IsActionJustPressed($"A_{PlayerNumber}") || Input.IsActionJustPressed("2"))
			{
				// diceRoll = negdice.diceroll(); 
				diceRoll = 2;
				updateDobbelSprite(diceRoll);

				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed($"B_{PlayerNumber}") || Input.IsActionJustPressed("3"))
			{
				// diceRoll = betterdice.diceroll();
				diceRoll = 3;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"X_{PlayerNumber}") || Input.IsActionJustPressed("4"))
			{
				// diceRoll = riskydice.diceroll();
				diceRoll = -1;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"Y_{PlayerNumber}") || Input.IsActionJustPressed("5"))
			{
				// diceRoll = turbodice.diceroll();
				diceRoll = -2;
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

	async Task RazorcapPurchase(Player player, int PlayerNumber)
	{
		bool waitingforbuttonpressRazorcap = true;
		GD.Print("Do you want to buy the razorcap for 50 pounds? Press left bumper for yes, right bumper for no");
		while (waitingforbuttonpressRazorcap)
		{
			if (Input.IsActionJustPressed($"yes_{PlayerNumber}"))
			{
				player.Currency -= 50;
				player.HasCap = true;
				spacesInfo[player.PositionSpace].Name = spacesInfo[player.PositionSpace].OriginalName;
				Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{player.PositionSpace + 1}");
				var sprite = markerNode.GetChild<Sprite2D>(0);
				sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[player.PositionSpace].OriginalName}.png");
				GD.Print(player.Name + " just bought the razor cap");
				waitingforbuttonpressRazorcap = false;

			}
			else if (Input.IsActionJustPressed($"no_{PlayerNumber}"))
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
		int attackdamage = 100;
		victim.Health -= attackdamage;
		attacker.HasCap = false;
		GD.Print(attacker.Name + " hit " + victim.Name + " for " + attackdamage + ". " + victim.Name + " has " + victim.Health + " health remaining");
		EmitSignal("updateplayerui", attacker);
		EmitSignal("updateplayerui", victim);
	}
	void SpawnRazorCap()
	{

		int rndRazorCapSpace = rnd.Next(0, 42);
		if (rndRazorCapSpace == 15 || rndRazorCapSpace == 36) //voor het middelste vak, waar de cap niet mag komen.
		{
			rndRazorCapSpace += 1;
		}
		for (int i = 0; i < Playerlist.Length; i++)
		{
			if (rndRazorCapSpace == Playerlist[i].PositionSpace)
			{
				rndRazorCapSpace += 1;
			}
		}
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{rndRazorCapSpace + 1}"); // het is + 1 omdat de markers 1 voorop lopen met de spaces tellen dan we in de index hebben staan

		var sprite = markerNode.GetChild<Sprite2D>(0);
		sprite.Texture = GD.Load<Texture2D>("res://assets/Spaces/RazorCap_Space.png");
		spacesInfo[rndRazorCapSpace].Name = "Razorcap_Space";
		GD.Print("razorcap ligt op vak " + rndRazorCapSpace);

	}

	bool CheckWinCondition()
	{
		int deadplayer = 0;
		for (int i = 0; i < Playerlist.Length; i++)
		{
			if (Playerlist[i].Health == 0)
			{
				deadplayer += 1;
			}
		}
		if (deadplayer == Playerlist.Length - 1 || TurnCount == 15)
		{
			return true;
		}
		else return false;
	}
	void EndGame()
	{

	}

	async Task ShopAsk(Player player, int PlayerNumber)
	{
		bool RunLoop = true;
		if (player.Currency > 0)
		{
			GD.Print("do you want to shop for items here? Left bumper for YES, right bumper for NO");
			while (RunLoop)
			{
				if (Input.IsActionJustPressed($"yes_{PlayerNumber}")) //yes i want to shop
				{
					GD.Print("Okay, come on in");
					await GenerateShopInv(player, PlayerNumber);
					RunLoop = false;
				}
				else if (Input.IsActionJustPressed($"no_{PlayerNumber}")) //no i dont want to shop
				{
					GD.Print("Okay, fuck off then");
					RunLoop = false;
				}
				await ToSignal(GetTree().CreateTimer(0), "timeout");
			}


		}
	}
	async Task GenerateShopInv(Player player, int PlayerNumber)
	{
		GD.Print("Shop inv generated");
		List<int> randomList = new List<int>();
		
		bool runloop = true;

		while (runloop)
		{
			int rnditem = rnd.Next(0, 5);
			if (!randomList.Contains(rnditem))
			{
				randomList.Add(rnditem);
			}
			if (randomList.Count() == 3)
			{	GD.Print(randomList[0] + " " + randomList[1] +" " + randomList[2] );

				for (int i = 0; i < 3; i++)
				{
					ShopInv[i] = (Iteminfo[randomList[i]].Name, Iteminfo[randomList[i]].Price); //pakt 3 random getallen en kiest 3 items.
					runloop = false;
				}

				await Shop(ShopInv, player, PlayerNumber);

			}
		}

	}
	async Task Shop((string Name, int Price)[] Shopinv, Player player, int PlayerNumber)
	{
		GD.Print("Welcome to the shop, these are my wares: " + Shopinv[0] + ", " + Shopinv[1] + " ," + Shopinv[2]);
		bool runloop = true;
		string ItemConfirm = "X";
		while (runloop)
		{
			bool runLoop2 = true;
			string ChosenItem = "0";
			while (runLoop2)
			{
				if (Input.IsActionJustPressed($"D-Pad-left_{PlayerNumber}"))
				{
					if (player.Currency >= Shopinv[0].Price)
						ChosenItem = Shopinv[0].Name;
					GD.Print("chose item: " + ChosenItem);
					runLoop2 = false;
				}
				if (Input.IsActionJustPressed($"D-Pad-up_{PlayerNumber}"))
				{
					if (player.Currency >= Shopinv[1].Price)
						ChosenItem = Shopinv[1].Name;
					GD.Print("chose item: " + ChosenItem);
					runLoop2 = false;
				}
				if (Input.IsActionJustPressed($"D-Pad-right_{PlayerNumber}"))
				{
					if (player.Currency >= Shopinv[2].Price)
						ChosenItem = Shopinv[2].Name;
					GD.Print("chose item: " + ChosenItem);
					runLoop2 = false;
				}
				await ToSignal(GetTree().CreateTimer(0), "timeout");

			}
			bool runLoop3 = true;

			while (runLoop3)
			{
				if (Input.IsActionJustPressed($"B_{PlayerNumber}") || Input.IsActionJustPressed($"yes_{PlayerNumber}"))
				{
					runloop = false;
					runLoop3 = false;
					ItemConfirm = ChosenItem;
					GD.Print(player.Name + "has chose item " + ItemConfirm);

				}
				if (Input.IsActionJustPressed($"A_{PlayerNumber}") || Input.IsActionJustPressed($"no_{PlayerNumber}"))
				{
					runLoop3 = false;
					GD.Print("choose another item");

				}
				await ToSignal(GetTree().CreateTimer(0), "timeout");
			}
		}
		bool runloop4 = true;
		for (int i = 0; i <= 2 && runloop4; i++)
		{
			if (player.Inventory[i] != "0")
			{
				player.Inventory[i] = ItemConfirm;
				runloop4 = false;
			}
		}




	}
	async Task DoubleDice(Player player)
	{
		int eyecount1 = rnd.Next(1, 7);
		int eyecount2 = rnd.Next(1, 7);
		int eyecountTotal = eyecount1 + eyecount1;
		await StartMovement(player, eyecountTotal, WhatPlayer);
	}

	async Task ChooseUseItem(Player player, int PlayerNumber)
	{
		bool RunLoop = true;
		GD.Print("in choose use item");
		while (RunLoop)
		{
			if (Input.IsActionJustPressed($"yes_{PlayerNumber}"))
			{
				GD.Print("said yes");
				await ChooseItem(player, PlayerNumber);
				RunLoop = false;
			}
			else if (Input.IsActionJustPressed($"no_{PlayerNumber}"))
			{
				GD.Print("said no");
				RunLoop = false;
			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
	}
	async Task ChooseItem(Player player, int PlayerNumber)
	{
		GD.Print("in Choose item");
		useItem = true;
		while (useItem)
		{
			if (Input.IsActionJustPressed($"D-Pad-left_{PlayerNumber}"))
			{
				itemId = player.Inventory[0];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");

						break;
					case "DoubleDice":
						await DoubleDice(player);
						GD.Print("Used item a Double Dice, it has vanished from their inventory.");

						break;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;

				}
				player.Inventory[0] = "0";

			}
			if (Input.IsActionJustPressed($"D-Pad-up_{PlayerNumber}"))
			{
				itemId = player.Inventory[1];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");

						break;
					case "DoubleDice":
						await DoubleDice(player);
						GD.Print("Used item a Double Dice, it has vanished from their inventory.");

						break;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;
				}
				player.Inventory[1] = "0";

			}
			if (Input.IsActionJustPressed($"D-Pad-right_{PlayerNumber}"))
			{
				itemId = player.Inventory[2];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");

						break;
					case "DoubleDice":
						await DoubleDice(player);
						GD.Print("Used item a Double Dice, it has vanished from their inventory.");

						break;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;
				}
				player.Inventory[2] = "0";

			}
			if (Input.IsActionJustPressed($"D-Pad-down_{PlayerNumber}"))
			{
				useItem = false;

			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
	}
}

