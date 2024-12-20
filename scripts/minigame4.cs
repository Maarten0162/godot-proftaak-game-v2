using Godot;
using System;

public partial class minigame4 : Control
{
    private string[] spelers = { "Speler 1", "Speler 2", "Speler 3", "Speler 4" };
    private string[] keuzes = new string[4];
    private bool[] keuzeGemaakt = new bool[4];
    private int[] winnaars = new int[2];
    private Label keuzeLabel;
    private Label resultaatLabel;
    private Label rondeLabel;
    private Label winconditiesLabel; // Nieuw: Wincondities Label
    private int huidigeHalveFinale = 0;
    private bool inFinale = false;

    public override void _Ready()
    {
        keuzeLabel = GetNode<Label>("KeuzeLabel");
        resultaatLabel = GetNode<Label>("ResultaatLabel");
        rondeLabel = GetNode<Label>("RondeLabel");
        winconditiesLabel = GetNode<Label>("LabelWincondities"); // Nieuw: Koppel het Label aan de scÃ¨ne

        // Stel de tekst voor de wincondities in
        winconditiesLabel.Text = "Wincondities:\n- Pistool wint van Hoed\n- Hoed wint van Mes\n- Mes wint van Pistool";

        ToonHalveFinale(0);
    }

    public override void _Process(double delta)
    {
        if (keuzeGemaakt[0] && keuzeGemaakt[1] && huidigeHalveFinale == 0)
        {
            VerwerkHalveFinale(0, 1, 0);
        }
        else if (keuzeGemaakt[2] && keuzeGemaakt[3] && huidigeHalveFinale == 1)
        {
            VerwerkHalveFinale(2, 3, 1);
        }
        else if (keuzeGemaakt[winnaars[0]] && keuzeGemaakt[winnaars[1]] && inFinale)
        {
            VerwerkFinale();
        }
    }

    private void ToonHalveFinale(int halveFinaleIndex)
    {
        huidigeHalveFinale = halveFinaleIndex;

        if (halveFinaleIndex == 0)
        {
            rondeLabel.Text = $"Halve Finale 1: {spelers[0]} vs {spelers[1]}";
            keuzeLabel.Text = $"Speler 1 (Q: Mes, W: Pistool, E: Hoed)\nSpeler 2 (A: Mes, S: Pistool, D: Hoed)";
        }
        else
        {
            rondeLabel.Text = $"Halve Finale 2: {spelers[2]} vs {spelers[3]}";
            keuzeLabel.Text = $"Speler 3 (Z: Mes, X: Pistool, C: Hoed)\nSpeler 4 (I: Mes, O: Pistool, P: Hoed)";
        }

        ResetKeuzes();
    }

    private void VerwerkHalveFinale(int speler1Index, int speler2Index, int winnaarIndex)
    {
        string resultaat = $"{spelers[speler1Index]} koos {keuzes[speler1Index]}, {spelers[speler2Index]} koos {keuzes[speler2Index]}.\n";

        if (keuzes[speler1Index] == keuzes[speler2Index])
        {
            // Gelijkspel: Start ronde opnieuw
            resultaat += "Gelijkspel! Ronde wordt opnieuw gestart.";
            resultaatLabel.Text = resultaat;
            ToonHalveFinale(huidigeHalveFinale);
            return;
        }

        string winnaar = BepaalWinnaar(keuzes[speler1Index], keuzes[speler2Index]);
        if (winnaar == "Speler 1")
        {
            winnaars[winnaarIndex] = speler1Index;
            resultaat += $"{spelers[speler1Index]} wint!";
        }
        else
        {
            winnaars[winnaarIndex] = speler2Index;
            resultaat += $"{spelers[speler2Index]} wint!";
        }

        resultaatLabel.Text = resultaat;

        if (huidigeHalveFinale == 0)
        {
            ToonHalveFinale(1);
        }
        else
        {
            ToonFinale();
        }
    }

    
    private void ToonFinale()
    {
    inFinale = true;
    ResetKeuzes();

    int speler1Index = winnaars[0];
    int speler2Index = winnaars[1];

    // Toon de finale ronde
    rondeLabel.Text = $"Finale: {spelers[speler1Index]} vs {spelers[speler2Index]}";

    // Toon de juiste keuzes voor de spelers in de finale
    if (speler1Index == 0)
    {
        keuzeLabel.Text = $"{spelers[speler1Index]} (Q: Mes, W: Pistool, E: Hoed)\n";
    }
    else if (speler1Index == 1)
    {
        keuzeLabel.Text = $"{spelers[speler1Index]} (Q: Mes, W: Pistool, E: Hoed)\n";
    }
    else if (speler1Index == 2)
    {
        keuzeLabel.Text = $"{spelers[speler1Index]} (Z: Mes, X: Pistool, C: Hoed)\n";
    }
    else if (speler1Index == 3)
    {
        keuzeLabel.Text = $"{spelers[speler1Index]} (I: Mes, O: Pistool, P: Hoed)\n";
    }

    // Voeg de keuzes van de tweede speler toe
    if (speler2Index == 0)
    {
        keuzeLabel.Text += $"{spelers[speler2Index]} (Q: Mes, W: Pistool, E: Hoed)";
    }
    else if (speler2Index == 1)
    {
        keuzeLabel.Text += $"{spelers[speler2Index]} (Q: Mes, W: Pistool, E: Hoed)";
    }
    else if (speler2Index == 2)
    {
        keuzeLabel.Text += $"{spelers[speler2Index]} (Z: Mes, X: Pistool, C: Hoed)";
    }
    else if (speler2Index == 3)
    {
        keuzeLabel.Text += $"{spelers[speler2Index]} (I: Mes, O: Pistool, P: Hoed)";
    }
    }

        

    private void VerwerkFinale()
    {
        int speler1Index = winnaars[0];
        int speler2Index = winnaars[1];
        string resultaat = $"{spelers[speler1Index]} koos {keuzes[speler1Index]}, {spelers[speler2Index]} koos {keuzes[speler2Index]}.\n";

        if (keuzes[speler1Index] == keuzes[speler2Index])
        {
            resultaat += "Gelijkspel! Finale wordt opnieuw gestart.";
            resultaatLabel.Text = resultaat;
            ToonFinale();
            return;
        }

        string winnaar = BepaalWinnaar(keuzes[speler1Index], keuzes[speler2Index]);
        if (winnaar == "Speler 1")
        {
            resultaat += $"{spelers[speler1Index]} wint de finale!";
        }
        else
        {
            resultaat += $"{spelers[speler2Index]} wint de finale!";
        }

        resultaatLabel.Text = resultaat;
        keuzeLabel.Text = "Spel afgelopen!";
    }

    private string BepaalWinnaar(string keuze1, string keuze2)
    {
        if (keuze1 == "Pistool" && keuze2 == "Hoed" || keuze1 == "Hoed" && keuze2 == "Mes" || keuze1 == "Mes" && keuze2 == "Pistool")
        {
            return "Speler 1";
        }
        else
        {
            return "Speler 2";
        }
    }

    private void ResetKeuzes()
    {
        for (int i = 0; i < keuzes.Length; i++)
        {
            keuzes[i] = "";
            keuzeGemaakt[i] = false;
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Speler 1
        if (Input.IsActionJustPressed("D-Pad-left_1")) { keuzes[0] = "Mes"; keuzeGemaakt[0] = true; }
        if (Input.IsActionJustPressed("D-Pad-up_1")) { keuzes[0] = "Pistool"; keuzeGemaakt[0] = true; }
        if (Input.IsActionJustPressed("D-Pad-right_1")) { keuzes[0] = "Hoed"; keuzeGemaakt[0] = true; }

        // Speler 2
        if (Input.IsActionJustPressed("D-Pad-left_2")) { keuzes[1] = "Mes"; keuzeGemaakt[1] = true; }
        if (Input.IsActionJustPressed("D-Pad-up_2")) { keuzes[1] = "Pistool"; keuzeGemaakt[1] = true; }
        if (Input.IsActionJustPressed("D-Pad-right_2")) { keuzes[1] = "Hoed"; keuzeGemaakt[1] = true; }

        // Speler 3
        if (Input.IsActionJustPressed("D-Pad-left_3")) { keuzes[2] = "Mes"; keuzeGemaakt[2] = true; }
        if (Input.IsActionJustPressed("D-Pad-up_3")) { keuzes[2] = "Pistool"; keuzeGemaakt[2] = true; }
        if (Input.IsActionJustPressed("D-Pad-right_3")) { keuzes[2] = "Hoed"; keuzeGemaakt[2] = true; }

        // Speler 4
        if (Input.IsActionJustPressed("D-Pad-left_4")) { keuzes[3] = "Mes"; keuzeGemaakt[3] = true; }
        if (Input.IsActionJustPressed("D-Pad-up_4")) { keuzes[3] = "Pistool"; keuzeGemaakt[3] = true; }
        if (Input.IsActionJustPressed("D-Pad-right_4")) { keuzes[3] = "Hoed"; keuzeGemaakt[3] = true; }
    }
}
