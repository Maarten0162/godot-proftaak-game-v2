using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class Minigame3 : Node2D
{
    private float[] reactietijden;
    private bool[] heeftGereageerd;
    private float startReactieTijd; // Variabele om de starttijd van de reactie op te slaan
    private Random random = new Random();
    private Label[] reactieLabels;
    private Label winnaarLabel;
    private Label winnaarResultaatLabel;
    private Label KnopLabel;
    private Label Uitleg;
    private Label Naam;
    private List<Sprite2D> preReactSprites = new List<Sprite2D>();
    private List<Sprite2D> postReactSprites = new List<Sprite2D>();
    private Sprite2D ASprite;
    private Sprite2D BSprite;
    private Sprite2D XSprite;
    private Sprite2D YSprite;
    private TextureRect inputbutton;
    private int minigameplayeramount;
    private bool[] activePlayers; // Array om actieve spelers bij te houden

    private bool spelActief = false;
    private int RandomKnop;
    private Sprite2D UitlegSprite;
    private Timer TimerUitleg;

    public override void _Ready()
    {
        inputbutton = GetNode<TextureRect>("InputButton");
        minigameplayeramount = 0;
        activePlayers = new bool[4]; // Initialiseer de actieve spelers array

        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite1"));
            postReactSprites.Add(GetNode<Sprite2D>("PostReactSprite1"));
            activePlayers[0] = true; // Markeer speler 1 als actief
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite2"));
            postReactSprites.Add(GetNode<Sprite2D>("PostReactSprite2"));
            activePlayers[1] = true; // Markeer speler 2 als actief
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite3"));
            postReactSprites.Add(GetNode<Sprite2D>("PostReactSprite3"));
            activePlayers[2] = true; // Markeer speler 3 als actief
            minigameplayeramount++;
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
        {
            preReactSprites.Add(GetNode<Sprite2D>("PreReactSprite4"));
            postReactSprites.Add(GetNode<Sprite2D>("PostReactSprite4"));
            activePlayers[3] = true; // Markeer speler 4 als actief
            minigameplayeramount++;
        }

        reactietijden = new float[minigameplayeramount];
        heeftGereageerd = new bool[minigameplayeramount];
        reactieLabels = new Label[minigameplayeramount];

        for (int i = 0; i < minigameplayeramount; i++)
        {
            reactieLabels[i] = GetNode<Label>($"Label{i + 1}");
        }

        winnaarLabel = GetNode<Label>("WinnaarLabel");
        winnaarResultaatLabel = GetNode<Label>("WinnaarResultaatLabel");
        KnopLabel = GetNode<Label>("KnopLabel");

        Naam = GetNode<Label>("Naam");
        Naam.Text = "Boom!";
        Uitleg = GetNode<Label>("Uitleg");
        UitlegSprite = GetNode<Sprite2D>("UitlegSprite");
        TimerUitleg = GetNode<Timer>("TimerUitleg");
        ASprite = GetNode<Sprite2D>("ASprite");
        BSprite = GetNode<Sprite2D>("BSprite");
        XSprite = GetNode<Sprite2D>("XSprite");
        YSprite = GetNode<Sprite2D>("YSprite");

        ASprite.Visible = true;
        BSprite.Visible = true;
        XSprite.Visible = true;
        YSprite.Visible = true;
        UitlegSprite.Visible = true;
        TimerUitleg.WaitTime = 10.0f;
        TimerUitleg.OneShot = true;
        TimerUitleg.Start();
        
        TimerUitleg.Connect("timeout", new Callable(this, nameof(OnTimerTimeout1)));
    }

    private void OnTimerTimeout1()
    {
        UitlegSprite.Visible = false;
        ASprite.Visible = false;
        BSprite.Visible = false;
        XSprite.Visible = false;
        YSprite.Visible = false;
        Naam.Text = "";
        Uitleg.Text = "";
        StartSpel();
    }


    public void StartSpel()
    {
        spelActief = false;
        Array.Clear(heeftGereageerd, 0, heeftGereageerd.Length);
        Array.Clear(reactietijden, 0, reactietijden.Length);

        winnaarLabel.Text = "Wacht tot het signaal...";
        winnaarResultaatLabel.Text = "";
        SetSpritesVisibility(true, false);

        float startTijd = (float)random.NextDouble() * 5f;
        GetTree().CreateTimer(startTijd).Connect("timeout", new Callable(this, nameof(StartReactieFase)));
    }

    public void StartReactieFase()
    {
        spelActief = true;
        winnaarLabel.Text = "Schiet nu!";
        startReactieTijd = Time.GetTicksMsec(); // Starttijd van de reactie opslaan
        GD.Print("ReactieFase gestart");

        Randombepalen();
    }

    public void Randombepalen()
    {
        Random rnd = new Random();
        RandomKnop = rnd.Next(1, 5);
        GD.Print($"Random gekozen knop: {RandomKnop}"); // Debugging print 
    }

    public override void _Process(double delta)
    {
        if (spelActief)
        {
            if (RandomKnop == 1)
            {
                KnopLabel.Text = "Druk op ";
                inputbutton.Texture = GD.Load<Texture2D>("res://assets/hud/XBOX BUTTONS/Digital Buttons/ABXY/button_xbox_digital_a_1.png");
                inputbutton.Show();
                CheckPlayerInput("A_1", 0);
                CheckPlayerInput("A_2", 1);
                CheckPlayerInput("A_3", 2);
                CheckPlayerInput("A_4", 3);
            }
            else if (RandomKnop == 2)
            {
                KnopLabel.Text = "Druk op ";
                inputbutton.Texture = GD.Load<Texture2D>("res://assets/hud/XBOX BUTTONS/Digital Buttons/ABXY/button_xbox_digital_b_1.png");
                inputbutton.Show();
                CheckPlayerInput("B_1", 0);
                CheckPlayerInput("B_2", 1);
                CheckPlayerInput("B_3", 2);
                CheckPlayerInput("B_4", 3);
            }
            else if (RandomKnop == 3)
            {
                KnopLabel.Text = "Druk op ";
                inputbutton.Texture = GD.Load<Texture2D>("res://assets/hud/XBOX BUTTONS/Digital Buttons/ABXY/button_xbox_digital_x_1.png");
                inputbutton.Show();
                CheckPlayerInput("X_1", 0);
                CheckPlayerInput("X_2", 1);
                CheckPlayerInput("X_3", 2);
                CheckPlayerInput("X_4", 3);
            }
            else
            {
                KnopLabel.Text = "Druk op ";
                inputbutton.Texture = GD.Load<Texture2D>("res://assets/hud/XBOX BUTTONS/Digital Buttons/ABXY/button_xbox_digital_y_1.png");
                inputbutton.Show();
                CheckPlayerInput("Y_1", 0);
                CheckPlayerInput("Y_2", 1);
                CheckPlayerInput("Y_3", 2);
                CheckPlayerInput("Y_4", 3);
            }
        }
    }

    private void CheckPlayerInput(string action, int index)
    {
        if (index >= 0 && index < activePlayers.Length && activePlayers[index] && Input.IsActionJustPressed(action) && !heeftGereageerd[index])
        {
            GD.Print($"Player {index + 1} pressed {action}");
            RegisterReaction(index);
        }
    }

    private void RegisterReaction(int spelerIndex)
    {
        if (spelerIndex >= 0 && spelerIndex < reactietijden.Length)
        {
            heeftGereageerd[spelerIndex] = true;
            float reactietijd = (Time.GetTicksMsec() - startReactieTijd) / 1000.0f; // Bereken de reactietijd
            reactietijden[spelerIndex] = reactietijd;
            reactieLabels[spelerIndex].Text = $"Speler {spelerIndex + 1} reageerde in {Math.Round(reactietijden[spelerIndex], 2)} seconden.";
            GD.Print($"Speler {spelerIndex + 1} geregistreerd met reactietijd: {reactietijd}");
            SetSpritesVisibility(false, true, spelerIndex);

            // Check if all active players have reacted
            if (heeftGereageerd.All(reactie => reactie))
            {
                ShowWinnerAndReturn();
            }
        }
    }

    private void ShowWinnerAndReturn()
    {
        int winnaarIndex = GetWinnerIndex();
        if (winnaarIndex != -1)
        {
            winnaarResultaatLabel.Text = $"Speler {winnaarIndex + 1} heeft gewonnen met een tijd van {Math.Round(reactietijden[winnaarIndex], 2)} seconden!";
        }
        else
        {
            winnaarResultaatLabel.Text = "Geen speler heeft gereageerd.";
        }

        // Wait for a short duration and switch to the main scene
        GetTree().CreateTimer(3f).Connect("timeout", new Callable(this, nameof(ReturnToMainScene)));
    }

    private void ReturnToMainScene()
    {
        GlobalVariables.Instance.SwitchToMainBoard();
    }

    private int GetWinnerIndex()
    {
        float snelsteTijd = float.MaxValue;
        int winnaarIndex = -1;

        for (int i = 0; i < reactietijden.Length; i++)
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
        for (int i = 0; i < preReactSprites.Count; i++)
        {
            if (specificIndex.HasValue && specificIndex.Value != i)
                continue;

            preReactSprites[i].Visible = preReact;
            postReactSprites[i].Visible = postReact;
        }
    }
    private async Task WaitForSeconds(float seconds)
    {
        await ToSignal(GetTree().CreateTimer(seconds), "timeout");
    }
}


