using Godot;
using System;

public partial class StartGame : Node
{


    public override void _Ready(){
        GlobalVariables.Instance.SwitchtoUitleg();
    }
}
