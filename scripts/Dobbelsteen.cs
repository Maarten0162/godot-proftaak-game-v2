using Godot;
using System;

public partial class Dobbelsteen : Node
{

		// public AnimatedSprite2D dobbelSprite;
	
		int get1;
		int get2;
		Random rnd = new Random();

	

		public Dobbelsteen(int get1, int get2){
			this.get1 = get1;
			this.get2 = get2;

			
		}

		// public int Rol(){
		// 	int antOgen = rnd.Next(get1, get2);
		// 	GD.Print("Dice result: " + antOgen);

		// 	for(int i = -3; i <= 9; i ++){
		// 		if(i == antOgen){
		// 			//dobbelSprite.Play($"{antOgen}");
		// 		}
		// 	}
		// 	return antOgen;
		// }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// dobbelSprite = GetNode<AnimatedSprite2D>("dobbelSprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
