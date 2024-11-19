using Godot;
using System;
using System.Threading.Tasks;

public partial class Uitleg : Node
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{	await WaitForSeconds(10);
		GlobalVariables.Instance.SwitchToMenu();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
		private async Task WaitForSeconds(float seconds)
	{
		await ToSignal(GetTree().CreateTimer(seconds), "timeout");
	}
}
