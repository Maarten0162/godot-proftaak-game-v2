using Godot;
using System;
using System.Linq;
public partial class Minigame5 : Node
{
	int minigameplayeramount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player1")){

	}
	if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3")){

	}
	if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player3")){

	}
	if (GlobalVariables.Instance.playersalive.Any(player => player.Name == "player4")){

	}


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
