using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Minigame3 : Node2D
{
    private float[] reactietijden = new float[4];
    private bool[] heeftGereageerd = new bool[4];
    private Timer spelTimer;
    private Timer reactieTimer;
    private Random random = new Random();
    private Label[] reactieLabels = new Label[4];
    private Label winnaarLabel;
    private Label winnaarResultaatLabel;
    private List<Sprite2D> preReactSprites = new List<Sprite2D>();
    private List<Sprite2D> postReactSprites = new List<Sprite2D>();

    private bool spelActief = false;
    int minigameplayeramount;

    public override void _Ready()
    {
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite1"));
            preReactSprites.Add(GetNode<Sprite2D>("PostReactSprite1"));
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite2"));
            preReactSprites.Add(GetNode<Sprite2D>("PostReactSprite2"));
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite3"));
            preReactSprites.Add(GetNode<Sprite2D>("PostReactSprite3"));
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite4"));
            preReactSprites.Add(GetNode<Sprite2D>("PostReactSprite4"));
            minigameplayeramount++;
        }

        for (int i = 0; i < minigameplayeramount; i++)
        {
            reactieLabels[i] = GetNode<Label>($"Label{i + 1}");
            preReactSprites[i] = GetNode<Sprite2D>($"PreReactSprite{i + 1}");
            postReactSprites[i] = GetNode<Sprite2D>($"PostReactSprite{i + 1}");
        }
        winnaarLabel = GetNode<Label>("WinnaarLabel");
        winnaarResultaatLabel = GetNode<Label>("WinnaarResultaatLabel"); 
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
        winnaarResultaatLabel.Text = ""; 
        SetSpritesVisibility(true, false);

        float startTijd = (float)random.NextDouble() * 5f;
        spelTimer.Start(startTijd);
    }

    private void StartReactieFase()
    {
        spelActief = true;
        winnaarLabel.Text = "Schiet nu!";
        reactieTimer.Start(); 
    }

    public override void _Process(double delta)
    {
        if (spelActief)
        {
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
        reactieTimer.Stop();
        int winnaarIndex = GetWinnerIndex();
        
        if (winnaarIndex >= 0)
        {
            winnaarResultaatLabel.Text = $"Speler {winnaarIndex + 1} wint met een reactietijd van {Math.Round(reactietijden[winnaarIndex], 2)} seconden!";
        }
        else
        {
            winnaarResultaatLabel.Text = "Niemand heeft op tijd gereageerd.";
        }
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
