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
        Random rnd = new Random();
        int Randomgetal = rnd.Next(0, 6);

        labelMinigame = GetNode<Label>("LabelMinigame");
        uitleg = GetNode<Label>("Uitleg");
        timer = GetNode<Timer>("Timer");

        // Start the timer (60 sec)
        timer.Start(60.0f);

        if (Randomgetal == 1)
        {
            // Connect the timer timeout signal to a method
            timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
            labelMinigame.Text = "Paarden Race";
            uitleg.Text = "In dit spel moet je er voor zorgen dat jouw paart zo ver mogelijk komt. \nWie het verste komt wint.";
        }
        else if (Randomgetal == 2)
        {
            timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
            labelMinigame.Text = "Pesisie test";
            uitleg.Text = "Uitleg van de minigame...";
        }
        else if (Randomgetal == 3)
        {
            timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
            labelMinigame.Text = "Reactie snelheid";
            uitleg.Text = "In dit spel moet je zo snel mogelijk reageren om als eerste thomas shelby neer te schieten. \nWacht op het signaal en schiet vervolgens zo snel mogelijk op de A knop knop om als eerste te schieten. \nDegenen die als eerste reageerd wint. ";
        }
        else if (Randomgetal == 4 && GlobalVariables.Instance.playersalive.Count == 4)
        {
            timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
            labelMinigame.Text = "Steen papier schaar";
            uitleg.Text = "Dit is het klassieke steen, papier schaar";
        }
    }

    private void OnTimerTimeout()
    {
        labelMinigame.Visible = false;
        uitleg.Visible = false;
        // GlobalVariables.Instance.SwitchToMinigame();
    }
}
