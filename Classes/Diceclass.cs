using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

public class Dice
{

	int max;
	int min;
	Random rnd = new Random();

   public Dice(int min, int max)
   {
    this.min = min;
	this.max = max;
   }

	public int diceroll()
	{
        int eyeCount = rnd.Next(min,max);
		return eyeCount;

	}

}