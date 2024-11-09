using Godot;
using System;

public partial class UI : Control
{
	
	private Label coincount;
	private Main main;
	
	  public override void _Ready()
	{
	   
	   main = GetNode<Main>("/root/Node2D");
	 
	   if(main == null)
	   {
	   	GD.Print("UI kon main node niet vinden");
	   }
	   else GD.Print("ui heeftmain node gevonden");
	   if(coincount == null)
	   {
	   	GD.Print("coincount niet gevonden");
	   }
	   else GD.Print("coincount wel gevonden");
	   main.PlayersReady += UpdateCoinCount;	
	   
	   
	}

	void UpdateCoinCount(Player player)
	{	
		GD.Print("probeer player 1 currency te printen");
				 coincount = GetNode<Label>($"CoinCount{player.Name}");
		coincount.Text = player.Currency.ToString();
	}
	
}
