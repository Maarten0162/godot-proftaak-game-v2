using Godot;
using System;

public partial class Loadingscreen : Node2D
{
    private Label labelMinigame;
    private Label uitleg;
    private Timer timer;

    [Signal]
    public delegate void LoadingCompleteEventHandler();

    public override void _Ready()
    {   
        labelMinigame = GetNode<Label>("LabelMinigame");
        uitleg = GetNode<Label>("Uitleg");
        timer = GetNode<Timer>("Timer");

        // Set the text for the labels
        labelMinigame.Text = "Naam van de Minigame";
        uitleg.Text = "Uitleg van de minigame... In dit spel moet je er voor zorgen dat jouw paart zo ver mogelijk komt. \n Dit doe je door zo vaak mogelijk op de A knop op je controler te klikken.";

        // Connect the timer timeout signal to a method
        timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));

        // Start the timer (e.g., 3 seconds)
        timer.Start(60.0f);

        /*if (minigame)
        {
        labelMinigame.Text = "Paarden Race";
        uitleg.Text = "Uitleg van de minigame... In dit spel moet je er voor zorgen dat jouw paart zo ver mogelijk komt. \n Dit doe je door zo vaak mogelijk op de A knop op je controler te klikken.";
        }
        else if (true)
        {
            labelMinigame.Text = "Minigame2";
        uitleg.Text = "Uitleg van de minigame...";
        }
        else if (true)
        {
            labelMinigame.Text = "Minigame3";
        uitleg.Text = "Uitleg van de minigame... ";
        }
        else
        {
            labelMinigame.Text = "Minigame4";
        uitleg.Text = "Uitleg van de minigame...";
        }*/
    }

    private void OnTimerTimeout()
    {
        labelMinigame.Visible = false;
        uitleg.Visible = false;
        GlobalVariables.Instance.SwitchToMainBoard();
       
    }
}
