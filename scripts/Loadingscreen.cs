using Godot;
using System;

public partial class Loadingscreen : Node2D
{
    private Label labelMinigame;
    private Label uitleg;
    private Timer timer;

    private int selectedGame; // Om het geselecteerde spel bij te houden

    [Signal]
    public delegate void LoadingCompleteEventHandler();

    public override void _Ready()
    {
        Random rnd = new Random();
        int RandomGetal = rnd.Next(1, 6); // 0-5 is exclusief 6, dus dit moet zijn 1, 6.
        RandomGetal = 1;
        labelMinigame = GetNode<Label>("LabelMinigame");
        uitleg = GetNode<Label>("Uitleg");
        timer = GetNode<Timer>("Timer");

        // Start the timer (60 sec)
        timer.Start(30.0f);

        if (RandomGetal == 1)
        {
            labelMinigame.Text = "Paarden Race";
            uitleg.Text = "In dit spel moet je er voor zorgen dat jouw paard zo ver mogelijk komt. \nWie het verste komt wint.";
            selectedGame = 1;
        }
        else if (RandomGetal == 2)
        {
            labelMinigame.Text = "Pesisie test";
            uitleg.Text = "Uitleg van de minigame...";
            selectedGame = 2;
        }
        else if (RandomGetal == 3)
        {
            labelMinigame.Text = "Reactie snelheid";
            uitleg.Text = "In dit spel moet je zo snel mogelijk reageren om als eerste Thomas Shelby neer te schieten. \nWacht op het signaal en schiet vervolgens zo snel mogelijk op de A knop om als eerste te schieten. \nDegene die als eerste reageert wint.";
            selectedGame = 3;
        }
        else if (RandomGetal == 4)
        {
            labelMinigame.Text = "Steen papier schaar";
            uitleg.Text = "Dit is het klassieke steen, papier schaar.";
            selectedGame = 4;
        }
        else
        {
            labelMinigame.Text = "Steen papier schaar";
            uitleg.Text = "Dit is het klassieke steen, papier schaar.";
            selectedGame = 5;
        }

        // Connect the timer timeout signal to the OnTimerTimeout method
        timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
    }

    private void OnTimerTimeout()
    {
        switch (selectedGame)
        {
            case 1:
                GlobalVariables.Instance.SwitchToMinigame1();
                break;
            case 2:
                GlobalVariables.Instance.SwitchToMinigame2();
                break;
            case 3:
                GlobalVariables.Instance.SwitchToMinigame3();
                break;
            case 4:
                GlobalVariables.Instance.SwitchToMinigame4();
                break;
            case 5:
                GlobalVariables.Instance.SwitchToMinigame5();
                break;
        }

        labelMinigame.Visible = false;
        uitleg.Visible = false;
    }
}
