using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

public class Dice
{

	int max;
	int min;
	Random rnd = new Random();

	private AudioStreamPlayer Dobbelgeluid;
   public Dice(int min, int max, AudioStreamPlayer Dobbelgeluid)
   {
	this.min = min;
	this.max = max;
	this.Dobbelgeluid = Dobbelgeluid;
   }

	public int diceroll()
	{
		int eyeCount = rnd.Next(min,max);
		
	if (Dobbelgeluid != null)
		{
			Dobbelgeluid.Play();
		}
		else
		{
			GD.Print("Nog geen goed geluid toegevoegd.");
		}
		
		return eyeCount;

	}

}
