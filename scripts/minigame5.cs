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
        new Vraag { VraagText = "Wat is de naam van het boek dat Tommy Shelby leest in seizoen 1?", Antwoorden = new List<string> { "The Great Gatsby", "The Count of Monte Cristo", "The Book of Tommy", "The Bible" }, CorrectAntwoord = "D" },
        new Vraag { VraagText = "Wie is de rivaal van de Shelby familie in seizoen 1?", Antwoorden = new List<string> { "Alfie Solomons", "Billy Kimber", "Darby Sabini", "Luca Changretta" }, CorrectAntwoord = "B" },
        new Vraag { VraagText = "Wat is de naam van de pub die de Shelby familie bezit?", Antwoorden = new List<string> { "The Garrison Pub", "The Shelby Pub", "The Crown Pub", "The Peaky Pub" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Wie speelt de rol van Polly Gray in Peaky Blinders?", Antwoorden = new List<string> { "Annabelle Wallis", "Sophie Rundle", "Helen McCrory", "Natasha O'Keeffe" }, CorrectAntwoord = "C" },
        new Vraag { VraagText = "In welk seizoen wordt de naam 'Peaky Blinders' officieel genoemd?", Antwoorden = new List<string> { "Seizoen 1", "Seizoen 2", "Seizoen 3", "Seizoen 4" }, CorrectAntwoord = "A" },
        new Vraag { VraagText = "Welke persoon helpt de Shelby familie met illegale activiteiten in seizoen 2?", Antwoorden = new List<string> { "Arthur Shelby", "Michael Gray", "Alfie Solomons", "John Shelby" }, CorrectAntwoord = "C" },
        new Vraag { VraagText = "Hoe heet de vrouw die Tommy Shelby trouwt in seizoen 4?", Antwoorden = new List<string> { "Grace Burgess", "Linda Shelby", "Polly Gray", "Lizzie Stark" }, CorrectAntwoord = "D" },
        new Vraag { VraagText = "Wat is de naam van het leger dat door Tommy Shelby wordt opgericht in seizoen 4?", Antwoorden = new List<string> { "The Red Brigade", "The Black Hand", "The Peaky Army", "The Birmingham Battalion" }, CorrectAntwoord = "C" }
    };

    private Vraag huidigeVraag;
    private int huidigeVraagIndex = -1;

    private int[] scores = { 0, 0, 0, 0 };
    private List<bool> heeftGeantwoord ;
    private List<string> antwoorden ;
    private int aantalGeantwoord = 0;
    private int snelsteSpeler = -1;

    private Label vraagLabel;
    private List<Label> spelerStatusLabels;
    private List<Label> spelerScoreLabels;

    public override void _Ready()
    {
        vraagLabel = GetNode<Label>("VraagLabel");

        // Verwijzingen naar spelerstatus- en scorelabels
        spelerStatusLabels = new List<Label>();
        spelerScoreLabels = new List<Label>();
        heeftGeantwoord = new List<bool>();
        antwoorden = new List<string>();
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1"))
        {GD.Print("in player 1");
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player1/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player1/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player2"))
        {   GD.Print("in player 2");
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player2/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player2/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3"))
        {GD.Print("in player 3");
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player3/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player3/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }
        if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4"))
        {GD.Print("in player 4");
            spelerStatusLabels.Add(GetNode<Label>("MarginContainer_Player4/StatusLabel"));
            spelerScoreLabels.Add(GetNode<Label>("MarginContainer_Player4/ScoreLabel"));
            heeftGeantwoord.Add(false);
            antwoorden.Add("");
        }









        KiesVolgendeVraag();
        UpdateSpelerUI();
    }

    private void KiesVolgendeVraag()
    {   GD.Print("kiesvolgendevraag");
        aantalGeantwoord = 0;
        for(int i = 0; i < GlobalVariables.Instance.playersalive.Count; i ++){
            heeftGeantwoord[i] = false;
        }
        if (huidigeVraagIndex >= 3)
        {
            BepaalWinnaar();
            return;
        }

        huidigeVraagIndex++;
        int randomVraagIndex = (int)(GD.Randi() % vragen.Count);

        huidigeVraag = vragen[randomVraagIndex];
        vragen.RemoveAt(randomVraagIndex);

        
        aantalGeantwoord = 0;
        snelsteSpeler = -1;

        ToonVraag();
        UpdateSpelerUI();
    }

    private void ToonVraag()
    {   GD.Print("toonvraag");
        vraagLabel.Text = $"{huidigeVraag.VraagText}\n" +
                          $"(A) = {huidigeVraag.Antwoorden[0]}\n" +
                          $"(B) = {huidigeVraag.Antwoorden[1]}\n" +
                          $"(X) = {huidigeVraag.Antwoorden[2]}\n" +
                          $"(Y) = {huidigeVraag.Antwoorden[3]}";
    }

    private void UpdateSpelerUI()
    {   GD.Print("updatespelerui");
        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {   GD.Print("in for loop update player ui");
            spelerStatusLabels[i].Text = $"Status: {(heeftGeantwoord[i] ? "Gekozen" : "Nog niet gekozen")}";
            spelerScoreLabels[i].Text = $"Score: {scores[i]}";
        }
        GD.Print("uit updateplayer ui");
    }

    private void RegistreerAntwoord(int speler, string antwoord)
    {   GD.Print("in registreerantwoord");
        if (heeftGeantwoord[speler])
            return;
        GD.Print("na eerste check registreerantwoord");
        heeftGeantwoord[speler] = true;
        antwoorden[speler] = antwoord;
        aantalGeantwoord++;

        if (antwoord == huidigeVraag.CorrectAntwoord && snelsteSpeler == -1)
        {
            snelsteSpeler = speler;
        }

        
        GD.Print("1");
        if (aantalGeantwoord == GlobalVariables.Instance.playersalive.Count)
        {GD.Print("2");
            ToonCorrectAntwoord();
            GD.Print("8");
        }
        UpdateSpelerUI();
    }

    private void ToonCorrectAntwoord()
    {GD.Print("3");
        vraagLabel.Text += $"\nHet juiste antwoord was: {huidigeVraag.CorrectAntwoord}";

        if (snelsteSpeler != -1 && antwoorden[snelsteSpeler] == huidigeVraag.CorrectAntwoord)
        {   GD.Print("4");
            scores[snelsteSpeler] += GlobalVariables.Instance.playersalive.Count-1;
        }
        GD.Print("5");
        for (int i = 0; i < GlobalVariables.Instance.playersalive.Count; i++)
        {GD.Print("6");
            if (heeftGeantwoord[i] && antwoorden[i] == huidigeVraag.CorrectAntwoord && i != snelsteSpeler)
            {GD.Print("7");
                scores[i] += 1;
            }
        }

        UpdateSpelerUI();
        GetTree().CreateTimer(3).Connect("timeout", Callable.From(KiesVolgendeVraag));
    }

    private void BepaalWinnaar()
    {GD.Print("in bepaalwinaar");
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

        vraagLabel.Text = $"Winnaar(s): Speler {string.Join(", Speler ", winnaars)}";
    }
    public override void _Input(InputEvent @event)
    {       GD.Print("in inputevent");
        if (@event.IsActionPressed("A_1")) RegistreerAntwoord(0, "A");
        if (@event.IsActionPressed("B_1")) RegistreerAntwoord(0, "B");
        if (@event.IsActionPressed("X_1")) RegistreerAntwoord(0, "C");
        if (@event.IsActionPressed("Y_1")) RegistreerAntwoord(0, "D");

        if (@event.IsActionPressed("A_2")) RegistreerAntwoord(1, "A");
        if (@event.IsActionPressed("B_2")) RegistreerAntwoord(1, "B");
        if (@event.IsActionPressed("X_2")) RegistreerAntwoord(1, "C");
        if (@event.IsActionPressed("Y_2")) RegistreerAntwoord(1, "D");

        if (@event.IsActionPressed("A_3")) RegistreerAntwoord(2, "A");
        if (@event.IsActionPressed("B_3")) RegistreerAntwoord(2, "B");
        if (@event.IsActionPressed("X_3")) RegistreerAntwoord(2, "C");
        if (@event.IsActionPressed("Y_3")) RegistreerAntwoord(2, "D");

        if (@event.IsActionPressed("A_4")) RegistreerAntwoord(3, "A");
        if (@event.IsActionPressed("B_4")) RegistreerAntwoord(3, "B");
        if (@event.IsActionPressed("X_4")) RegistreerAntwoord(3, "C");
        if (@event.IsActionPressed("Y_4")) RegistreerAntwoord(3, "D");
    }
}
