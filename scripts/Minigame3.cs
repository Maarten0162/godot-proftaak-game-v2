using Godot;
using System;

public partial class Minigame3 : Node2D
{
     private float[] reactietijden = new float[4];
    private bool[] heeftGereageerd = new bool[4];
    private Timer spelTimer;
    private Timer reactieTimer;
    private Random random = new Random();
    private Label[] reactieLabels = new Label[4];
    private Label winnaarLabel;
    private Label winnaarResultaatLabel;  // Nieuw label voor de winnaar
    private Sprite2D[] preReactSprites = new Sprite2D[4];
    private Sprite2D[] postReactSprites = new Sprite2D[4];
    private bool spelActief = false;

    public override void _Ready()
    {
        for (int i = 0; i < 4; i++)
        {
            reactieLabels[i] = GetNode<Label>($"Label{i + 1}");
            preReactSprites[i] = GetNode<Sprite2D>($"PreReactSprite{i + 1}");
            postReactSprites[i] = GetNode<Sprite2D>($"PostReactSprite{i + 1}");
        }
        winnaarLabel = GetNode<Label>("WinnaarLabel");
        winnaarResultaatLabel = GetNode<Label>("WinnaarResultaatLabel"); // Nieuwe label reference

        spelTimer = GetNode<Timer>("SpelTimer");
        reactieTimer = GetNode<Timer>("ReactieTimer");

        spelTimer.Connect("timeout", new Callable(this, nameof(StartReactieFase)));
        reactieTimer.Connect("timeout", new Callable(this, nameof(EindeReactieFase)));
        
        StartSpel();
    }

    private void StartSpel()
    {
        spelActief = false;
        heeftGereageerd = new bool[4];
        reactietijden = new float[4];
        winnaarLabel.Text = "Wacht tot het signaal...";
        winnaarResultaatLabel.Text = "";  // Maak het winnaarresultaat leeg
        SetSpritesVisibility(true, false);

        float startTijd = (float)random.NextDouble() * 5f;
        spelTimer.Start(startTijd);
    }

    private void StartReactieFase()
    {
        spelActief = true;
        winnaarLabel.Text = "Schiet nu!";
        reactieTimer.Start(); // Start de timer zonder tijdslimiet, we gebruiken hem alleen om tijd te meten
    }

    public override void _Process(double delta)
    {
        if (spelActief)
        {
            // Nu reageren de knoppen als het spel actief is en de reactie timer draait
            CheckPlayerInput("button_press_space", 0);
            CheckPlayerInput("button_press_w", 1);
            CheckPlayerInput("button_press_e", 2);
            CheckPlayerInput("button_press_r", 3);
        }
    }

    private void CheckPlayerInput(string action, int index)
    {
        // Controleer of de speler de juiste knop indrukt en nog niet heeft gereageerd
        if (Input.IsActionJustPressed(action) && !heeftGereageerd[index])
        {
            RegisterReaction(index);
        }
    }

    private void RegisterReaction(int spelerIndex)
    {
        heeftGereageerd[spelerIndex] = true;
        float reactietijd = (float)reactieTimer.TimeLeft; // Converteer double naar float
        reactietijden[spelerIndex] = reactietijd;
        reactieLabels[spelerIndex].Text = $"Speler {spelerIndex + 1} reageerde in {Math.Round(reactietijden[spelerIndex], 2)} seconden.";
        SetSpritesVisibility(false, true, spelerIndex);
    }

    private void EindeReactieFase()
    {
        spelActief = false;
        reactieTimer.Stop(); // Stop de timer
        int winnaarIndex = GetWinnerIndex();
        
        if (winnaarIndex >= 0)
        {
            winnaarResultaatLabel.Text = $"Speler {winnaarIndex + 1} wint met een reactietijd van {Math.Round(reactietijden[winnaarIndex], 2)} seconden!";
        }
        else
        {
            winnaarResultaatLabel.Text = "Niemand heeft op tijd gereageerd.";
        }

        // StartSpel(); // Verwijder deze lijn om het spel niet automatisch opnieuw te starten
    }

    private int GetWinnerIndex()
    {
        float snelsteTijd = float.MaxValue;
        int winnaarIndex = -1;
        
        for (int i = 0; i < 4; i++)
        {
            if (heeftGereageerd[i] && reactietijden[i] < snelsteTijd)
            {
                snelsteTijd = reactietijden[i];
                winnaarIndex = i;
            }
        }

        return winnaarIndex;
    }

    private void SetSpritesVisibility(bool preReact, bool postReact, int? specificIndex = null)
    {
        for (int i = 0; i < 4; i++)
        {
            if (specificIndex.HasValue && specificIndex.Value != i)
                continue;

            preReactSprites[i].Visible = preReact;
            postReactSprites[i].Visible = postReact;
        }
    }
}
