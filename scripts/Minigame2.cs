using Godot;
using System;
using System.Collections.Generic;

public partial class Minigame2 : Node2D
{
    // Lijst van spelers en hun labels
    private string[] players = { "Player 1", "Player 2", "Player 3", "Player 4" };
    private Dictionary<string, float> reactionTimes = new Dictionary<string, float>();
    private bool[] playerHasPressedButton = new bool[4];  // Huidige staat van de spelers (of ze al hebben gedrukt)

    // Timer en status
    private DateTime startTime;
    private bool isGameActive = true;
    private int countdownSeconds = 1;
    private bool timerVisible = true;

    // UI-elementen
    private Label TimerLabel;
    private Label LabelWinnaar;
    private Timer countdownTimer;
    private Label[] playerLabels = new Label[4];  // Verwijzingen naar de labels van de spelers
    private ColorRect[] playerColorRects = new ColorRect[4];  // Verwijzingen naar de ColorRect's

    public override void _Ready()
    {
        // Verwijzingen naar UI-elementen
        TimerLabel = GetNodeOrNull<Label>("GameUI/TimerLabel");
        LabelWinnaar = GetNodeOrNull<Label>("GameUI/CenterContainer/LabelWinnaar");

        // Verbind de labels en colorrects(=roodkleurtje) van de spelers
        for (int i = 0; i < players.Length; i++)
        {
            playerLabels[i] = GetNodeOrNull<Label>($"GameUI/LabelSpeler{i + 1}");
            playerColorRects[i] = GetNodeOrNull<ColorRect>($"GameUI/MarginContainer{i + 1}/TextureRect/ColorRect");

        }

        // Stel de countdown timer in 
        countdownTimer = new Timer(); 
        countdownTimer.WaitTime = 1.0f;
        countdownTimer.OneShot = false;
        countdownTimer.Timeout += OnCountdownTimeout;
        AddChild(countdownTimer); //gaat dus van 0 naar 1, naar 2, naar 3. daarna zie je hem niet meer.

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
        for (int i = 0; i < players.Length; i++)
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
        for (int i = 0; i < players.Length; i++)
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
            for (int i = 0; i < players.Length; i++)
            {
                playerLabels[i].Text = $"{players[i]} - Reactietijd: {reactionTimes[players[i]]:F2} sec";
                
            }

            CheckWinner();
        }
    }

    private void CheckWinner()
    {
        // Start met de eerste speler
        string closestPlayer = players[0];
        float closestTimeDiff = Math.Abs(reactionTimes[players[0]] - 10);

        // Vergelijk elke speler's reactietijd met 10 seconden
        for (int i = 1; i < players.Length; i++)
        {
            float timeDiff = Math.Abs(reactionTimes[players[i]] - 10);

            // Als de huidige tijd dichter bij 10 seconden ligt, update dan de winnaar
            if (timeDiff < closestTimeDiff)
            {
                closestTimeDiff = timeDiff;
                closestPlayer = players[i];
            }
        }

        // Toon de winnaar en het verschil
        LabelWinnaar.Text = $"{closestPlayer} is het dichtst bij 10 sec met {closestTimeDiff:F2} sec verschil";
        LabelWinnaar.SelfModulate = new Color(1, 1, 0);  // Geel super mooi kleurtje voor de winnaar
        LabelWinnaar.Show();
    }
}
