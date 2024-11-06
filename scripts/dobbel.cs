using Godot;
using System;

public partial class Dobbel : Sprite2D
{
private Random _random = new Random();
private string[] _diceFaces = {
"res://assets/dice/default-dice/Dobbel1.png",
"res://assets/dice/default-dice/Dobbel2.png",
"res://assets/dice/default-dice/Dobbel3.png",
"res://assets/dice/default-dice/Dobbel4.png",
"res://assets/dice/default-dice/Dobbel5.png",
"res://assets/dice/default-dice/Dobbel6.png",
};

public override void _Ready() 
{ 
GD.Print("Dice ready!"); 
Roll(); 
} 
public int Roll() 
{ 
int randomIndex = _random.Next(0, _diceFaces.Length); 
Texture = (Texture2D)GD.Load(_diceFaces[randomIndex]); 
GD.Print("Rolled: " + (randomIndex + 1));
return randomIndex;
} 
}
