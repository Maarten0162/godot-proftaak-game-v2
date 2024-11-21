using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public partial class Minigame2 : Node2D
{
    // Lijst van spelers en hun labels
    private string[] players = { "Player 1", "Player 2", "Player 3", "Player 4" };
    private Dictionary<string, float> reactionTimes = new Dictionary<string, float>();
    private bool[] playerHasPressedButton;  // Huidige staat van de spelers (of ze al hebben gedrukt)

    // Timer en status
    private DateTime startTime;
    private bool isGameActive = true;
    private int countdownSeconds = 1;
    private bool timerVisible = true;

    //loadingscreen
    private Label Uitleg;
    private Label Naam;
    private Sprite2D UitlegSprite;
    private Timer TimerUitleg;
    private Sprite2D ASprite;

    // UI-elementen
    private Label TimerLabel;
    private Label LabelWinnaar;
    private Timer countdownTimer;
    
    private Label[] playerLabels = new Label[4];  // Verwijzingen naar de labels van de spelers
    private ColorRect[] playerColorRects = new ColorRect[4];  // Verwijzingen naar de ColorRect's

    

    public override void _Ready()
    { playerHasPressedButton = new bool[GlobalVariables.Instance.playersalive.Count];
        
        // Verwijzingen naar UI-elementen
        TimerLabel = GetNodeOrNull<Label>("GameUI/TimerLabel");
        LabelWinnaar = GetNodeOrNull<Label>("GameUI/CenterContainer/LabelWinnaar");

      if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1")){
            playerLabels[0] = GetNodeOrNull<Label>($"GameUI/LabelSpeler{0 + 1}");
            playerColorRects[0] = GetNodeOrNull<ColorRect>($"GameUI/MarginContainer{0 + 1}/TextureRect/ColorRect");
      }
            if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2")){
            playerLabels[1] = GetNodeOrNull<Label>($"GameUI/LabelSpeler{1 + 1}");
            playerColorRects[1] = GetNodeOrNull<ColorRect>($"GameUI/MarginContainer{1 + 1}/TextureRect/ColorRect");
      }
            if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3")){
            playerLabels[2] = GetNodeOrNull<Label>($"GameUI/LabelSpeler{2 + 1}");
            playerColorRects[2] = GetNodeOrNull<ColorRect>($"GameUI/MarginContainer{2 + 1}/TextureRect/ColorRect");
      }
            if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4")){
            playerLabels[3] = GetNodeOrNull<Label>($"GameUI/LabelSpeler{3 + 1}");
            playerColorRects[3] = GetNodeOrNull<ColorRect>($"GameUI/MarginContainer{3 + 1}/TextureRect/ColorRect");
      }
        

        // Stel de countdown timer in 
        countdownTimer = new Timer(); 
        countdownTimer.WaitTime = 1.0f;
        countdownTimer.OneShot = false;
        countdownTimer.Timeout += OnCountdownTimeout;
        AddChild(countdownTimer); //gaat dus van 0 naar 1, naar 2, naar 3. daarna zie je hem niet meer.

        
        Naam = GetNode<Label>("Naam");
        Naam.Text = "Perfect Timing";
        Uitleg = GetNode<Label>("Uitleg");
        UitlegSprite = GetNode<Sprite2D>("UitlegSprite");
        TimerUitleg = GetNode<Timer>("TimerUitleg");
        ASprite = GetNode<Sprite2D>("ASprite");
        
        ASprite.Visible = true;
        UitlegSprite.Visible = true;
        TimerUitleg.WaitTime = 10.0f;
        TimerUitleg.OneShot = true;
        TimerUitleg.Start();

        TimerUitleg.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
    }

    private void OnTimerTimeout()
    {
        UitlegSprite.Visible = false;
        ASprite.Visible = false;
        Naam.Text = "";
        Uitleg.Text = "";
        StartCountdown();
    }

    private void StartCountdown()
    {
        countdownSeconds = 1;
        countdownTimer.Start();
        TimerLabel.Show();
		GD.Print("Countdown started"); // Debug
    }

    private void OnCountdownTimeout()
    {
        GD.Print("OnCountdownTimeout called");
		TimerLabel.Text = countdownSeconds.ToString();

        if (countdownSeconds >= 4) //bij 3 seconden gaat hij weg
        {
            TimerLabel.Hide();
            countdownTimer.Stop();
            timerVisible = false;
            startTime = DateTime.Now.AddSeconds(-4); // Starttijd aanpassen om de reactietijd correct te meten dus 7 seconden zie je niet
        }
        else // anders telt hij gwn 1 seconden er bij heel de tijd
        {
            countdownSeconds++;
        }
    }

    public override void _Process(double delta)
    {
        // wanneer een speler op de knop drukt
        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {
            if (Input.IsActionJustPressed($"A_{i + 1}") && !playerHasPressedButton[i])
            {
                OnPlayerPressed(i);
            }
        }
    }

    private void OnPlayerPressed(int playerIndex)
    {
        if (isGameActive && !timerVisible)
        {
            // Zet de knop als ingedrukt en dat de tijd niet veranderd
            playerHasPressedButton[playerIndex] = true;

            // Bereken de reactietijd dus 
            float reactionTime = (float)(DateTime.Now - startTime).TotalSeconds;
            reactionTimes[players[playerIndex]] = reactionTime;

            // Verander de kleur van de ColorRect dus laat rodd kleurtje zien
            ColorRect colorRect = playerColorRects[playerIndex];
            colorRect.Visible = true;
            

            // Update de UI
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Controleer of alle spelers hebben gedrukt
        bool allPlayersPressed = true;
        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {
            if (!playerHasPressedButton[i])
            {
                allPlayersPressed = false;
                break;
            }
        }

        // Als alle spelers hebben gedrukt, toon de reactietijd en de winnaar, dus degene die het dichstebij 10 zit
        if (allPlayersPressed)
        {
            for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
            {
                playerLabels[i].Text = $"{players[i]} - Reactietijd: {reactionTimes[players[i]]:F2} sec";
                
            }

            CheckWinner();
        }
    }

    private void ReturnToMainScene()
    {
        GlobalVariables.Instance.SwitchToMainBoard();
    }
    private async void CheckWinner()
    {
        // Start met de eerste speler
        string closestPlayer = players[0];
        float closestTimeDiff = Math.Abs(reactionTimes[players[0]] - 10);

        // Vergelijk elke speler's reactietijd met 10 seconden
        for (int i = 1; i < GlobalVariables.Instance.playersalive.Count; i++)
        {
            float timeDiff = Math.Abs(reactionTimes[players[i]] - 10);

            // Als de huidige tijd dichter bij 10 seconden ligt, update dan de winnaar
            if (timeDiff < closestTimeDiff)
            {
                closestTimeDiff = timeDiff;
                closestPlayer = players[i];
                GlobalVariables.Instance.Winner = i;
            }
        }

        // Toon de winnaar en het verschil
        LabelWinnaar.Text = $"{closestPlayer} is het dichtst bij 10 sec met {closestTimeDiff:F2} sec verschil";
        LabelWinnaar.SelfModulate = new Color(1, 1, 0);  // Geel super mooi kleurtje voor de winnaar
        LabelWinnaar.Show();
        await WaitForSeconds(3);
        GetTree().CreateTimer(3f).Connect("timeout", new Callable(this, nameof(ReturnToMainScene)));
    }
    private async Task WaitForSeconds(float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
	}
  
}
  
