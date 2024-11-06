using Godot;
using System;

public class Player{

   public Player(int currencyinput)
   {
     currencyinput = currency;
   }

   private int currency;

   public int Currency{
      get{
         return currency;
      }
   }
   public void Addcurrency(int amount)
   {
     currency += amount;
   }
}