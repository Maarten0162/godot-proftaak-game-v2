using Godot;
using System;
//hello
public class Player{

   public Player()
   {
	  currency = 10;
	  health = 100;
   }

   private int currency;

   public int Currency{
	  get{
		 return currency;
	  }
	  set{
		 currency = value;
	  }
   }
   private int health;

   public int Health{
	  get{
		 return health;
	  }
	  set{
		 health = value;
	  }
   }
}
 
