using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    private Node2D player1;
    private Node2D player2;
    private Node2D player3;
    private Node2D player4;

    private List<Node2D> spaces = new List<Node2D>(); 
    private int player1CurrentSpace = 0; 

    int loc_pl1 = 0;
    int loc_pl2 = 0;
    int loc_pl3 = 0;
    int loc_pl4 = 0;

	int spacesAmount = 19;
    

	private bool isRolling = false;

    public override void _Ready()
    {
		
        // Initialize players
        player1 = GetNode<Node2D>("players/player1");
        player2 = GetNode<Node2D>("players/player2");
        player3 = GetNode<Node2D>("players/player3");
        player4 = GetNode<Node2D>("players/player4");


		

        for (int i = 1; i <= spacesAmount; i++)
        {
            spaces.Add(GetNode<Node2D>($"spaces/Marker2D{i}"));
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("test1") && !isRolling)
        {
			isRolling = true;
            int diceRoll = dobbelDefault();

            player1CurrentSpace = player1CurrentSpace + diceRoll;

            if (player1CurrentSpace < 0)
            {
                player1CurrentSpace = player1CurrentSpace = 0;
            }
			

            // Move player to the next space
            Node2D nextSpace = spaces[player1CurrentSpace];
            player1.Position = nextSpace.Position;
        }

		if (player1CurrentSpace > spaces.Count) {
			player1CurrentSpace = player1CurrentSpace - spaces.Count;
		}

		if (Input.IsActionJustReleased("test1"))
        {
            isRolling = false;
        }
		
    }

	

    public int dobbelDefault()
    {
        Random rnd = new Random();
        int antOgen = rnd.Next(1, 4); // Rolls a dice between 1 and 6 (standard dice)
        GD.Print("Dice result: " + antOgen);
        return antOgen;
    }

    public int dobbelUpgrade1()
    {
        Random rnd = new Random();
        int antOgen = rnd.Next(1, 7);
        GD.Print("Dice result: " + antOgen);
        return antOgen;
    }

    public int dobbelUpgrade2()
    {
        Random rnd = new Random();
        int antOgen = rnd.Next(1, 7);
        GD.Print("Dice result: " + antOgen);
        return antOgen;
    }

    public int dobbelUpgrade3()
    {
        Random rnd = new Random();
        int antOgen = rnd.Next(-3, 10);
        GD.Print("Dice result: " + antOgen);
        return antOgen;
    }
}
