using Godot;
using Godot.NativeInterop;
using System;
using System.Numerics;
using System.Threading;

public partial class Menu : Node2D
{
	Button start;
	Button ktwee;
	Button kdrie;
	Button kvier;


	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	GlobalVariables.Instance.playeramount = 0;	
	

		


		start = GetNode<Button>("Node2D/Button5");
		ktwee = GetNode<Button>("Node2D/Button2");
		kdrie = GetNode<Button>("Node2D/Button3");
		kvier = GetNode<Button>("Node2D/Button4");


		start.Text = "Select aantal spelers";


		ktwee.Size = new Godot.Vector2(31, 31);
		ktwee.Scale = new Godot.Vector2(1.5f, 1.5f);
		ktwee.Position = new Godot.Vector2(-113f, -15.5f);

		kdrie.Size = new Godot.Vector2(31, 31);
		kdrie.Scale = new Godot.Vector2(1.5f, 1.5f);
		kdrie.Position = new Godot.Vector2(-15.5f, -15.5f);

		kvier.Size = new Godot.Vector2(31, 31);
		kvier.Scale = new Godot.Vector2(1.5f, 1.5f);
		kvier.Position = new Godot.Vector2(82f, -15.5f);

		start.Size = new Godot.Vector2(100, 31);
		start.Scale = new Godot.Vector2(1, 1f);
		start.Position  = new Godot.Vector2(-70f, 136f);

		
	

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
		GlobalVariables.Instance.playeramount = 2;
		start.Text = "Start Spel met " + GlobalVariables.Instance.playeramount + " spelers";
		
	}
	private void drie()
	{
		GlobalVariables.Instance.playeramount = 3;
		start.Text = "Start Spel met " + GlobalVariables.Instance.playeramount + " spelers";
	}
	private void vier()
	{	
		GlobalVariables.Instance.playeramount = 4;
		start.Text = "Start Spel met " + GlobalVariables.Instance.playeramount + " spelers";
	}
}
