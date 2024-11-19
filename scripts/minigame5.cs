using Godot;
using System;
using System.Collections.Generic;

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

    private int[] scores = { 0, 0, 0, 0 };  // Scores van de spelers
    private bool[] heeftGeantwoord = { false, false, false, false };  // Of een speler al geantwoord heeft
    private string[] antwoorden = { "", "", "", "" };  // De antwoorden van de spelers
    private int aantalGeantwoord = 0;  // Aantal spelers die geantwoord hebben
    private int snelsteSpeler = -1;  // De speler die het eerst heeft geantwoord

    private Label vraagLabel;
    private Label statusLabel;
    private Label scoreLabel;
    private Timer timer;

    public override void _Ready()
    {
        vraagLabel = GetNode<Label>("VraagLabel");
        statusLabel = GetNode<Label>("StatusLabel");
        scoreLabel = GetNode<Label>("ScoreLabel");
        timer = GetNode<Timer>("Timer");

        KiesVolgendeVraag();
    }

    private void KiesVolgendeVraag()
{
    if (huidigeVraagIndex >= 4) // 5 vragen
    {
        BepaalWinnaar();
        return;
    }

    huidigeVraagIndex++;

    // Hernoemen van de randomIndex naar randomVraagIndex om conflicten te vermijden
    int randomVraagIndex = (int)(GD.Randi() % vragen.Count);

    // Zorg ervoor dat je de vraag niet herhaalt
    huidigeVraag = vragen[randomVraagIndex];

    // Verwijder de vraag uit de lijst om duplicaten te voorkomen
    vragen.RemoveAt(randomVraagIndex);

    // Reset alles voor de nieuwe vraag
    heeftGeantwoord = new bool[4];
    antwoorden = new string[4];
    aantalGeantwoord = 0;
    snelsteSpeler = -1;

    ToonVraag();
    ToonStatus();
}


    private void ToonVraag()
    {
        vraagLabel.Text = $"{huidigeVraag.VraagText}\n" +
                          $"(A) A: {huidigeVraag.Antwoorden[0]}\n" +
                          $"(B) B: {huidigeVraag.Antwoorden[1]}\n" +
                          $"(X) C: {huidigeVraag.Antwoorden[2]}\n" +
                          $"(Y) D: {huidigeVraag.Antwoorden[3]}";
    }

    private void ToonStatus()
    {
        string status = "Status:\n";
        for (int i = 0; i < 4; i++)
        {
            status += $"Speler {i + 1}: {(heeftGeantwoord[i] ? "Heeft gekozen" : "Nog niet gekozen")}\n";
        }
        statusLabel.Text = status;
    }

    private void RegistreerAntwoord(int speler, string antwoord)
    {
        if (heeftGeantwoord[speler]) // Dubbele antwoorden blokkeren
            return;

        heeftGeantwoord[speler] = true;
        antwoorden[speler] = antwoord;
        aantalGeantwoord++;

        // Als dit de eerste speler is die het juiste antwoord geeft
        if (antwoord == huidigeVraag.CorrectAntwoord && snelsteSpeler == -1)
        {
            snelsteSpeler = speler;
        }

        ToonStatus();

        // Als alle spelers hebben geantwoord
        if (aantalGeantwoord == 4)
        {
            ToonCorrectAntwoord();
        }
    }

    private void ToonCorrectAntwoord()
    {
        vraagLabel.Text += $"\nHet juiste antwoord was: {huidigeVraag.CorrectAntwoord}";

        // De snelste speler krijgt 3 punten, als die het juiste antwoord gaf
        if (snelsteSpeler != -1 && antwoorden[snelsteSpeler] == huidigeVraag.CorrectAntwoord)
        {
            scores[snelsteSpeler] += 3; // Snelste speler krijgt 3 punten
        }

        // Andere spelers die het juiste antwoord hebben gegeven krijgen 1 punt
        for (int i = 0; i < 4; i++)
        {
            if (heeftGeantwoord[i] && antwoorden[i] == huidigeVraag.CorrectAntwoord && i != snelsteSpeler)
            {
                scores[i] += 1; // Spelers die het juiste antwoord gaven, maar niet het snelste waren, krijgen 1 punt
            }
        }

        // Update de score na het beantwoorden van de vraag
        UpdateScores();

        // Wacht 3 seconden voordat de volgende vraag wordt geladen
        GetTree().CreateTimer(3).Connect("timeout", Callable.From(KiesVolgendeVraag));
    }

    private void UpdateScores()
    {
        scoreLabel.Text = $"Scores:\n" +
                          $"Speler 1: {scores[0]}\n" +
                          $"Speler 2: {scores[1]}\n" +
                          $"Speler 3: {scores[2]}\n" +
                          $"Speler 4: {scores[3]}";
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

        vraagLabel.Text = $"Winnaar(s): Speler {string.Join(", Speler ", winnaars)}";
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("A_1")) RegistreerAntwoord(0, "A");
        if (@event.IsActionPressed("B_1")) RegistreerAntwoord(0, "B");
        if (@event.IsActionPressed("speler1_c")) RegistreerAntwoord(0, "C");
        if (@event.IsActionPressed("speler1_d")) RegistreerAntwoord(0, "D");

        if (@event.IsActionPressed("A_2")) RegistreerAntwoord(1, "A");
        if (@event.IsActionPressed("B_2")) RegistreerAntwoord(1, "B");
        if (@event.IsActionPressed("speler2_c")) RegistreerAntwoord(1, "C");
        if (@event.IsActionPressed("speler2_d")) RegistreerAntwoord(1, "D");

        if (@event.IsActionPressed("A_3")) RegistreerAntwoord(2, "A");
        if (@event.IsActionPressed("B_3")) RegistreerAntwoord(2, "B");
        if (@event.IsActionPressed("speler3_c")) RegistreerAntwoord(2, "C");
        if (@event.IsActionPressed("speler3_d")) RegistreerAntwoord(2, "D");

        if (@event.IsActionPressed("A_4")) RegistreerAntwoord(3, "A");
        if (@event.IsActionPressed("B_4")) RegistreerAntwoord(3, "B");
        if (@event.IsActionPressed("speler4_c")) RegistreerAntwoord(3, "C");
        if (@event.IsActionPressed("speler4_d")) RegistreerAntwoord(3, "D");
    }
}