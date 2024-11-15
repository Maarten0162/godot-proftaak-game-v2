using Godot;
using Godot.NativeInterop;
using System;
using System.Numerics;
using System.Threading;

public partial class Menu : Node2D
{
	Button start = new Button();
	Button ktwee = new Button();
	Button kdrie = new Button();
	Button kvier = new Button();
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	GlobalVariables.Instance.playeramount = 0;
		AddChild(start);
		AddChild(ktwee);
		AddChild(kdrie);
		AddChild(kvier);	

		start.Position = new Godot.Vector2(-75, 2);
		ktwee.Position = new Godot.Vector2(-80, -70);
		kdrie.Position = new Godot.Vector2(-35, -70);
		kvier.Position = new Godot.Vector2(10, -70);


		start.Text = "Start Game!";		
		ktwee.Text = "2";
		kdrie.Text = "3";
		kvier.Text = "4";

		start.ZIndex = 3;
		ktwee.ZIndex = 3;
		kdrie.ZIndex = 3;
		kvier.ZIndex = 3;

		start.Pressed += ()=> StartGame(GlobalVariables.Instance.playeramount);
		ktwee.Pressed += ()=> twee();
		kdrie.Pressed += ()=> drie();
		kvier.Pressed += ()=> vier();

 	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("D-Pad-left_1"))
		{
			twee();
		}

		if(Input.IsActionJustPressed("D-Pad-up_1"))
		{
			drie();
		}

		if(Input.IsActionJustPressed("D-Pad-right_1"))
		{
			vier();
		}

		if(Input.IsActionJustPressed("D-Pad-down_1"))
		{
			StartGame(GlobalVariables.Instance.playeramount);
		}

	}
	private void StartGame(int Speler)
	{
		if(GlobalVariables.Instance.playeramount == 0)
		{
			GD.Print("Selecteer het aantal spelers");
		}
		else
		{	
			GlobalVariables.Instance.SwitchToMainBoard();
		}
		
	}
	private void twee()
	{
		ktwee.Position = new Godot.Vector2(-80000, -70000);
		kdrie.Position = new Godot.Vector2(-350000, -70000);
		kvier.Position = new Godot.Vector2(100000, -70000);
		GlobalVariables.Instance.playeramount = 2;
		
	}
	private void drie()
	{
		ktwee.Position = new Godot.Vector2(-80000, -70000);
		kdrie.Position = new Godot.Vector2(-350000, -70000);
		kvier.Position = new Godot.Vector2(100000, -70000);
		GlobalVariables.Instance.playeramount = 3;
	}
	private void vier()
	{
		ktwee.Position = new Godot.Vector2(-80000, -70000);
		kdrie.Position = new Godot.Vector2(-350000, -70000);
		kvier.Position = new Godot.Vector2(100000, -70000);
		GlobalVariables.Instance.playeramount = 4;
	}
}
