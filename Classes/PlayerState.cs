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
    public string[] Items { get; set; }
    public int RollAdjustment { get; set; }

    // Constructor that accepts player state data
    public PlayerState(Vector2 position, int positionspace, int health, int currency, bool skipturn, bool hascap, bool hasknuckles, bool hasgoldenknuckles, string[] items, int rolladjustment)
    {
        Position = position;
        PositionSpace = positionspace;
        Health = health;
        Currency = currency;
        SkipTurn = skipturn;
        HasCap = hascap;
        HasKnuckles = hasknuckles;
        HasGoldenKnuckles = hasgoldenknuckles;
        Items = items;
        RollAdjustment = rolladjustment;
    }
}
