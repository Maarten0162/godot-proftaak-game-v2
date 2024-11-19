using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;



public partial class Main : Node2D
{


	int lostcurrency;
	int gainedCurrency;
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

	TextureRect invSprite1;
	TextureRect invSprite2;
	TextureRect invSprite3;

	Label turnLabel;

	Vector2 invSprite1Play1Pos;
	Vector2 invSprite2Play1Pos;
	Vector2 invSprite3Play1Pos;

	Vector2 invSprite1Play2Pos;
	Vector2 invSprite2Play2Pos;
	Vector2 invSprite3Play2Pos;


	Vector2 invSprite1Play3Pos;
	Vector2 invSprite2Play3Pos;
	Vector2 invSprite3Play3Pos;


	Vector2 invSprite1Play4Pos;
	Vector2 invSprite2Play4Pos;
	Vector2 invSprite3Play4Pos;


	Control mainShop;
	Control textShop;


	Panel invSlot1;
	Panel invSlot2;
	Panel invSlot3;

	TextureRect turnDpadIcon1;
	TextureRect turnDpadIcon2;
	TextureRect turnDpadIcon3;

	TextureRect dpadIcon1;
	TextureRect dpadIcon2;
	TextureRect dpadIcon3;


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
	public List<Player> playersalive;

	bool waitingforbuttonpress;
	bool ContinueLoop;
	bool useItem;
	string ChosenItem;
	string itemId;
	int WhatPlayer;
	public int PlayerAmount;

	Vector2 Sprite1OldPos;
	Vector2 Sprite2OldPos;
	Vector2 Sprite3OldPos;
	Label Spacelabel;
	Vector2 Slot1OldPos;
	Vector2 Slot2OldPos;
	Vector2 Slot3OldPos;


	public override void _Ready()
	{
		mainShop = GetNode<Control>("CanvasLayersshop/TextureRectRounded");
		textShop = GetNode<Control>("CanvasLayersshop/WelcomeScreen");

		Spacelabel = GetNode<Label>("CanvasLayerspaces/SpaceLabel");
		invSprite1Play1Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player1/Playerhud/item1").Position;
		invSprite2Play1Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player1/Playerhud/item2").Position;
		invSprite3Play1Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player1/Playerhud/item3").Position;

		invSprite1Play2Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player2/Playerhud/item1").Position;
		invSprite2Play2Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player2/Playerhud/item2").Position;
		invSprite3Play2Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player2/Playerhud/item3").Position;

		invSprite1Play3Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player3/Playerhud/item1").Position;
		invSprite2Play3Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player3/Playerhud/item2").Position;
		invSprite3Play3Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player3/Playerhud/item3").Position;

		invSprite1Play4Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player4/Playerhud/item1").Position;
		invSprite2Play4Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player4/Playerhud/item2").Position;
		invSprite3Play4Pos = GetNode<TextureRect>($"Node2D/CanvasLayer/player4/Playerhud/item3").Position;

		turnLabel = GetNode<Label>("Node2D/CanvasLayer/Label");

		turnLabel.Text = "Speler1 is aan de beurt, kies een item om te gebruiken";


		//zet players invisble worden visible in Updatehud()
		GetNode<Node2D>($"Node2D/CanvasLayer/player1").Hide();
		GetNode<Node2D>($"Node2D/CanvasLayer/player2").Hide();
		GetNode<Node2D>($"Node2D/CanvasLayer/player3").Hide();
		GetNode<Node2D>($"Node2D/CanvasLayer/player4").Hide();

		textShop.Hide();
		mainShop.Hide();

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


		if (GlobalVariables.Instance.TurnCount == 0)
		{
			GlobalVariables.Instance.playersalive = new List<Player>();
			for (int i = 1; i <= spacesAmount; i++)
			{
				Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i}");
				var sprite = markerNode.GetChild<Sprite2D>(0);
				spacesInfo[i - 1] = (markerNode, sprite.Name, sprite.Name);
				GD.Print("plek " + i + " is gevuld en de kleur is" + spacesInfo[i - 1].Name);
			}
		}
		int PlayerAmount = GlobalVariables.Instance.playeramount;

		if (GlobalVariables.Instance.TurnCount > 0)
		{
			GD.Print("in niet ronde 1");


			for (int i = 0; i < spacesAmount; i++)
			{
				Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i + 1}");
				spacesInfo[i].Space = markerNode;
			}

		}

		if (PlayerAmount == 2)
		{
			Vector2 player1start = spacesInfo[0].Space.Position;
			player1 = GetNode<Player>("player1");
			player1.Position = player1start;
			player1.PositionSpace = 0;
			Vector2 player2start = spacesInfo[5].Space.Position;
			player2 = GetNode<Player>("player2");
			player2.Position = player2start;
			player2.PositionSpace = 5;

			Playerlist = new Player[2] { player1, player2 };
			playersalive = new List<Player> { player1, player2 };



		}
		else if (PlayerAmount == 3)
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
			Playerlist = new Player[3] { player1, player2, player3 };
			playersalive = new List<Player> { player1, player2, player3 };

		}
		else if (PlayerAmount == 4)
		{
			GD.Print("in 4 spelers");


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
			playersalive = new List<Player> { player1, player2, player3, player4 };

		}
		GlobalVariables.Instance.playersalive = playersalive;
		if (GlobalVariables.Instance.TurnCount > 0)
		{
			RestoreAllStates();
			playersalive[GlobalVariables.Instance.Winner].Currency += 30;
		}

		for (int i = 0; i < Playerlist.Length; i++)
		{
			if (Playerlist[i].Health == 0)
			{
				Playerlist[i].Hide();
			}




		}

		for (int i = 0; i < playersalive.Count; i++)
		{
			Updatehud(playersalive[i]);
		}
		GD.Print($"Players Alive: {GlobalVariables.Instance.playersalive.Count}");
		for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
		{
			GD.Print($"Player {i}: {GlobalVariables.Instance.playersalive[i].Name}");
		}





		Iteminfo = new (string Name, int Price)[15] { ("Whiskey", 20), ("GoldenPipe", 20), ("DoubleDice", 10), ("TripleDice", 13), ("TwentyDice", 20), ("TenDice", 8), ("DashMushroom", 5), ("TeleportTorndPlayer", 15), ("SwitchPlaces", 15), ("StealPlayerCap", 40), ("PoisonMushroom", 5), ("StealCoins", 30), ("BrassKnuckles", 20), ("GoldenKnuckles", 50), ("BearTrap", 40) };
		ShopInv = new (string Name, int Price)[3] { ("test", 10), ("test", 10), ("test", 10) };



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


				for (int x = 0; x < playersalive.Count; x++) // cycled door elke speler heen zolang de speler nog dicerolls heeft 
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
						if (Otherspace == playersalive[x].PositionSpace && i != diceRoll && player != playersalive[x]) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
						{

							if (player.HasKnuckles || player.HasGoldenKnuckles)
							{
								KnucklesAttack(player, playersalive[x]);
							}
							else if (player.HasCap)
							{
								RazorCapAttack(player, playersalive[x]);
								hasattacked = true;
								ContinueLoop = false;
							}

						}
					}
					if (spaceinfront == playersalive[x].PositionSpace && i != diceRoll && player != playersalive[x]) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{

						if (player.HasKnuckles || player.HasGoldenKnuckles)
						{
							KnucklesAttack(player, playersalive[x]);
						}
						else if (player.HasCap)
						{
							RazorCapAttack(player, playersalive[x]);
							hasattacked = true;
							ContinueLoop = false;
						}

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
					UpdateSpaceLabel("Shop", player);
					await ShopAsk(player);
				}
			}
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace + 1) % spacesInfo.Length;

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}
			if (spacesInfo[player.PositionSpace].Name == "bearTrap_Space")
			{
				Beartrap(player);
				ContinueLoop = false;
			}
			if (spacesInfo[player.PositionSpace].Name == "RazorCap_Space" && i != diceRoll)
			{
				if (player.Currency >= 50)
				{
					UpdateSpaceLabel("RazorCappurchase", player);
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

			for (int x = 0; x < playersalive.Count; x++) // cycled door elke speler heenzolang de speler nog dicerolls heeft
			{

				if (spaceBehind == 15 || spaceBehind == 36)
				{
					int Otherspace;
					if (spaceBehind == 15)
					{
						Otherspace = 36;
					}
					else Otherspace = 15;
					if (Otherspace == playersalive[x].PositionSpace && i != diceRoll && player != playersalive[x] && (playersalive[x].HasCap || playersalive[x].HasKnuckles)) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
					{
						if (player.HasKnuckles || player.HasKnuckles)
						{
							KnucklesAttack(player, playersalive[x]);
						}
						else if (player.HasCap)
						{
							RazorCapAttack(playersalive[x], player);
							hasattacked = true;
							ContinueLoop = false;
						}

					}
				}

				if (spaceBehind == playersalive[x].PositionSpace && playersalive[x].HasCap && i != diceRoll && player != playersalive[x] && (playersalive[x].HasCap || playersalive[x].HasKnuckles)) //als currenct speler en een andere speler dezelfde positie hebben EN het is niet dezelfde speler, en de andere speler heeft een cap hij checkt 2 posities voor zich omdat hij checkt voordat hij beweegt, als je checkt nadat hij beweegt en de speler gooit 1 dan werkt het niet
				{
					if (player.HasKnuckles)
					{
						KnucklesAttack(player, playersalive[x]);
					}
					else if (player.HasCap)
					{
						RazorCapAttack(playersalive[x], player);
						hasattacked = true;
						ContinueLoop = false;
					}


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
				{	UpdateSpaceLabel("Shop", player);
					await ShopAsk(player);
				}
			}
			if (ContinueLoop)
			{
				player.PositionSpace = (player.PositionSpace - 1 + spacesInfo.Length) % spacesInfo.Length; //dit zorgt voor de wrap around, dat hij door kan als hij bij het aan het einde aankomt.

				player.Position = spacesInfo[player.PositionSpace].Space.Position;
			}
			if (spacesInfo[player.PositionSpace].Name == "RazorCap_Space" && i != diceRoll)
			{
				if (player.Currency >= 50)
				{
					UpdateSpaceLabel("RazorCappurchase", player);
					await RazorcapPurchase(player);
				}
				else GD.Print("sorry " + player.Name + " you don't have enough pounds.");
			}
			if (spacesInfo[player.PositionSpace].Name == "bearTrap_Space")
			{
				Beartrap(player);
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
			UpdateSpaceLabel("blueSpace", player);

		}
		else if (typeOfSpace == "redSpace")
		{
			RedSpace(player);
			UpdateSpaceLabel("redSpace", player);
		}
		else if (typeOfSpace.Contains("sc"))
		{
			GD.Print("You found a shortcut!");
			if (typeOfSpace.Contains("top"))
			{
				if (typeOfSpace.Contains("Left"))
				{
					UpdateSpaceLabel("TopLeftShortcut", player);
					TopLeftshortcut(player);
				}
				else { TopRightshortcut(player); UpdateSpaceLabel("TopRightShortcut", player); }
			}
			else if (typeOfSpace.Contains("Left"))
			{

				BottomLeftShortcut(player);
				UpdateSpaceLabel("BottomLeftShortcut", player);
			}
			else { BottomRightShortcut(player); UpdateSpaceLabel("TopRightShortcut", player); }
		}
		else if (typeOfSpace == "getRobbedSpace")
		{
			int robbedAmount = Robbery(player);
			UpdateSpaceLabel("Robbery", player);
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
			UpdateSpaceLabel("KnockoutSpace", player);
			GD.Print("You just got knocked out! you have to skip a turn");
		}
		else if (typeOfSpace == "robSpace")
		{
			int robbedAmount = RobSomeone(player);
			UpdateSpaceLabel("robSpace", player);
			GD.Print("You just robbed someone! you gained " + robbedAmount + " Pounds!");
		}
		else if (typeOfSpace == "Whiskey_Space")
		{
			Whiskey(player);
			UpdateSpaceLabel("Whiskey_Space", player);
			spacesInfo[player.PositionSpace].Name = spacesInfo[player.PositionSpace].OriginalName;
			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{player.PositionSpace + 1}");
			var sprite = markerNode.GetChild<Sprite2D>(0);
			sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[player.PositionSpace].OriginalName}.png");

		}
		await Task.CompletedTask;

	}

	//TURNS
	async Task Turn()
	{
		for (int i = 0; i < playersalive.Count; i++)
		{
			if (playersalive[i].Health == 0)
			{
				playersalive.Remove(Playerlist[i]);
				i--;
			}
		}
		if (player1.Health != 0)
		{
			await Turn_Player(player1);
		}
		if (player2.Health != 0)
		{
			await Turn_Player(player2);
		}
		if (GlobalVariables.Instance.playeramount > 2)
		{
			if (player3.Health != 0)
			{
				await Turn_Player(player3);
			}
		}
		if (GlobalVariables.Instance.playeramount > 3)
		{
			if (player4.Health != 0)
			{
				await Turn_Player(player4);
			}
		}


		GlobalVariables.Instance.TurnCount++;



		GD.Print("voor razorcap");
		if (GlobalVariables.Instance.TurnCount > 0) //dit zorgt ervoor dat de cap gaat spawnen
		{
			bool RunLoop = true;
			while (RunLoop)
			{
				for (int i = 0; i < playersalive.Count; i++)
				{
					if (playersalive[i].HasCap)
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
					GD.Print("spawnt nu razorcap");
					SpawnRazorCap();
					RunLoop = false;
				}

			}
		}


		SaveAllStates();
		if (playersalive.Count == 1 || GlobalVariables.Instance.TurnCount == 15)
		{
			EndGame();
		}
		else {
			await WaitForSeconds(3);
			ChooseMiniGame();
		}

	}

	async Task Turn_Player(Player player)
	{

		turnDpadIcon1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot1/inputBox");
		turnDpadIcon2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot2/inputBox");
		turnDpadIcon3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot3/inputBox");

		invSprite1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item1");
		invSprite2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item2");
		invSprite3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item3");

		invSlot1 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot1");
		invSlot2 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot2");
		invSlot3 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot3");



		{
			if (player == player1)
			{
				WhatPlayer = 1;
			}
			else if (player == player2)
			{
				WhatPlayer = 2;
			}
			else if (player == player3)
			{
				WhatPlayer = 3;
			}
			else if (player == player4)
			{
				WhatPlayer = 4;
			}

			GD.Print(player.Name + " Its your turn!");

			turnLabel.Text = $"{player.Name} is aan de beurt, wil je een item gebruiken?";
			turnLabel.Show();
			if (player.SkipTurn == false) // dit checkt of de speler zen beurt moet overslaan
			{
				//choose wich dice, hiervoor hebben we de shop mechanic + een shop menu nodig
				string useditem = await ChooseUseItem(player);



				invSprite1.Scale = new Vector2(0.01f, 0.01f);
				invSprite2.Scale = new Vector2(0.01f, 0.01f);
				invSprite3.Scale = new Vector2(0.01f, 0.01f);

				invSlot1.Scale = new Vector2(0.7f, 0.7f);
				invSlot2.Scale = new Vector2(0.7f, 0.7f);
				invSlot3.Scale = new Vector2(0.7f, 0.7f);




				if (useditem != "dice")
				{
					diceRoll = await AwaitButtonPress(player);
					diceRoll += player.RollAdjustment;
					await StartMovement(player, diceRoll);
				}

				if (diceRoll != 0)// zorgt ervoor dat als iemand 0  gooit de space niet nog een keer geactivate word, dat willen we niet.
				{
					player.RollAdjustment = 0; //dit zorgt ervoor dat volgende turn deze geen roll buff of debuff heeft.
					await Placedetection(spacesInfo[player.PositionSpace].Name, player);
				}

				Updatehud(player);

				if (player.Health > 0)
				{
					GD.Print(player.Name + " staat op " + spacesInfo[player.PositionSpace].Name + player.PositionSpace + " na " + diceRoll + " te hebben gegooid.");
				}
				else
				{
					player.Hide();
				}
			}


			if (player.SkipTurn == true)
			{
				player.SkipTurn = false;// dit zorgt ervoor dat next turn deze speler wel dingen mag doen
				GD.Print(player.Name + " Had to skip his turn!");
			}
			GD.Print("before timer");
			await WaitForSeconds(2);
			GD.Print("after timer");
			Spacelabel.Text = "";
		}
		async Task<int> AwaitButtonPress(Player player)
		{
			waitingforbuttonpress = true;
			turnLabel.Hide();
			turnDpadIcon1.Hide();
			turnDpadIcon2.Hide();
			turnDpadIcon3.Hide();
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
	}
	private async void ChooseTurn()
	{
		await Turn();
	}

	//EVENTSPACES
	void BlueSpace(Player player)
	{
		player.Currency += 15;
	}
	void RedSpace(Player player)
	{
		player.Currency -= 15;
	}
	int Robbery(Player player)
	{
		lostcurrency = rnd.Next(10, 31);
		player.Currency -= lostcurrency;
		player.Health -= 10;
		return lostcurrency;
	}
	int RobSomeone(Player player)
	{
		gainedCurrency = rnd.Next(10, 31);
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
		Updatehud(attacker);
		if (victim.Health == 0)
		{
			playersalive.Remove(victim);
			victim.Hide();
		}
		Updatehud(victim);
	}

	void RazorCapAttack(Player attacker, Player victim)
	{
		int attackdamage = rnd.Next(40, 61);
		victim.Health -= attackdamage;
		attacker.HasCap = false;
		GD.Print(attacker.Name + " hit " + victim.Name + " for " + attackdamage + ". " + victim.Name + " has " + victim.Health + " health remaining");
		Updatehud(attacker);
		if (victim.Health == 0)
		{
			playersalive.Remove(victim);
			victim.Hide();
		}
		Updatehud(victim);
	}
	void SpawnRazorCap()
	{

		int rndRazorCapSpace = rnd.Next(0, 42);
		if (rndRazorCapSpace == 15 || rndRazorCapSpace == 36) //voor het middelste vak, waar de cap niet mag komen.
		{
			rndRazorCapSpace += 1;
		}
		for (int i = 0; i < playersalive.Count; i++)
		{
			if (rndRazorCapSpace == playersalive[i].PositionSpace)
			{
				rndRazorCapSpace += 1;
			}
		}
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{2 + 1}"); // het is + 1 omdat de markers 1 voorop lopen met de spaces tellen dan we in de index hebben staan

		var sprite = markerNode.GetChild<Sprite2D>(0);
		sprite.Texture = GD.Load<Texture2D>("res://assets/Spaces/RazorCap_Space.png");
		spacesInfo[2].Name = "RazorCap_Space";
		GD.Print("razorcap ligt op vak " + rndRazorCapSpace);

	}


	void EndGame()
	{
		GD.Print("in end screen");
		GlobalVariables.Instance.SwitchToendscreen();
	}

	async Task ShopAsk(Player player)
	{	UpdateSpaceLabel("Shop", player);
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
				openShop(ShopInv, player);
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
					{
						mainShop.Hide();
						ChosenItem = Shopinv[0].Name;
						GD.Print("chose item: " + ChosenItem);
						runLoop2 = false;
					}
				}
				if (Input.IsActionJustPressed($"D-Pad-up_{WhatPlayer}"))
				{
					if (player.Currency >= Shopinv[1].Price)
					{
						mainShop.Hide();
						ChosenItem = Shopinv[1].Name;
						GD.Print("chose item: " + ChosenItem);
						runLoop2 = false;
					}
				}
				if (Input.IsActionJustPressed($"D-Pad-right_{WhatPlayer}"))
				{
					if (player.Currency >= Shopinv[2].Price)
					{
						ChosenItem = Shopinv[2].Name;
						GD.Print("chose item: " + ChosenItem);
						mainShop.Hide();
						runLoop2 = false;
					}
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

		while (RunLoop)
		{
			if (Input.IsActionJustPressed($"yes_{WhatPlayer}"))
			{
				updateInvPos(player);
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
		turnLabel.Text = $"{player.Name} is aan de beurt, Kies een item om te gebruiken?";
		useItem = true;
		while (useItem)
		{
			if (Input.IsActionJustPressed($"D-Pad-left_{WhatPlayer}"))
			{
				turnLabel.Hide();
				resetInvPos(player);
				itemId = player.Inventory[0];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");
						break;

					case "Whiskey":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await selectTrapPositon("Whiskey");
						GD.Print("Used item Whiskey; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenPipe":
						bool canuse = false;
						for (int i = 0; i < spacesAmount; i++)
						{
							GD.Print("checking for razorcapspace" + i);
							if (spacesInfo[i].Name == "RazorCap_Space")
							{
								canuse = true;
							}
						}
						if (canuse)
						{
							player.Inventory[0] = "0";
							updateInvSprite(player);
							GoldenPipe(player);
							useditem = "nodice";
							GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
							return useditem;
						}
						else GD.Print("no razorcapspace");
						break;

					case "DoubleDice":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await DoubleDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TripleDice":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await TripleDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TwentyDice":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await TwentyDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TenDice":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await TenDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "DashMushroom":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						DashMushroom(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "TeleportTorndPlayer":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						TeleportTorndPlayer(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "SwitchPlaces":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						SwitchPlaces(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "StealPlayerCap":
						bool canuse1 = false;
						for (int i = 0; i < playersalive.Count; i++)
						{
							if (playersalive[i].HasCap)
							{
								canuse1 = true;
							}
						}
						if (canuse1)
						{
							player.Inventory[0] = "0";
							updateInvSprite(player);
							StealPlayerCap(player);
							GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
							useditem = "nodice";
							return useditem;
						}
						else GD.Print("no one has the cap, better luck next time");
						break;

					case "PoisonMushroom":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						PoisonMushroom(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "BearTrap":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						await selectTrapPositon("BearTrap");
						GD.Print("Used item Beartrap; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "StealCoins":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						StealCoins(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "BrassKnuckles":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						BrassKnuckles(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenKnuckles":
						player.Inventory[0] = "0";
						updateInvSprite(player);
						GoldenKnuckles(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
				}
				updateInvSprite(player);
			}
			if (Input.IsActionJustPressed($"D-Pad-up_{WhatPlayer}"))
			{
				turnLabel.Hide();
				resetInvPos(player);
				itemId = player.Inventory[1];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");
						break;

					case "Whiskey":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await selectTrapPositon("Whiskey");
						GD.Print("Used item Whiskey; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenPipe":
						bool canuse = false;
						for (int i = 0; i < spacesAmount; i++)
						{
							GD.Print("checking for razorcapspace" + i);
							if (spacesInfo[i].Name == "RazorCap_Space")
							{
								canuse = true;
							}
						}
						if (canuse)
						{
							player.Inventory[1] = "0";
							updateInvSprite(player);
							GoldenPipe(player);
							useditem = "nodice";
							GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
							return useditem;
						}
						else GD.Print("no razorcapspace");
						break;

					case "DoubleDice":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await DoubleDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TripleDice":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await TripleDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TwentyDice":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await TwentyDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "TenDice":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await TenDice(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "dice";
						return useditem;

					case "DashMushroom":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						DashMushroom(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "TeleportTorndPlayer":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						TeleportTorndPlayer(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "SwitchPlaces":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						SwitchPlaces(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "StealPlayerCap":
						bool canuse1 = false;
						for (int i = 0; i < playersalive.Count; i++)
						{
							if (playersalive[i].HasCap)
							{
								canuse1 = true;
							}
						}
						if (canuse1)
						{
							player.Inventory[1] = "0";
							updateInvSprite(player);
							StealPlayerCap(player);
							GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
							useditem = "nodice";
							return useditem;
						}
						else GD.Print("no one has the cap, better luck next time");
						break;

					case "PoisonMushroom":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						PoisonMushroom(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "BearTrap":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						await selectTrapPositon("BearTrap");
						GD.Print("Used item Beartrap; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "StealCoins":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						StealCoins(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "BrassKnuckles":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						BrassKnuckles(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenKnuckles":
						player.Inventory[1] = "0";
						updateInvSprite(player);
						GoldenKnuckles(player);
						GD.Print("Used item " + useditem + "; it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
				}
				updateInvSprite(player);
			}
			if (Input.IsActionJustPressed($"D-Pad-right_{WhatPlayer}"))
			{
				turnLabel.Hide();
				resetInvPos(player);
				itemId = player.Inventory[2];
				switch (itemId)
				{
					case "0":
						GD.Print("no item in this slot");

						break;
					case "Whiskey":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await selectTrapPositon("Whiskey");
						GD.Print("Used item Whiskey  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenPipe":
						bool canuse = false;
						for (int i = 0; i < spacesAmount; i++)
						{
							GD.Print("checking for razorcapspace" + i);
							if (spacesInfo[i].Name == "RazorCap_Space")
							{
								canuse = true;
							}
						}
						if (canuse)
						{
							player.Inventory[2] = "0";
							updateInvSprite(player);
							GoldenPipe(player);
							useditem = "nodice";
							GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
							return useditem;
						}
						else GD.Print("no razorcapspace");
						break;

					case "DoubleDice":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await DoubleDice(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "dice";
						return useditem;
					case "TripleDice":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await TripleDice(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "dice";
						return useditem;
					case "TwentyDice":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await TwentyDice(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "dice";
						return useditem;
					case "TenDice":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await TenDice(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "dice";
						return useditem;
					case "DashMushroom":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						DashMushroom(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "TeleportTorndPlayer":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						TeleportTorndPlayer(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "SwitchPlaces":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						SwitchPlaces(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "StealPlayerCap":
						bool canuse1 = false;
						for (int i = 0; i < playersalive.Count; i++)
						{
							if (playersalive[i].HasCap)
							{
								canuse1 = true;
							}
						}
						if (canuse1)
						{
							player.Inventory[2] = "0";
							updateInvSprite(player);
							StealPlayerCap(player);
							GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
							useditem = "nodice";
							return useditem;
						}
						else GD.Print("no one has the cap, better luck next time");
						break;
					case "PoisonMushroom":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						PoisonMushroom(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "BearTrap":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						await selectTrapPositon("BearTrap");
						GD.Print("Used item Beartrap  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "StealCoins":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						StealCoins(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
					case "BrassKnuckles":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						BrassKnuckles(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;

					case "GoldenKnuckles":
						player.Inventory[2] = "0";
						updateInvSprite(player);
						GoldenKnuckles(player);
						GD.Print("Used item " + useditem + "  it has vanished from their inventory.");
						useditem = "nodice";
						return useditem;
				}
			}
			if (Input.IsActionJustPressed($"D-Pad-down_{WhatPlayer}"))
			{
				turnLabel.Text = $"{player.Name} is aan de beurt, wil je een item gebruiken?";
				resetInvPos(player);
				updateInvSprite(player);
				useItem = false;

			}
			await ToSignal(GetTree().CreateTimer(0), "timeout");
		}
		Updatehud(player);
		return useditem;
	}
	//items en item spaces
	void Whiskey(Player player) //!ITEM SPACE player verliest currency EN moet een turn overslaan EN ITEM
	{
		GD.Print("you found a bottle of whiskey and drank it all");
		player.SkipTurn = true;
		player.Currency -= player.Currency / 3;
		GD.Print("You got too drunk and went on a spending spree! also you have to skip your next turn because of your hangover");
		Updatehud(player);
	}
	void Beartrap(Player player) // dit is de space
	{	UpdateSpaceLabel("BearTrap", player);
		GD.Print("you stepped into a beartrap, you take damage and next turn cant walk well");
		player.Health -= 40;
		player.RollAdjustment += -5;
		spacesInfo[player.PositionSpace].Name = spacesInfo[player.PositionSpace].OriginalName;
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{player.PositionSpace + 1}");
		var sprite = markerNode.GetChild<Sprite2D>(0);
		sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[player.PositionSpace].OriginalName}.png");
		Updatehud(player);
	}
	void GoldenPipe(Player player) //**KAN ALLEEN AANGEVRAAGD WORDEN ALS DE razorcapspace op het bord zit!
	{
		for (int i = 0; i < spacesAmount; i++)
		{
			if (spacesInfo[i].Name == "RazorCap_Space")
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
		Updatehud(player);
	}
	void TeleportTorndPlayer(Player player) //teleport to a random player
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, playersalive.Count);
			if (player != playersalive[rndplayer])
			{
				player.Position = playersalive[rndplayer].Position;
				player.PositionSpace = playersalive[rndplayer].PositionSpace;
				runloop = false;
			}
		}

	}
	void SwitchPlaces(Player player) // switch places with a random player
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, playersalive.Count);
			if (player != playersalive[rndplayer])
			{
				Vector2 originalposition = player.Position;
				int originalpositionspace = player.PositionSpace;

				player.Position = playersalive[rndplayer].Position;
				player.PositionSpace = playersalive[rndplayer].PositionSpace;

				playersalive[rndplayer].Position = originalposition;
				playersalive[rndplayer].PositionSpace = originalpositionspace;
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
			for (int i = 0; i < playersalive.Count; i++)
			{
				if (playersalive[i].HasCap)
				{
					if (playersalive[i].Health > 10)
					{
						playersalive[i].Health -= 10;
					}
					playersalive[i].HasCap = false;
					player.HasCap = true;
					victim = playersalive[i].Name;
					runloop = false;
				}
			}
		}
		Updatehud(player);
		GD.Print(player.Name + "used his goons to steal te cap from: " + victim);
	}
	void PoisonMushroom(Player player)//geeft een random player een roll debuff next turn
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, playersalive.Count);
			if (player != playersalive[rndplayer])
			{
				playersalive[rndplayer].RollAdjustment -= 5;
				runloop = false;
			}
		}
		Updatehud(player);

	}
	void StealCoins(Player player) // steelt currency tussen 1 en de helft van een random persoon;
	{
		bool runloop = true;
		while (runloop)
		{
			int rndplayer = rnd.Next(0, playersalive.Count);
			if (player != playersalive[rndplayer])
			{
				int stolenamount = rnd.Next(0, playersalive[rndplayer].Currency / 2);
				player.Currency += stolenamount;
				runloop = false;
			}
		}
		Updatehud(player);
	}
	void BrassKnuckles(Player player) //de player krijgt brass knuckles, een mini razor cap die minder damage doet maar waar je ook niet stopt nadat je aangevallen hebt, kan in combinatie met de razorcap
	{
		player.HasKnuckles = true;
		Updatehud(player);
	}

	void GoldenKnuckles(Player player) // goldenknuckles, knuckles die niet kapot gaan.
	{
		player.HasGoldenKnuckles = true;
		Updatehud(player);
	}

	void ChooseMiniGame()
	{
		GlobalVariables.Instance.SwitchToMinigame();

	}

	async Task selectTrapPositon(string itemused)
	{
		GD.Print("in buttonselect");



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

		if (itemused == "Whiskey")
		{
			buttonmin1.Pressed += () => WhiskeyAmount(-1);
			buttonmin2.Pressed += () => WhiskeyAmount(-2);
			buttonplus1.Pressed += () => WhiskeyAmount(1);
			buttonplus2.Pressed += () => WhiskeyAmount(2);
		}
		else if (itemused == "BearTrap")
		{
			buttonmin1.Pressed += () => beartrapamount(-1);
			buttonmin2.Pressed += () => beartrapamount(-2);
			buttonplus1.Pressed += () => beartrapamount(1);
			buttonplus2.Pressed += () => beartrapamount(2);
		}
		buttonmin1.Position = new Vector2(501, 318);
		buttonmin2.Position = new Vector2(471, 318);
		buttonplus1.Position = new Vector2(561, 318);
		buttonplus2.Position = new Vector2(591, 318);


		buttonmin1.Show();
		buttonmin2.Show();
		buttonplus1.Show();
		buttonplus2.Show();
		await Task.CompletedTask;
	}

	private void WhiskeyAmount(int WhiskeySpacesamount)
	{
		GD.Print(WhiskeySpacesamount);
		int whiskeyspace = player1.PositionSpace + WhiskeySpacesamount;
		Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{whiskeyspace + 1}"); // het is + 1 omdat de markers 1 voorop lopen met de spaces tellen dan we in de index hebben staan

		var sprite = markerNode.GetChild<Sprite2D>(0);
		GD.Print("test");
		sprite.Texture = GD.Load<Texture2D>("res://assets/Spaces/Whiskey_Space.png");
		spacesInfo[whiskeyspace].Name = "Whiskey_Space";
		GD.Print(spacesInfo[whiskeyspace].Name);
		GD.Print("whiskey ligt op vak " + whiskeyspace);
		buttonmin1.Hide();
		buttonmin2.Hide();
		buttonplus1.Hide();
		buttonplus2.Hide();

		buttonmin1.ZIndex = 0;
		buttonmin2.ZIndex = 0;
		buttonplus1.ZIndex = 0;
		buttonplus2.ZIndex = 0;
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


	public void SaveAllStates()
	{
		for (int playerNumber = 1; playerNumber <= Playerlist.Length; playerNumber++)
		{
			// Get the player node
			Player playerNode = GetNode<Player>($"player{playerNumber}");

			// Save the player's state
			PlayerState playerState = playerNode.SavePlayerState();

			// Save the state to GlobalVariables or another state management system
			GlobalVariables.Instance.SavePlayerState(playerNumber, playerNode);
		}
		Node2D[] spaceNodes = new Node2D[spacesInfo.Length];
		string[] spaceNames = new string[spacesInfo.Length];
		string[] spaceOriginalNames = new string[spacesInfo.Length];
		for (int i = 0; i < spacesAmount; i++)
		{

			spaceNames[i] = spacesInfo[i].Name;   // Accessing the Name from the tuple
			spaceOriginalNames[i] = spacesInfo[i].OriginalName;  // Accessing the OriginalName
		}


		GlobalVariables.Instance.SaveBoardState(spaceNodes, spaceNames, spaceOriginalNames);



	}


	public void RestoreAllStates()
	{
		for (int i = 1; i <= GlobalVariables.Instance.playeramount; i++)
		{
			// Retrieve the saved player state
			PlayerState playerState = GlobalVariables.Instance.GetPlayerState(i);

			// Get the player node
			Player playerNode = GetNode<Player>($"player{i}");

			// Load the state into the player node
			playerNode.LoadPlayerState(playerState);
		}
		BoardState bord = GlobalVariables.Instance.GetBoardState();
		for (int i = 0; i < spacesAmount; i++)
		{

			spacesInfo[i].Name = bord.Names[i];
			spacesInfo[i].OriginalName = bord.OriginalNames[i];

			Node2D markerNode = GetNode<Node2D>($"spaces/Marker2D{i + 1}");
			var sprite = markerNode.GetChild<Sprite2D>(0);
			sprite.Texture = GD.Load<Texture2D>($"res://assets/Spaces/{spacesInfo[i].Name}.png");
		}

	}
	//andere ui
	void Updatehud(Player player)
	{

		TextureRect icon = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/");

		invSprite1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item1");
		invSprite2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item2");
		invSprite3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item3");

		Label currency = GetNode<Label>($"Node2D/CanvasLayer/{player.Name}/Playerhud/Currency{player.Name}");
		Label health = GetNode<Label>($"Node2D/CanvasLayer/{player.Name}/Playerhud/Health{player.Name}");
		Label rolladjustLabel = GetNode<Label>($"Node2D/CanvasLayer/{player.Name}/Playerhud/rolladjust/Label");

		currency.Text = player.Currency.ToString();
		health.Text = player.Health.ToString();
		GD.Print(player.Name);
		GetNode<Node2D>($"Node2D/CanvasLayer/{player.Name}").Show();

		Control cap = GetNode<Control>($"Node2D/CanvasLayer/{player.Name}/Playerhud/razorCap");

		Control brassknuckles = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/knuckles/brass");

		Control goldenknuckles = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/knuckles/golden");

		//update alive sprite	
		if (player.Health == 0)
		{
			icon.Texture = GD.Load<Texture2D>($"res://assets/hud/{player.Name}Dead.png");
		}

		if (player.HasCap == true)
		{
			cap.Show();
		}
		else
		{
			cap.Hide();
		}

		if (player.HasKnuckles == true)
		{
			brassknuckles.Show();
		}
		else
		{
			brassknuckles.Hide();
		}

		if (player.HasGoldenKnuckles == true)
		{
			goldenknuckles.Show();
		}
		else
		{
			goldenknuckles.Hide();
		}

		if (player.RollAdjustment == 0)
		{
			rolladjustLabel.Hide();
		}
		else if (player.RollAdjustment > 0)
		{
			rolladjustLabel.Text = "+" + player.RollAdjustment.ToString();
		}
		else if (player.RollAdjustment < 0)
		{
			rolladjustLabel.Text = player.RollAdjustment.ToString();
		}

		for (int i = 0; i < 4; i++)
		{

		}


		updateInvSprite(player);
	}
	public void updateInvPos(Player player)
	{
		GD.Print("run updateInvPos");


		dpadIcon1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot1/inputBox");
		dpadIcon2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot2/inputBox");
		dpadIcon3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot3/inputBox");


		Label label = GetNode<Label>($"Node2D/CanvasLayer/Label");

		invSprite1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item1");
		invSprite2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item2");
		invSprite3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item3");

		invSlot1 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot1");
		invSlot2 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot2");
		invSlot3 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot3");


		TextureRect Sprite1NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item1PosItem");
		TextureRect Sprite2NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item2PosItem");
		TextureRect Sprite3NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item3PosItem");






		label.Show();

		dpadIcon1.Show();
		dpadIcon2.Show();
		dpadIcon3.Show();




		invSprite1.Scale = new Vector2(0.03f, 0.03f);
		invSprite2.Scale = new Vector2(0.03f, 0.03f);
		invSprite3.Scale = new Vector2(0.03f, 0.03f);

		invSlot1.Scale = new Vector2(2.1f, 2.1f);
		invSlot2.Scale = new Vector2(2.1f, 2.1f);
		invSlot3.Scale = new Vector2(2.1f, 2.1f);

		invSprite1.GlobalPosition = new Vector2(Sprite1NewPos.GlobalPosition.X, Sprite1NewPos.GlobalPosition.Y);
		invSprite2.GlobalPosition = new Vector2(Sprite2NewPos.GlobalPosition.X, Sprite2NewPos.GlobalPosition.Y);
		invSprite3.GlobalPosition = new Vector2(Sprite3NewPos.GlobalPosition.X, Sprite3NewPos.GlobalPosition.Y);

		invSlot1.Position = new Vector2(invSprite1.Position.X - 4, invSprite1.Position.Y - 4);
		invSlot2.Position = new Vector2(invSprite2.Position.X - 4, invSprite2.Position.Y - 4);
		invSlot3.Position = new Vector2(invSprite3.Position.X - 4, invSprite3.Position.Y - 4);

	}
	void resetInvPos(Player player)
	{
		invSprite1 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item1");
		invSprite2 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item2");
		invSprite3 = GetNode<TextureRect>($"Node2D/CanvasLayer/{player.Name}/Playerhud/item3");

		invSlot1 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot1");
		invSlot2 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot2");
		invSlot3 = GetNode<Panel>($"Node2D/CanvasLayer/{player.Name}/Playerhud/slot3");

		TextureRect Sprite1NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item1PosItem");
		TextureRect Sprite2NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item2PosItem");
		TextureRect Sprite3NewPos = GetNode<TextureRect>($"Node2D/CanvasLayer/item3PosItem");






		if (player == player1)
		{
			invSprite1.Position = invSprite1Play1Pos;
			invSprite2.Position = invSprite2Play1Pos;
			invSprite3.Position = invSprite3Play1Pos;

			invSlot1.Position = invSprite1Play1Pos;
			invSlot2.Position = invSprite2Play1Pos;
			invSlot3.Position = invSprite3Play1Pos;
		}
		else if (player == player2)
		{
			invSprite1.Position = invSprite1Play2Pos;
			invSprite2.Position = invSprite2Play2Pos;
			invSprite3.Position = invSprite3Play2Pos;

			invSlot1.Position = invSprite1Play2Pos;
			invSlot2.Position = invSprite2Play2Pos;
			invSlot3.Position = invSprite3Play2Pos;
		}
		else if (player == player3)
		{
			invSprite1.Position = invSprite1Play3Pos;
			invSprite2.Position = invSprite2Play3Pos;
			invSprite3.Position = invSprite3Play3Pos;

			invSlot1.Position = invSprite1Play3Pos;
			invSlot2.Position = invSprite2Play3Pos;
			invSlot3.Position = invSprite3Play3Pos;
		}
		else if (player == player4)
		{
			invSprite1.Position = invSprite1Play4Pos;
			invSprite2.Position = invSprite2Play4Pos;
			invSprite3.Position = invSprite3Play4Pos;

			invSlot1.Position = invSprite1Play4Pos;
			invSlot2.Position = invSprite2Play4Pos;
			invSlot3.Position = invSprite3Play4Pos;
		}

		invSprite1.Scale = new Vector2(0.01f, 0.01f);
		invSprite2.Scale = new Vector2(0.01f, 0.01f);
		invSprite3.Scale = new Vector2(0.01f, 0.01f);

		invSlot1.Scale = new Vector2(0.7f, 0.7f);
		invSlot2.Scale = new Vector2(0.7f, 0.7f);
		invSlot3.Scale = new Vector2(0.7f, 0.7f);

		dpadIcon1.Hide();
		dpadIcon2.Hide();
		dpadIcon3.Hide();


	}

	//inventory
	// ("Whiskey", 20), ("GoldenPipe", 20), ("DoubleDice", 10), ("TripleDice", 13), ("TwentyDice", 20), ("TenDice", 8), 
	// ("DashMushroom", 5), ("TeleportTorndPlayer", 15), ("SwitchPlaces", 15), ("StealPlayerCap", 40), 
	// ("PoisonMushroom", 5), ("StealCoins", 30), ("BrassKnuckles", 20), ("GoldenKnuckles", 50), 
	// ("BearTrap", 40) };

	void updateInvSprite(Player player)
	{
		switch (player.Inventory[0])
		{
			case "0":

				GD.Print("no item in slot 1");
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/empty.png");
				break;

			case "Whiskey":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/Wiskey.png");
				break;

			case "GoldenPipe":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenPipe.png");
				break;

			case "DoubleDice":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/DoubleDice.png");
				break;

			case "TripleDice":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/TripleDice.png");
				break;

			case "TwentyDice":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/TwentyDice.png");
				break;
			case "TenDice":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/TenDice.png");
				break;

			case "DashMushroom":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/DashMushroom.png");
				break;

			case "TeleportTorndPlayer":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/TeleportTorndPlayer.png");
				break;

			case "SwitchPlaces":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/SwitchPlaces.png");
				break;

			case "StealPlayerCap":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/StealPlayerCap.png");
				break;

			case "PoisonMushroom":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/PoisonMushroom.png");
				break;

			case "StealCoins":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/StealCoins.png");
				break;

			case "BrassKnuckles":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/BrassKnuckles.png");
				break;

			case "GoldenKnuckles":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenKnuckles.png");
				break;

			case "BearTrap":
				invSprite1.Texture = GD.Load<Texture2D>($"res://assets/items/Beartrap.png");
				break;


		}

		switch (player.Inventory[1])
		{
			case "0":

				GD.Print("no item in slot 1");
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/empty.png");
				break;

			case "Whiskey":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/Wiskey.png");
				break;

			case "GoldenPipe":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenPipe.png");
				break;

			case "DoubleDice":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/DoubleDice.png");
				break;

			case "TripleDice":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/TripleDice.png");
				break;

			case "TwentyDice":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/TwentyDice.png");
				break;
			case "TenDice":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/TenDice.png");
				break;

			case "DashMushroom":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/DashMushroom.png");
				break;

			case "TeleportTorndPlayer":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/TeleportTorndPlayer.png");
				break;

			case "SwitchPlaces":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/SwitchPlaces.png");
				break;

			case "StealPlayerCap":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/StealPlayerCap.png");
				break;

			case "PoisonMushroom":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/PoisonMushroom.png");
				break;

			case "StealCoins":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/StealCoins.png");
				break;

			case "BrassKnuckles":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/BrassKnuckles.png");
				break;

			case "GoldenKnuckles":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenKnuckles.png");
				break;

			case "BearTrap":
				invSprite2.Texture = GD.Load<Texture2D>($"res://assets/items/Beartrap.png");
				break;


		}

		switch (player.Inventory[2])
		{
			case "0":

				GD.Print("no item in slot 1");
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/empty.png");
				break;

			case "Whiskey":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/Wiskey.png");
				break;

			case "GoldenPipe":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenPipe.png");
				break;

			case "DoubleDice":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/DoubleDice.png");
				break;

			case "TripleDice":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/TripleDice.png");
				break;

			case "TwentyDice":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/TwentyDice.png");
				break;
			case "TenDice":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/TenDice.png");
				break;

			case "DashMushroom":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/DashMushroom.png");
				break;

			case "TeleportTorndPlayer":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/TeleportTorndPlayer.png");
				break;

			case "SwitchPlaces":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/SwitchPlaces.png");
				break;

			case "StealPlayerCap":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/StealPlayerCap.png");
				break;

			case "PoisonMushroom":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/PoisonMushroom.png");
				break;

			case "StealCoins":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/StealCoins.png");
				break;

			case "BrassKnuckles":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/BrassKnuckles.png");
				break;

			case "GoldenKnuckles":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/GoldenKnuckles.png");
				break;

			case "BearTrap":
				invSprite3.Texture = GD.Load<Texture2D>($"res://assets/items/Beartrap.png");
				break;


		}

	}

	void UpdateSpaceLabel(string whatspace, Player player)
	{	GD.Print("in updatelabel");
		if (whatspace == "blueSpace")
		{
			Spacelabel.Text ="Je staat op een blauw vakje. je krijgt 15 pond.";

		}
		else if (whatspace == "redSpace")
		{
			Spacelabel.Text = "Je staat op een rood vakje, je verliest 15 pond.";
		}
		else if (whatspace == "TopLeftShortcut")
		{
			Spacelabel.Text = "Je staat bij een roeiboot. je gebruikt de boot om op de rivier te varen ";
		}
		else if (whatspace == "TopRightShortcut")
		{
			Spacelabel.Text = "Je staat bij op het station. je stapt op de trein";
		}
		else if (whatspace == "BotLeftShortcut")
		{
			Spacelabel.Text = "Je staat bij een roeiboot. je gebruikt de boot om op de rivier te varen ";
		}
		else if (whatspace == "BotRightShortcut")
		{
			Spacelabel.Text = "Je staat bij op het station. je stapt op de trein";
		}
		else if (whatspace == "getRobbedSpace")
		{
			Spacelabel.Text = "je bent beroofd! je verliest " + lostcurrency + " en ze doen 10 damage!";
		}
		else if (whatspace == "knockoutSpace")
		{
			Spacelabel.Text = "je word bewusteloos geslagen! je moet je volgende beurt overslaan.";
		}
		else if (whatspace == "robSpace")
		{
			Spacelabel.Text = "je ziet een doelwit en overvalt hem, je steelt " + gainedCurrency + " pond!";
		}
		else if (whatspace == "Whiskey_Space")
		{
			Spacelabel.Text = "je ziet een fles whiskey staan en drinkt hem helemaal op! daarna ga je de kroeg in en besteed je een substantieel bedrag. je slaat volgende beurt over om je kater weg te werken.";

		}
		else if(whatspace == "RazorCappurchase"){
			Spacelabel.Text = "je hebt de optie om een razorcap te kopen voor 50 pond. kies linker bumper voor ja en rechterbumper voor nee."; 
		}
		else if(whatspace == "Shop"){
			Spacelabel.Text = "je staat bij de shop, wil je naar binnen?";
		}
		else if(whatspace == "BearTrap"){
			Spacelabel.Text = "Je staat in een berenval en verliest 40 health, ook loop je volgende beurt mank.";
		}
	}
	 private async Task WaitForSeconds(float seconds)
    {
        await ToSignal(GetTree().CreateTimer(seconds), "timeout");
    }

	void openMainShop((string Name, int Price)[] shopItems, Player player)
	{
		textShop.Hide();
		mainShop.Show();
		string[] textureRectPaths = {
		"CanvasLayersshop/TextureRectRounded/Slot1Rect/itemTex",
		"CanvasLayersshop/TextureRectRounded/Slot2Rect/itemTex",
		"CanvasLayersshop/TextureRectRounded/Slot3Rect/itemTex"
		};

		string[] titleLabelsPaths = {
		"CanvasLayersshop/TextureRectRounded/Slot1Rect/title",
		"CanvasLayersshop/TextureRectRounded/Slot2Rect/title",
		"CanvasLayersshop/TextureRectRounded/Slot3Rect/title"
		};

		string[] descriptionLabelsPaths = {
		"CanvasLayersshop/TextureRectRounded/Slot1Rect/description",
		"CanvasLayersshop/TextureRectRounded/Slot2Rect/description",
		"CanvasLayersshop/TextureRectRounded/Slot3Rect/description"
		};

		string[] priceLabelsPaths = {
		"CanvasLayersshop/TextureRectRounded/Slot1Rect/itemTex/priceLabel",
		"CanvasLayersshop/TextureRectRounded/Slot2Rect/itemTex/priceLabel",
		"CanvasLayersshop/TextureRectRounded/Slot3Rect/itemTex/priceLabel"
		};

		for (int i = 0; i < shopItems.Length; i++)
		{

			var currentItem = shopItems[i];
			TextureRect itemTex = GetNode<TextureRect>(textureRectPaths[i]);
			Label titleLabel = GetNode<Label>(titleLabelsPaths[i]);
			Label DescriptionLabel = GetNode<Label>(descriptionLabelsPaths[i]);
			Label PriceLabel = GetNode<Label>(priceLabelsPaths[i]);


			switch (currentItem.Name)
			{

				case "Whiskey":
					GD.Print("WHiskey");
					DescriptionLabel.Text = "Force a random player to drink Whiskey and cause him to black out.";
					break;

				case "GoldenPipe":
					DescriptionLabel.Text = "Take a carage straight the razorcap.";
					break;

				case "DoubleDice":
					DescriptionLabel.Text = "Why throw one when you can throw two. (Throw 2 dices with each a value between 0-6)";
					break;

				case "TripleDice":
					DescriptionLabel.Text = "The only thing better than two dices are three dices. (Throw 3 dices with each a value between 0-6)";
					break;

				case "TwentyDice":
					DescriptionLabel.Text = "Throw a dice with an eyecount between -20 and 20";
					break;
				case "TenDice":
					DescriptionLabel.Text = "Throw a dice with an eyecount between 0 and 10";
					break;

				case "DashMushroom":
					DescriptionLabel.Text = "These speedy boots will make you the fastest man in England (adds a random number to your next throw)";
					break;

				case "TeleportTorndPlayer":
					DescriptionLabel.Text = "Teleport to random player on the board.";
					break;

				case "SwitchPlaces":
					DescriptionLabel.Text = "Swap places with a random player on the board.";
					break;

				case "StealPlayerCap":
					DescriptionLabel.Text = "Steal the Razor cap from the player that has it.";
					break;

				case "PoisonMushroom":
					DescriptionLabel.Text = "Removes a random number from your next throw.";
					break;

				case "StealCoins":
					DescriptionLabel.Text = "Steal coins from a random player.";
					break;

				case "BrassKnuckles":
					DescriptionLabel.Text = "Knuckels that will deal 20 damage to a player when passing them. (1 use only)";
					break;

				case "GoldenKnuckles":
					DescriptionLabel.Text = "Knuckels that will deal 20 damage to a player when passing them. (∞ uses)";
					break;

				case "BearTrap":
					DescriptionLabel.Text = "Place a bear trap somewhere on the board. (Deals damage to passing players and cause them to skip a turn)";
					break;
			}
			itemTex.Texture = GD.Load<Texture2D>($"res://assets/items/{currentItem.Name}.png");
			titleLabel.Text = currentItem.Name.ToString();
			PriceLabel.Text = "£ " + currentItem.Price.ToString();
			GD.Print("loaded item texture: " + itemTex);
			GD.Print($"Item: {currentItem.Name}, Price: {currentItem.Price}");
		}

	}

	void openShop((string Name, int Price)[] shopItems, Player player)
	{
		textShop.Show();
		Timer timer = new Timer();

		// Set timer properties
		timer.WaitTime = 3.0f; // Timer duration in seconds
		timer.OneShot = true;  // The timer will stop after one timeout

		// Add the timer to the scene
		AddChild(timer);

		// Connect the "timeout" signal to a callback function
		timer.Timeout += () => OnTimerTimeout(shopItems, player);

		// Start the timer
		timer.Start();
	}

	// Callback function to handle timer's timeout signal
	private void OnTimerTimeout((string Name, int Price)[] shopItems, Player player)
	{
		mainShop.Show();
		openMainShop(shopItems, player);
	}
}
