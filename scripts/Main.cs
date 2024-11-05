using Godot;
using System;

public partial class Main : Node2D
{
	int loc_pl1 = 0;
	int loc_pl2 = 0;
	int loc_pl3 = 0;
	int loc_pl4 = 0;

	int spacesAmount = 41;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void move(){
		if (Input.IsActionJustPressed("input_move")) {
			
		}
	}

}
