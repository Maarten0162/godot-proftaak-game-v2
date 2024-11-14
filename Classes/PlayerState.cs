using Godot;
using System.Collections.Generic;

public class PlayerState
{
    public Vector2 Position { get; set; }
    public int PositionSpace { get; set; }
    public int Health { get; set; }
    public int Currency { get; set; }
    public bool SkipTurn { get; set; }
    public bool HasCap { get; set; }
    public bool HasKnuckles { get; set; }
    public bool HasGoldenKnuckles { get; set; }
    public string[] Inventory { get; set; }
    public int RollAdjustment { get; set; }


    // Constructor that accepts player state data
  public PlayerState(Player player)
    {
        Position = player.Position;
        PositionSpace = player.PositionSpace;
        Health = player.Health;
        Currency = player.Currency;
        SkipTurn = player.SkipTurn;
        HasCap = player.HasCap;
        HasKnuckles = player.HasKnuckles;
        HasGoldenKnuckles = player.HasGoldenKnuckles;
        Inventory = player.Inventory;
        RollAdjustment = player.RollAdjustment;

     
    }

}
