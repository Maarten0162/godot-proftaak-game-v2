using Godot;
using System;

public class BoardState
{
   
    public string[] Names { get; set; }
    public string[] OriginalNames { get; set; }

    // Constructor to initialize the board state with Node2D arrays and strings
    public BoardState( string[] names, string[] originalNames)
    {
        
        Names = names;
        OriginalNames = originalNames;
    }
}