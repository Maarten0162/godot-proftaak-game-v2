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
	public Button buttonmin1;
	public Button buttonmin2;
	public Button buttonplus1;
	public Button buttonplus2;
	
	public Player player1;
	public Player player2;
	public Player player3;
	public Player player4;
	public Player[] Playerlist;


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
	Dice twentydice;
	Dice tendice;
	int diceRoll;

	private (Node2D Space, string Name, string OriginalName)[] spacesInfo;
	private (string Name, int Price)[] Iteminfo; // hier gaan de namen van alle items in.
	private (string Name, int Price)[] ShopInv;
	private int[] MiniGames;
	bool waitingforbuttonpress;
	bool ContinueLoop;
	bool useItem;
	string ChosenItem;
	string itemId;
	int WhatPlayer;
	public int PlayerAmount;
	public override void _Ready()
	{
		dobbelgeluid = GetNode<AudioStreamPlayer>("Dobbelgeluid");

		// Initialiseer de dobbelstenen met het dobbelgeluid.
		basicdice = new Dice(0, 4, dobbelgeluid, 0);
		betterdice = new Dice(3, 7, dobbelgeluid, 7);
		riskydice = new Dice(0, 7, dobbelgeluid, 5);
		turbodice = new Dice(-3, 10, dobbelgeluid, 7);
		negdice = new Dice(-5, 0, dobbelgeluid, 0);
		onedice = new Dice(1, 2, dobbelgeluid, 0);
		tendice = new Dice(0, 11, dobbelgeluid, 0);
		twentydice = new Dice(-20, 21, dobbelgeluid, 0);

		spacesInfo = new (Node2D, string, string)[spacesAmount];
		for (int i = 1; i <= spacesAmount; i++)
		{
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");
			var sprite = markerNode.GetChild<Sprite2D>(0);
			spacesInfo[i - 1] = (markerNode, sprite.Name, sprite.Name);
			GD.Print("plek " + i + " is gevuld en de kleur is" + sprite.Name);
		}
		int PlayerAmount = GlobalVariables.Instance.playeramount;

		if(PlayerAmount == 2)
		{				Vector2 player1start = spacesInfo[0].Space.Position;
				player1 = GetNode<Player>("player1");
				player1.Position = player1start;
				player1.PositionSpace = 0;
						Vector2 player2start = spacesInfo[30].Space.Position;
				player2 = GetNode<Player>("player2");
				player2.Position = player2start;
				player2.PositionSpace = 30;				
				
				Playerlist = new Player[2] { player1, player2};
		for (int i = 0; i < Playerlist.Length; i++)
		{
			EmitSignal("updateplayerui", Playerlist[i]);
		}
				
				
		}
		else if(PlayerAmount == 3)
		{
			Vector2 player1start = spacesInfo[0].Space.Position;
				player1 = GetNode<Player>("player1");
				player1.Position = player1start;
				player1.PositionSpace = 0;
						Vector2 player2start = spacesInfo[9].Space.Position;
				player2 = GetNode<Player>("player2");
				player2.Position = player2start;
				player2.PositionSpace = 9;	
								Vector2 player3start = spacesInfo[30].Space.Position;
				player3 = GetNode<Player>("player3");
				player3.Position = player3start;
				player3.PositionSpace = 30;
				Playerlist = new Player[3] { player1, player2, player3};
		for (int i = 0; i < Playerlist.Length; i++)
		{
			EmitSignal("updateplayerui", Playerlist[i]);
		}
		}
		else if(PlayerAmount == 4)
		{	GD.Print("in 4 spelers");
		
		
			Vector2 topLeft = spacesInfo[0].Space.Position;
			player1 = GetNode<Player>("player1");
		player1.Position = topLeft;
		player1.PositionSpace = 0;

		Vector2 topRight = spacesInfo[9].Space.Position;
		player2 = GetNode<Player>("player2");
		player2.Position = topRight;
		player2.PositionSpace = 9;

		Vector2 botLeft = spacesInfo[21].Space.Position;
		player3 = GetNode<Player>("player3");
		player3.Position = botLeft;
		player3.PositionSpace = 21;

		Vector2 botRight = spacesInfo[30].Space.Position;
		player4 = GetNode<Player>("player4");
		player4.Position = botRight;
		player4.PositionSpace = 30;
		
		Playerlist = new Player[4] { player1, player2, player3, player4 };
		for (int i = 0; i < Playerlist.Length; i++)
		{
			EmitSignal("updateplayerui", Playerlist[i]);
		}
		}
		

		

		Iteminfo = new (string Name, int Price)[15] { ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10), ("test", 10) };
		ShopInv = new (string Name, int Price)[3] { ("test", 10), ("test", 10), ("test", 10) };

		MiniGames = new int[10];
		for (int i = 0; i < 9; i++)
		{
			MiniGames[i] = i;
		}

		dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");
		dobbelSprite.Play("0");
		
		buttonmin1 = new Button();
		buttonmin2 = new Button();
		buttonplus1 = new Button();
		buttonplus2 = new Button();
		
		ChooseTurn();
		
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
		bool hasattacked = false;
		ContinueLoop = true;
		for (int i = 0; i < diceRoll && ContinueLoop; i++)
		{
			int spaceinfront = (player.PositionSpace + 1) % spacesInfo.Length;

			if (player.HasCap || player.HasKnuckles || player.HasGoldenKnuckles) //checkt of current speler de cap heeft
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
						if (Otherspace == Playerlist[x].PositionSpace && i != diceRoll && player != Playerlist[x]) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
						{

							if (player.HasKnuckles || player.HasGoldenKnuckles)
							{
								KnucklesAttack(player, Playerlist[x]);
							}
							else if(player.HasCap){
							RazorCapAttack(player, Playerlist[x]);
							hasattacked = true;
							}
							ContinueLoop = false;
						}
					}
					if (spaceinfront == Playerlist[x].PositionSpace && i != diceRoll && player != Playerlist[x]) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{

						if (player.HasKnuckles || player.HasGoldenKnuckles)
						{
							KnucklesAttack(player, Playerlist[x]);
						}
						else if(player.HasCap){
						RazorCapAttack(player, Playerlist[x]);
						hasattacked = true;
						}
						ContinueLoop = false;
					}

				}
			}
			if ((spaceinfront == 6 || spaceinfront == 24) && hasattacked == false) //SHOP REGISTRATIE
			{

				bool hasitemspace = false;
				for (int x = 0; x < 3; x++)
				{
					switch (player.Inventory[x])
					{
						case "0":
							hasitemspace = true;
							break;

					}
				}
				if (hasitemspace)
				{
					await ShopAsk(player);
				}
			}			
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}
			if(spacesInfo[player.PositionSpace].Name == "bearTrap_Space")
			{
				await BearTrapHit(player);
				ContinueLoop = false;
			}			
			if (spacesInfo[player.PositionSpace].Name == "Razorcap_Space" && i != diceRoll)
			{
				if (player.Currency >= 50)
				{
					await RazorcapPurchase(player);
				}
				else GD.Print("sorry " + player.Name + " you don't have enough pounds.");
			}
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
		}


	}
	async Task NegMovement(Player player, int diceRoll)
	{
		bool hasattacked = false;
		ContinueLoop = true;

		for (int i = 0; i > diceRoll && ContinueLoop; i--)
		{
			int spaceBehind = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length;

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
					if (Otherspace == Playerlist[x].PositionSpace && i != diceRoll && player != Playerlist[x] && (Playerlist[x].HasCap || Playerlist[x].HasKnuckles)) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{
						if (player.HasKnuckles|| player.HasKnuckles)
						{
							KnucklesAttack(player, Playerlist[x]);
						}
						else if(player.HasCap){
						RazorCapAttack(Playerlist[x], player);
						hasattacked = true;
						}
						ContinueLoop = false;
					}
				}

				if (spaceBehind == Playerlist[x].PositionSpace && Playerlist[x].HasCap && i != diceRoll && player != Playerlist[x] && (Playerlist[x].HasCap || Playerlist[x].HasKnuckles)) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, en de andere speler heeft een cap hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
				{
					if (player.HasKnuckles)
					{
						KnucklesAttack(player, Playerlist[x]);
					}
					else if(player.HasCap){
					RazorCapAttack(Playerlist[x], player);
					hasattacked = true;
					}
					ContinueLoop = false;
					
				}
			}

			if ((spaceBehind == 5 || spaceBehind == 23) && !hasattacked) // SHOP
			{

				bool hasitemspace = false;
				for (int x = 0; x < 3; x++)
				{
					switch (player.Inventory[x])
					{
						case "0":
							hasitemspace = true;
							break;

					}
				}
				if (hasitemspace)
				{
					await ShopAsk(player);
				}
			}
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}
			if (spacesInfo[player.PositionSpace].Name == "Razorcap_Space" && i != diceRoll)
			{
				if (player.Currency >= 50)
				{
					await RazorcapPurchase(player);
				}
				else GD.Print("sorry " + player.Name + " you don't have enough pounds.");
			}
			if(spacesInfo[player.PositionSpace].Name == "bearTrap_Space")
			{
				await BearTrapHit(player);
				ContinueLoop = false;
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
		await Task.CompletedTask;

	}

	//TURNS
	async Task Turn4()
	{
		if (player1.Health != 0)
		{
			await Turn_Player(player1);
		}
		if (player2.Health != 0&& player4 != null)
		{
			await Turn_Player(player2);
		}
		if (player3.Health != 0&& player3 != null)
		{
			await Turn_Player(player3);
		}
		if (player4.Health != 0&& player4 != null)
		{
			await Turn_Player(player4);
		}

		TurnCount++;

		if (CheckWinCondition()) //functie checkt of alle spelers op 1 na dood zijn, of dat er 15 turns voorbij zijn gegaan.
		{
			EndGame();
		}
		// ChooseMiniGame();
		await selectTrapPositon();
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
		async Task Turn3()
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


		TurnCount++;

		if (CheckWinCondition()) //functie checkt of alle spelers op 1 na dood zijn, of dat er 15 turns voorbij zijn gegaan.
		{
			EndGame();
		}
		// ChooseMiniGame();
		await selectTrapPositon();
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
	async Task Turn2()
	{
		if (player1.Health != 0)
		{
			await Turn_Player(player1);
		}
		if (player2.Health != 0)
		{
			await Turn_Player(player2);
		}


		TurnCount++;

		if (CheckWinCondition()) //functie checkt of alle spelers op 1 na dood zijn, of dat er 15 turns voorbij zijn gegaan.
		{
			EndGame();
		}
		// ChooseMiniGame();

		await selectTrapPositon();
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

			string useditem = await ChooseUseItem(player);
			diceRoll += player.RollAdjustment;			
			if (useditem != "dice")
			{
				diceRoll = await AwaitButtonPress(player); // ik kies nu betterdice maar dit moet dus eigenlijk gedaan worden via buttons in het menu? idk wrs kunenn we gwn doen A is dice 1, B is dice 2, X is dice 3 met kleine animatie.

				await StartMovement(player, diceRoll);
			}

			if (diceRoll != 0)// zorgt ervoor dat als iemand 0  gooit de space niet nog een keer geactivate word, dat willen we niet.
			{
				player.RollAdjustment = 0; //dit zorgt ervoor dat volgende turn deze geen roll buff of debuff heeft.
				await Placedetection(spacesInfo[player.PositionSpace].Name, player);
			}

			EmitSignal("updateplayerui", player);

			if (player.Health > 0)
			{
				GD.Print(player.Name + " staat op " + spacesInfo[player.PositionSpace].Name + player.PositionSpace + " na " + diceRoll + " te hebben gegooid.");
			}
			else
			{
				player.Hide();
			}
		}



		player.SkipTurn = false;// dit zorgt ervoor dat next turn deze speler wel dingen mag doen
		GD.Print(player.Name + " Had to skip his turn!");


	}
	async Task<int> AwaitButtonPress(Player player)
	{
		waitingforbuttonpress = true;
		while (waitingforbuttonpress)
		{

			if (Input.IsActionJustPressed($"A_{WhatPlayer}"))
			{
				diceRoll = basicdice.diceroll();
				player.Currency -= basicdice.Price;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed($"B_{WhatPlayer}"))
			{
				diceRoll = betterdice.diceroll();
				player.Currency -= betterdice.Price;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"X_{WhatPlayer}"))
			{
				diceRoll = riskydice.diceroll();
				player.Currency -= riskydice.Price;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed($"Y_{WhatPlayer}"))
			{
				diceRoll = turbodice.diceroll();
				player.Currency -= turbodice.Price;
				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;

			}
			else if (Input.IsActionJustPressed("1"))
			{
				diceRoll = 1;

				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed("2"))
			{
				diceRoll = 2;

				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed("3"))
			{
				diceRoll = 3;

				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed("4"))
			{
				diceRoll = -1;

				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			else if (Input.IsActionJustPressed("5"))
			{
				diceRoll = -2;

				updateDobbelSprite(diceRoll);
				waitingforbuttonpress = false;
				return diceRoll;
			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
		GD.Print("GEEN BUTTON GEPRESSED, ERROR ERROR ERROR");
		return 0;
	}
	private async void ChooseTurn()
	{
		if(PlayerAmount == 4){
		while (true)
		{GD.Print("playeramount is 4");

			await Turn();

		}}
			if(PlayerAmount == 3){
		while (true)
		{

			await Turn3();

		}}
			if(PlayerAmount == 2){
		while (true)
		{

			await Turn2();

		}}
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
		GD.Print("Do you want to buy the razorcap for 50 pounds? Press left bumper for yes, right bumper for no");
		while (waitingforbuttonpressRazorcap)
		{
			if (Input.IsActionJustPressed($"yes_{WhatPlayer}"))
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
			else if (Input.IsActionJustPressed($"no_{WhatPlayer}"))
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
	void KnucklesAttack(Player attacker, Player victim)
	{
		int attackdamage = 15;
		victim.Health -= attackdamage;
		attacker.HasKnuckles = false;
		GD.Print(attacker.Name + " hit " + victim.Name + " for " + attackdamage + ". " + victim.Name + " has " + victim.Health + " health remaining");
		EmitSignal("updateplayerui", attacker);
		if (victim.Health == 0)
		{
			victim.Hide();
		}
		EmitSignal("updateplayerui", victim);
	}

	void RazorCapAttack(Player attacker, Player victim)
	{
		int attackdamage = rnd.Next(40, 61);
		victim.Health -= attackdamage;
		attacker.HasCap = false;
		GD.Print(attacker.Name + " hit " + victim.Name + " for " + attackdamage + ". " + victim.Name + " has " + victim.Health + " health remaining");
		EmitSignal("updateplayerui", attacker);
		if (victim.Health == 0)
		{
			victim.Hide();
		}
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

	async Task ShopAsk(Player player)
	{
		bool RunLoop = true;
		if (player.Currency > 0)
		{
			GD.Print("do you want to shop for items here? Left bumper for YES, right bumper for NO");
			while (RunLoop)
			{
				if (Input.IsActionJustPressed($"yes_{WhatPlayer}")) //yes i want to shop
				{
					GD.Print("Okay, come on in");
					await GenerateShopInv(player);
					RunLoop = false;
				}
				else if (Input.IsActionJustPressed($"no_{WhatPlayer}")) //no i dont want to shop
				{
					GD.Print("Okay, fuck off then");
					RunLoop = false;
				}
				await ToSignal(GetTree().CreateTimer(0), "timeout");
			}


		}
	}
	async Task GenerateShopInv(Player player)
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
			{
				GD.Print(randomList[0] + " " + randomList[1] + " " + randomList[2]);

				for (int i = 0; i < 3; i++)
				{
					ShopInv[i] = (Iteminfo[randomList[i]].Name, Iteminfo[randomList[i]].Price); //pakt 3 random getallen en kiest 3 items.
					runloop = false;
				}

				await Shop(ShopInv, player);

			}
		}

	}
	async Task Shop((string Name, int Price)[] Shopinv, Player player)
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
				if (Input.IsActionJustPressed($"D-Pad-left_{WhatPlayer}"))
				{
					if (player.Currency >= Shopinv[0].Price)
						ChosenItem = Shopinv[0].Name;
					GD.Print("chose item: " + ChosenItem);
					runLoop2 = false;
				}
				if (Input.IsActionJustPressed($"D-Pad-up_{WhatPlayer}"))
				{
					if (player.Currency >= Shopinv[1].Price)
						ChosenItem = Shopinv[1].Name;
					GD.Print("chose item: " + ChosenItem);
					runLoop2 = false;
				}
				if (Input.IsActionJustPressed($"D-Pad-right_{WhatPlayer}"))
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
				if (Input.IsActionJustPressed($"B_{WhatPlayer}") || Input.IsActionJustPressed($"yes_{WhatPlayer}"))
				{
					runloop = false;
					runLoop3 = false;
					ItemConfirm = ChosenItem;
					GD.Print(player.Name + "has chose item " + ItemConfirm);

				}
				if (Input.IsActionJustPressed($"A_{WhatPlayer}") || Input.IsActionJustPressed($"no_{WhatPlayer}"))
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



	async Task<string> ChooseUseItem(Player player)
	{
		string useditem;
		bool RunLoop = true;
		GD.Print("in choose use item");
		while (RunLoop)
		{
			if (Input.IsActionJustPressed($"yes_{WhatPlayer}"))
			{
				GD.Print("said yes");
				useditem = await ChooseItem(player);

				return useditem;
				
			}
			else if (Input.IsActionJustPressed($"no_{WhatPlayer}"))
			{
				GD.Print("said no");

				return "nouseditem";
			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
		return "nouseditem";
	}
	async Task<string> ChooseItem(Player player)
	{
		string useditem = "";
		GD.Print("in Choose item");
		useItem = true;
		while (useItem)
		{
			if (Input.IsActionJustPressed($"D-Pad-left_{WhatPlayer}"))
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
						useditem = "dice";
						player.Inventory[0] = "0";
						return useditem;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;

				}

			}
			if (Input.IsActionJustPressed($"D-Pad-up_{WhatPlayer}"))
			{
				itemId = player.Inventory[1];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");

						break;
					case "DoubleDice":
						GD.Print("Used item a Double Dice, it has vanished from their inventory.");
						await DoubleDice(player);
						useditem = "dice";
						player.Inventory[1] = "0";
						return useditem;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;
				}

			}
			if (Input.IsActionJustPressed($"D-Pad-right_{WhatPlayer}"))
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
						useditem = "dice";
						player.Inventory[2] = "0";
						return useditem;
					case "2":
						// Example effect for item 2
						GD.Print("test");

						break;
						
				}

			}
			if (Input.IsActionJustPressed($"D-Pad-down_{WhatPlayer}"))
			{
				useItem = false;

			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
		return useditem;
	}
	//items en item spaces
	void Whiskey(Player player) //!ITEM SPACE player verliest currency EN moet een turn overslaan
	{
		GD.Print("you found a bottle of whiskey and drank it all");
		player.SkipTurn = true;
		player.Currency -= player.Currency / 3;
		GD.Print("You got too drunk and went on a spending spree! also you have to skip your next turn because of your hangover");
	}
	void GoldenPipe(Player player) //**KAN ALLEEN AANGEVRAAGD WORDEN ALS DE razorcapspace op het bord zit!
	{
		for (int i = 0; i < spacesAmount; i++)
		{
			if (spacesInfo[i].Name == "Razorcap_Space")
			{
				if (i == 0)
				{
					player.Position = spacesInfo[41].Space.Position;
					player.PositionSpace = 41;
				}
				else
				{
					player.Position = spacesInfo[i - 1].Space.Position;
					player.PositionSpace = i - 1;
				}
			}
		}
	}
	async Task DoubleDice(Player player) //2X 1-6 dice
	{
		int eyecount1 = rnd.Next(1, 7);
		int eyecount2 = rnd.Next(1, 7);
		diceRoll = eyecount1 + eyecount2;
		await StartMovement(player, diceRoll);

	}
	async Task TripleDice(Player player) // 3x 1-6 dice
	{
		int eyecount1 = riskydice.diceroll();
		int eyecount2 = riskydice.diceroll();
		int eyecount3 = riskydice.diceroll();
		diceRoll = eyecount1 + eyecount2 + eyecount3;
		await StartMovement(player, diceRoll);
	}
	async Task TwentyDice(Player player) //dice van -20 tot +20
	{
		diceRoll = twentydice.diceroll();
		await StartMovement(player, diceRoll);

	}
	async Task TenDice(Player player) //rolt 1 dice van 0-10
	{
		diceRoll = tendice.diceroll();
		await StartMovement(player, diceRoll);
	}
	void DashMushroom(Player player) // doet Plus X bij deze speler zijn volgende dice roll
	{
		player.RollAdjustment += 5;
	}
	void TeleportTorndPlayer(Player player) //teleport to a random player
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, Playerlist.Length);
			if (player != Playerlist[rndplayer])
			{
				player.Position = Playerlist[rndplayer].Position;
				player.PositionSpace = Playerlist[rndplayer].PositionSpace;
				runloop = false;
			}
		}

	}
	void SwitchPlaces(Player player) // switch places with a random player
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, Playerlist.Length);
			if (player != Playerlist[rndplayer])
			{
				Vector2 originalposition = player.Position;
				int originalpositionspace = player.PositionSpace;

				player.Position = Playerlist[rndplayer].Position;
				player.PositionSpace = Playerlist[rndplayer].PositionSpace;

				Playerlist[rndplayer].Position = originalposition;
				Playerlist[rndplayer].PositionSpace = originalpositionspace;
				runloop = false;
			}
		}
	}

	void StealPlayerCap(Player player) //** jat de cap van de speler die hem heeft, moet een check zijn of iemand de cap heeft En of je het zelf niet bent. De victim neemt ook 10 damage TENZIJ hij daarvan dood zou gaan, dan niet.
	{
		string victim = "";
		bool runloop = true;
		while (runloop)
		{
			for (int i = 0; i < Playerlist.Length; i++)
			{
				if (Playerlist[i].HasCap)
				{
					if (Playerlist[i].Health > 10)
					{
						Playerlist[i].Health -= 10;
					}
					Playerlist[i].HasCap = false;
					victim = Playerlist[i].Name;
					runloop = false;
				}
			}
		}
		GD.Print(player.Name + "used his goons to steal te cap from: " + victim);
	}
	void PoisonMushroom(Player player)//geeft een random player een roll debuff next turn
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, Playerlist.Length);
			if (player != Playerlist[rndplayer])
			{
				Playerlist[rndplayer].RollAdjustment -= 5;
				runloop = false;
			}
		}

	}
	void StealCoins(Player player) // steelt currency tussen 1 en de helft van een random persoon;
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, Playerlist.Length);
			if (player != Playerlist[rndplayer])
			{
				int stolenamount = rnd.Next(0, Playerlist[rndplayer].Currency / 5);
				player.Currency += stolenamount;
			}
		}
	}
	void BrassKnuckles(Player player) //de player krijgt brass knuckles, een mini razor cap die minder damage doet maar waar je ook niet stopt nadat je aangevallen hebt, kan in combinatie met de razorcap
	{
		player.HasKnuckles = true;
	}
	void StealItem(Player player) // **steal een random item van een random player, MOET WEL EEN CHECK ZIJN OF ER EEN SPELER IS MET EEN ITEM!!!
	{
		bool runloop1 = true;
		int howmanyitems = 0;
		while (runloop1)
		{
			int rndplayer = rnd.Next(0, Playerlist.Length);
			if (player != Playerlist[rndplayer])
			{
				for (int i = 0; i < 2; i++)
				{
					if (Playerlist[rndplayer].Inventory[i] != "0") //gaat van de gekozen speler zen inv af, om te checken of hij wel items heeft
					{
						howmanyitems++;
					}
				}
				if (howmanyitems != 0) // de gekozen speler heeft items
				{
					bool runloop2 = true;
					for (int i = 0; i <= 2 && runloop2; i++)
					{
						if (player.Inventory[i] != "0")
						{
							player.Inventory[i] = Playerlist[rndplayer].Inventory[rnd.Next(0, 3)];
							runloop1 = false;
							runloop2 = false;
						}
					}
				}
			}

		}


	}
	void GoldenKnuckles(Player player) // oldenknuckles, knuckles die niet kapot gaan.
	{
		player.HasGoldenKnuckles = true;
	}	

	// void ChooseMiniGame()
	// {
	// 	int Whatminigame = rnd.Next(1, MiniGames.Length);
	// 	GetTree().ChangeSceneToFile($"pathtoscene{Whatminigame}");//hier pakt hij 1 van de minigames die net random is gekozen.
	// }

		async Task selectTrapPositon() 
	{	GD.Print("in buttonselect");
		
		
		
   		AddChild(buttonmin1);
		AddChild(buttonmin2);
		AddChild(buttonplus1);
		AddChild(buttonplus2);
		
		buttonmin1.ZIndex = 3;
		buttonmin2.ZIndex = 3;
		buttonplus1.ZIndex = 3;
		buttonplus2.ZIndex = 3;
		
		buttonmin1.Text = "-1";
		buttonmin2.Text = "-2";
		buttonplus1.Text = "+1";
		buttonplus2.Text = "+2";
		
		buttonmin1.Pressed += ()=> beartrapamount(-1);
		buttonmin2.Pressed += ()=> beartrapamount(-2);
		buttonplus1.Pressed += ()=> beartrapamount(1);
		buttonplus2.Pressed += ()=> beartrapamount(2);
		
		buttonmin1.Position = new Vector2(501, 318);
		buttonmin2.Position = new Vector2(471, 318);
		buttonplus1.Position = new Vector2(561, 318);	
		buttonplus2.Position = new Vector2(591, 318);
		
		
		buttonmin1.Show();
		buttonmin2.Show();
		buttonplus1.Show();
		buttonplus2.Show();
	}
	
	
	private void beartrapamount(int bearTrapSpaceAmount) 
	{
		GD.Print(bearTrapSpaceAmount);
		int bearTrapSpace = player1.PositionSpace + bearTrapSpaceAmount;
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{bearTrapSpace + 1}"); // het is + 1 omdat de markers 1 voorop lopen met de spaces tellen dan we in de index hebben staan

		var sprite = markerNode.GetChild<Sprite2D>(0);
		GD.Print("test");
		sprite.Texture = GD.Load<Texture2D>("res://assets/Spaces/bearTrap_Space.png");
		spacesInfo[bearTrapSpace].Name = "bearTrap_Space";
		GD.Print(spacesInfo[bearTrapSpace].Name);
		GD.Print("Beartrap ligt op vak " + bearTrapSpace);
		buttonmin1.Hide();			
		buttonmin2.Hide();
		buttonplus1.Hide();
		buttonplus2.Hide();
		
		buttonmin1.ZIndex = 0;
		buttonmin2.ZIndex = 0;
		buttonplus1.ZIndex = 0;
		buttonplus2.ZIndex = 0;
		
	}
	
	async Task BearTrapHit(Player player) 
	{
		player.Health -= 20;
		player.SkipTurn = true;	
			spacesInfo[player.PositionSpace].Name = spacesInfo[player.PositionSpace].OriginalName;
				Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{player.PositionSpace + 1}");
				var sprite = markerNode.GetChild<Sprite2D>(0);
				sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[player.PositionSpace].OriginalName}.png");
			
		
	}

}

