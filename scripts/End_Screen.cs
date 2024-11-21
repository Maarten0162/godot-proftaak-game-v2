using Godot;
using System;

public partial class End_Screen : Node
{	Label winnerlabel;
	string winner;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	int mosthealth = 0;
		for(int i = 0; i <  GlobalVariables.Instance.playersalive.Count; i++)
		{
			if(GlobalVariables.Instance.playersalive[i].Health > mosthealth){
				GlobalVariables.Instance.playersalive[i].Health = mosthealth;
			}

		}
		for(int i = 0; i <  GlobalVariables.Instance.playersalive.Count; i++)
		{
			if(GlobalVariables.Instance.playersalive[i].Health == mosthealth){
			winner = GlobalVariables.Instance.playersalive[i].Name;
		}

		winnerlabel = GetNode<Label>("WinnerLabel");
		winnerlabel.Text = "De winnaar is: " + winner + "!!!";
	}}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
