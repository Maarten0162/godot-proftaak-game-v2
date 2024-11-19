using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Bomb : Node2D
{
		private List<Sprite2D> BombSprites = new List<Sprite2D>();
		private int minigameplayeramount; // Number of players in the game
		private List<string> Spelers = new List<string>();
	// Called when the node enters the scene tree for the first time.
		private List<bool> hastheBomb = new List<bool>();

		private Timer Bom;

	public override void _Ready()
	{
		minigameplayeramount = 3;
		BombSprites = new List<Sprite2D>();
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
		{
			BombSprites.Add(GetNode<Sprite2D>("Player1"));
			GetNode<Sprite2D>("Player1").Show();
			// scoreLabels[0] = GetNode<Label>("Label1");
			minigameplayeramount++;
			Spelers.Insert(0,"player1");
			hastheBomb.Insert(0,false);

		}
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
		{
			BombSprites.Add(GetNode<Sprite2D>("Player2"));
			GetNode<Sprite2D>("Player2").Show();
			// scoreLabels[1] = GetNode<Label>("Label2");
			minigameplayeramount++;
			Spelers.Insert(1,"player2");
			hastheBomb.Insert(1,false);
		}
		// if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
		// {
		// 	BombSprites.Add(GetNode<Sprite2D>("Player3"));
		// 	GetNode<Sprite2D>("Player3").Show();
		// 	// scoreLabels[2] = GetNode<Label>("Label3");
		// 	minigameplayeramount++;
		// 	Spelers.Insert(2,"player3");
		// 	hastheBomb.Insert(2,false);
		// }
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
		{
			BombSprites.Add(GetNode<Sprite2D>("Player4"));
			GetNode<Sprite2D>("Player4").Show();
			// scoreLabels[3] = GetNode<Label>("Label4");
			minigameplayeramount++;
			Spelers.Insert(3,"player4");
			hastheBomb.Insert(3,false);
		}
		//hastheBomb[0] = true;
		GD.Print("Ready Done");
		Bom.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Spelers.Contains("player1") && hastheBomb[0] == true && Input.IsActionJustPressed("A_1"))
		{
			Actie(0);
		}
		if (Spelers.Contains("player1") && hastheBomb[1] == true && Input.IsActionJustPressed("A_2"))
		{
			Actie(1);
		}
		if (Spelers.Contains("player1") && hastheBomb[2] == true && Input.IsActionJustPressed("A_3"))
		{
			Actie(2);
		}
		if (Spelers.Contains("player1") && hastheBomb[3] == true && Input.IsActionJustPressed("A_4"))
		{
			Actie(3);
		}


	}
	private void Actie(int Speler)
	{
		if (Bom.TimeLeft <= 0)
		{
			GD.Print("Hallo");
		}
		else
		{
			passtheBomb();
		}
	}

	private void passtheBomb()
	{

	}
	private void Ontploffen()
	{
		if (hastheBomb[0] == true)
		{
			Spelers.Remove("player1");
			GetNode<Sprite2D>("Player1").Hide();
		}
		if (hastheBomb[1] == true)
		{
			Spelers.Remove("player2");
			GetNode<Sprite2D>("Player2").Hide();
		}
		if (hastheBomb[2] == true)
		{
			Spelers.Remove("player3");
			GetNode<Sprite2D>("Player3").Hide();
		}
		if (hastheBomb[3] == true)
		{
			Spelers.Remove("player4");
			GetNode<Sprite2D>("Player4").Hide();
		}
		
		if(Spelers.Count == 1)
		{
			Winnaar();
		}
	}
	private void Winnaar()
	{
		if (Spelers.Contains("player1"))
		{
			// Spelers 1 wint;
		}
		if (Spelers.Contains("player2"))
		{
			// Spelers 2 wint;
		}
		if (Spelers.Contains("player3"))
		{
			// Spelers 3 wint
		}
		if (Spelers.Contains("player4"))
		{
			// Spelers 4 wint			
		}
	}
}
