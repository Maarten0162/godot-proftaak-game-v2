using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class minigame5 : Control
{
    private struct Vraag
    {
        public string VraagText;
        public List<string> Antwoorden;
        public string CorrectAntwoord;
    }

    private List<Vraag> vragen = new List<Vraag>
    {
        new Vraag { VraagText = "Wie is het hoofdpersonage van Peaky Blinders?", Antwoorden = new List<string> { "Thomas Shelby", "Arthur Shelby", "Polly Gray", "John Shelby" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Waar speelt Peaky Blinders zich af?", Antwoorden = new List<string> { "London", "Birmingham", "Manchester", "Liverpool" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "In welk tijdperk speelt Peaky Blinders zich af?", Antwoorden = new List<string> { "1890s", "1920s", "1940s", "1960s" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Wie speelt de rol van Thomas Shelby in Peaky Blinders?", Antwoorden = new List<string> { "Cillian Murphy", "Tom Hardy", "Finn Cole", "Sam Neill" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Wat is de naam van het boek dat Tommy Shelby leest in seizoen 1?", Antwoorden = new List<string> { "The Great Gatsby", "The Count of Monte Cristo", "The Book of Tommy", "The Bible" }, CorrectAntwoord = "Y" },
        new Vraag { VraagText = "Wie is de rivaal van de Shelby familie in seizoen 1?", Antwoorden = new List<string> { "Alfie Solomons", "Billy Kimber", "Darby Sabini", "Luca Changretta" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Wat is de naam van de pub die de Shelby familie bezit?", Antwoorden = new List<string> { "The Garrison Pub", "The Shelby Pub", "The Crown Pub", "The Peaky Pub" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Wie speelt de rol van Polly Gray in Peaky Blinders?", Antwoorden = new List<string> { "Annabelle Wallis", "Sophie Rundle", "Helen McCrory", "Natasha O'Keeffe" }, CorrectAntwoord = "X" },
        new Vraag { VraagText = "In welk seizoen wordt de naam 'Peaky Blinders' officieel genoemd?", Antwoorden = new List<string> { "Seizoen 1", "Seizoen 2", "Seizoen 3", "Seizoen 4" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Welke persoon helpt de Shelby familie met illegale activiteiten in seizoen 2?", Antwoorden = new List<string> { "Arthur Shelby", "Michael Gray", "Alfie Solomons", "John Shelby" }, CorrectAntwoord = "X" },
        new Vraag { VraagText = "Hoe heet de vrouw die Tommy Shelby trouwt in seizoen 4?", Antwoorden = new List<string> { "Grace Burgess", "Linda Shelby", "Polly Gray", "Lizzie Stark" }, CorrectAntwoord = "Y" },
        new Vraag { VraagText = "Wie gaat er dood in Seioen 6?", Antwoorden = new List<string> { "Thomas Shelby", "Billy Grade", "Arthur Shelby", "Lizzie Stark" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Wie Vermoorde Grace Shelby?", Antwoorden = new List<string> { "Polly Gray", "Luca Changretta", "Vincente Changretta", "Billy Kimber" }, CorrectAntwoord = "X" },
        new Vraag { VraagText = "In welk seizoen zijn de Peaky Blinders opgepakt?", Antwoorden = new List<string> { "5", "3", "2", "4" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Buiten Thomas, Arthur, Polly en Ada, Welk karakter heeft de meeste afleveringen mee gedraaid in de serie?", Antwoorden = new List<string> { "Thomas Shelby", "Luca Changretta", "Micheal Gray", "Charlie Strong" }, CorrectAntwoord = "Y" },
        new Vraag { VraagText = "Hoeveel Vershillende acteurs speelt de rol van Winston Churchill door de volledige show heen?", Antwoorden = new List<string> { "6", "2", "3", "1" }, CorrectAntwoord = "X" },
        new Vraag { VraagText = "Welk product produceerd Alfie Solomons in zijn fabriek", Antwoorden = new List<string> { "Tabak", "Cocaïne", "Bar Krukken", "Rum" }, CorrectAntwoord = "Y" },
        new Vraag { VraagText = "Wie maakt de muziek van Peaky Blinders?", Antwoorden = new List<string> { "Nick Cave", "Hans Zimmer", "John Powell", "John Williams" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Wat is het oficiele naam voor het bedrijf van de Peaky Blinders?", Antwoorden = new List<string> { "Peaky Blinders", "Shelby Company Limited", "The Garisson Pub", "Order of the Peaky Blinders" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Wat is de naam van het leger dat door Tommy Shelby wordt opgericht in seizoen 4?", Antwoorden = new List<string> { "The Red Brigade", "The Black Hand", "The Peaky Army", "The Birmingham Battalion" }, CorrectAntwoord = "X" }
    };

    private Vraag huidigeVraag;
    private int huidigeVraagIndex = -1;

    private int[] scores = { 0, 0, 0, 0 };
    private List<bool> heeftGeantwoord;
    private List<string> antwoorden;
    private int aantalGeantwoord = 0;
    private int snelsteSpeler = -1;

    private Label vraagLabel;
    private List<Label> spelerStatusLabels;
    private List<Label> spelerScoreLabels;
    private Label Uitleg;
    private Label Naam;
    private Sprite2D UitlegSprite;
    private Timer TimerUitleg;


    public override void _Ready()
    {
        vraagLabel = GetNode<Label>("VraagLabel");
        heeftGeantwoord = new List<bool>();
        antwoorden = new List<string>();
        // Verwijzingen naar spelerstatus- en scorelabels
        spelerStatusLabels = new List<Label>();
        spelerScoreLabels = new List<Label>();
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
        {
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player1/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player1/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");

        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
        {
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player2/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player2/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
        {
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player3/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player3/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
        {
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player4/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player4/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
       
        UpdateSpelerUI();

        Naam = GetNode<Label>("Naam");
        Naam.Text = "Quizz";
        Uitleg = GetNode<Label>("Uitleg");
        UitlegSprite = GetNode<Sprite2D>("UitlegSprite");
        TimerUitleg = GetNode<Timer>("TimerUitleg");

        UitlegSprite.Visible = true;
        TimerUitleg.WaitTime = 20.0f;
        TimerUitleg.OneShot = true;
        TimerUitleg.Start();

        TimerUitleg.Connect("timeout", new Callable(this, nameof(OnTimerTimeout1)));
    }

    private void OnTimerTimeout1()
    {
        UitlegSprite.Visible = false;
        Naam.Text = "";
        Uitleg.Text = "";
        KiesVolgendeVraag();
    }

    private void KiesVolgendeVraag()
    {   for(int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++){
        heeftGeantwoord[i] = false;
    }   
     aantalGeantwoord = 0;
    
        if (huidigeVraagIndex >= 3)
        {
            BepaalWinnaar();
            return;
        }

        huidigeVraagIndex++;
        int randomVraagIndex = (int)(GD.Randi() % vragen.Count);

        huidigeVraag = vragen[randomVraagIndex];
        vragen.RemoveAt(randomVraagIndex);

        
       
        snelsteSpeler = -1;

        ToonVraag();
        UpdateSpelerUI();
    }

    private void ToonVraag()
    {
        vraagLabel.Text = $"{huidigeVraag.VraagText}\n" +
                          $"(A) = {huidigeVraag.Antwoorden[0]}\n" +
                          $"(B) = {huidigeVraag.Antwoorden[1]}\n" +
                          $"(X) = {huidigeVraag.Antwoorden[2]}\n" +
                          $"(Y) = {huidigeVraag.Antwoorden[3]}";
    }

    private void UpdateSpelerUI()
    {
        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {
            spelerStatusLabels[i].Text = $"Status: {(heeftGeantwoord[i] ? "Gekozen" : "Nog niet gekozen")}";
            spelerScoreLabels[i].Text = $"Score: {scores[i]}";
        }
    }

    private void RegistreerAntwoord(int speler, string antwoord)
    {
        if (heeftGeantwoord[speler])
            return;

        heeftGeantwoord[speler] = true;
        antwoorden[speler] = antwoord;
        aantalGeantwoord++;

        if (antwoord == huidigeVraag.CorrectAntwoord && snelsteSpeler == -1)
        {
            snelsteSpeler = speler;
        }

        UpdateSpelerUI();

        if (aantalGeantwoord == GlobalVariables.Instance.playersalive.Count)
        {
            ToonCorrectAntwoord();
        }
    }

    private void ToonCorrectAntwoord()
    {
        vraagLabel.Text += $"\nHet juiste antwoord was: {huidigeVraag.CorrectAntwoord}";

        if (snelsteSpeler != -1 && antwoorden[snelsteSpeler] == huidigeVraag.CorrectAntwoord)
        {
            scores[snelsteSpeler] += GlobalVariables.Instance.playersalive.Count - 1;
        }

        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {
            if (heeftGeantwoord[i] && antwoorden[i] == huidigeVraag.CorrectAntwoord && i != snelsteSpeler)
            {
                scores[i] += 1;
            }
        }

        UpdateSpelerUI();
        GetTree().CreateTimer(3).Connect("timeout", Callable.From(KiesVolgendeVraag));
    }

    private void BepaalWinnaar()
    {
        int hoogsteScore = 0;
        List<int> winnaars = new List<int>();

        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] > hoogsteScore)
            {
                hoogsteScore = scores[i];
                winnaars.Clear();
                winnaars.Add(i + 1);
            }
            else if (scores[i] == hoogsteScore)
            {
                winnaars.Add(i + 1);
            }
        }
            for( int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++){
                if(scores[i] == hoogsteScore){
                    GlobalVariables.Instance.Winner = i;
                }
            }
            //HIER MOET EEN WINAAR NOG KOMEN IDK WAT DIT IS
            GlobalVariables.Instance.SwitchToMainBoard();
        vraagLabel.Text = $"Winnaar(s): Speler {string.Join(", Speler ", winnaars)}";
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("A_1")) RegistreerAntwoord(0, "A");
        if (@event.IsActionPressed("B_1")) RegistreerAntwoord(0, "B");
        if (@event.IsActionPressed("X_1")) RegistreerAntwoord(0, "X");
        if (@event.IsActionPressed("Y_1")) RegistreerAntwoord(0, "Y");

        if (@event.IsActionPressed("A_2")) RegistreerAntwoord(1, "A");
        if (@event.IsActionPressed("B_2")) RegistreerAntwoord(1, "B");
        if (@event.IsActionPressed("X_2")) RegistreerAntwoord(1, "X");
        if (@event.IsActionPressed("Y_2")) RegistreerAntwoord(1, "Y");

        if (@event.IsActionPressed("A_3")) RegistreerAntwoord(2, "A");
        if (@event.IsActionPressed("B_3")) RegistreerAntwoord(2, "B");
        if (@event.IsActionPressed("X_3")) RegistreerAntwoord(2, "X");
        if (@event.IsActionPressed("Y_3")) RegistreerAntwoord(2, "Y");

        if (@event.IsActionPressed("A_4")) RegistreerAntwoord(3, "A");
        if (@event.IsActionPressed("B_4")) RegistreerAntwoord(3, "B");
        if (@event.IsActionPressed("X_4")) RegistreerAntwoord(3, "X");
        if (@event.IsActionPressed("Y_4")) RegistreerAntwoord(3, "Y");
    }
}
