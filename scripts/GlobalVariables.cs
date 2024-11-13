using Godot;

public partial class GlobalVariables : Node
{  
    public static GlobalVariables Instance { get; private set; }

    public Player[] Playerlist { get; set; }
    public int playeramount { get; set; }
    public string Winner { get; set; }
    


    public override void _Ready()
    {      
        Instance = this;
    }
}
