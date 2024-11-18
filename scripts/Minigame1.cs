using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Minigame1 : Node
{
	private int[] scores = new int[4];
	private List<Sprite2D> horseSprites = new List<Sprite2D>();
	private Label[] scoreLabels = new Label[4];
	private Label highscoreLabel;
	private int highscore = 0;
	private Timer gameTimer;
	private bool isGameActive = true;
	private string highscoreFilePath = "res://Minigame1/Highscore.txt";
	private bool[] playerReadyFlags = new bool[4]; // Track readiness of players
	private int readyPlayers = 0; // Count of ready players
	private int minigameplayeramount; // Number of players in the game
	


	public override void _Ready()
	{
		
		minigameplayeramount = 0;
		horseSprites = new List<Sprite2D>();
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
		{
			horseSprites.Add(GetNode<Sprite2D>("Horse1"));
			GetNode<Sprite2D>("Horse1").Show();
			// scoreLabels[0] = GetNode<Label>("Label1");
			minigameplayeramount++;
		}
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
		{
			horseSprites.Add(GetNode<Sprite2D>("Horse2"));
			GetNode<Sprite2D>("Horse2").Show();
			// scoreLabels[1] = GetNode<Label>("Label2");
			minigameplayeramount++;
		}
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
		{
			horseSprites.Add(GetNode<Sprite2D>("Horse3"));
			GetNode<Sprite2D>("Horse3").Show();
			// scoreLabels[2] = GetNode<Label>("Label3");
			minigameplayeramount++;
		}
		if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
		{
			horseSprites.Add(GetNode<Sprite2D>("Horse4"));
			GetNode<Sprite2D>("Horse4").Show();
			// scoreLabels[3] = GetNode<Label>("Label4");
			minigameplayeramount++;
		}


		highscoreLabel = GetNode<Label>("LabelHighscore");

		gameTimer = GetNode<Timer>("Timer");
		gameTimer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));


		LoadHighscore();
		gameTimer.Start(); // Start de timer

		UpdateUI();

	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("A_1") || Input.IsActionJustPressed("1"))
		{
			OnKeyPressed(0);
		}
		if (Input.IsActionJustPressed("A_2"))
		{
			OnKeyPressed(1);
		}
		if (Input.IsActionJustPressed("A_3"))
		{
			OnKeyPressed(2);
		}
		if (Input.IsActionJustPressed("A_4"))
		{
			OnKeyPressed(3);
		}
	}

	private void OnKeyPressed(int playerIndex)
	{
		if (isGameActive)
		{
			scores[playerIndex]++;
			GD.Print($"Speler {playerIndex + 1} score verhoogd!"); // Debug bericht om te controleren of de methode wordt aangeroepen
			UpdateHighscore();
			UpdateUI();
		}
	}

	private void UpdateHighscore()
	{
		foreach (var score in scores)
		{
			if (score > highscore)
			{
				highscore = score;
			}
		}
		highscoreLabel.Text = $"Highscore: {highscore}";
		SaveHighscore();
	}

	private void UpdateUI()
	{
		for (int i = 0; i < minigameplayeramount; i++)
		{
			if (horseSprites[i] != null)
			{
				// Update de positie van het paard op basis van de score
				float newX = scores[i] * 10; // Pas de factor aan op basis van je layout
				horseSprites[i].Position = new Vector2(newX, horseSprites[i].Position.Y);
			}

			if (scoreLabels[i] != null)
			{
				// Update de score label
				scoreLabels[i].Text = $"Speler {i + 1} Score: {scores[i]}";
			}
		}
	}

	private void SaveHighscore()
	{
		var file = FileAccess.Open(highscoreFilePath, FileAccess.ModeFlags.Write);
		file.StoreLine(highscore.ToString());
		file.Close();
	}

	private void LoadHighscore()
	{
		if (FileAccess.FileExists(highscoreFilePath))
		{
			var file = FileAccess.Open(highscoreFilePath, FileAccess.ModeFlags.Read);
			highscore = int.Parse(file.GetLine());
			highscoreLabel.Text = $"Highscore: {highscore}";
			file.Close();
		}

	}

	private void OnTimerTimeout()
	{
		isGameActive = false;
		GD.Print("Tijd is om!"); // Debug bericht om te zien dat de timer is afgelopen
		CheckWinner();
		GetTree().CreateTimer(3f).Connect("timeout", new Callable(this, nameof(ReturnToMainScene)));
	}

	private void ReturnToMainScene()
    {
        GlobalVariables.Instance.SwitchToMainBoard();
    }
	private void CheckWinner()
	{
		GD.Print("checkwinner");

// Initialize the highest score
int highestscore = 0;

// Find the highest score
for (int i = 0; i < minigameplayeramount; i++)
{
    if (scores[i] > highestscore)
    {
        GD.Print("highestscore updated");
        highestscore = scores[i];
    }
}

// Find the player(s) with the highest score
for (int i = 0; i < minigameplayeramount; i++)
{
    GD.Print("checking which player has highest score");

    if (highestscore == scores[i])
    {
        // Print player information (e.g., their name, score, etc.)
        GD.Print($"Player {i} has the highest score: {scores[i]}");
	

        // Assign the player with the highest score as the winner
        GlobalVariables.Instance.Winner = i;   

        
        break;
    }
	
}
}}

