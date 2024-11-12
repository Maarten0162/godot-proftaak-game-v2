using Godot;
using System;
using System.Diagnostics.SymbolStore;

public partial class UI : Control
{
	
	private Label coincount;
	private Label Health;
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
	   main.updateplayerui += UpdateUI;	  
	   
	   
	}

	void UpdateUI(Player player)
	{	
		coincount = GetNode<Label>($"CoinCount{player.Name}");
		Health = GetNode<Label>($"Health{player.Name}");
		coincount.Text = player.Currency.ToString();
			Health.Text = player.Health.ToString();
		
		 //hier komt een death UI icon;
	}
	
}
