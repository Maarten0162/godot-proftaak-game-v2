using Godot;
using System.Collections.Generic;
using System;


public partial class GlobalVariables : Node
{  
    public static GlobalVariables Instance { get; private set; }

    public Player[] Playerlist { get; set; }
    public int playeramount { get; set; }
    public string Winner { get; set; }
        // Dictionary to store the state of each player
    public Dictionary<int, PlayerState> BoardState = new Dictionary<int, PlayerState>();

    // Method to save the state of a player
    public void SavePlayerState(int playerNumber, Vector2 position, int positionspace, int health, int currency, bool skipturn, bool hascap, bool hasknuckles, bool hasgoldenknuckles, string[] items, int rolladjustment)
    {
        // Create a new PlayerState and store it in the dictionary for the given player
        BoardState[playerNumber] = new PlayerState(position, positionspace, health, currency, skipturn, hascap, hasknuckles, hasgoldenknuckles, items, rolladjustment);
    }

    // Method to retrieve a player's state, or create a default if it doesn't exist
    public PlayerState GetPlayerState(int playerNumber)
    {
        if (BoardState.TryGetValue(playerNumber, out PlayerState playerState))
        {
            return playerState;
        }
        
        // Return a default PlayerState if the player's state hasn't been saved
        return new PlayerState(Vector2.Zero, 42, 100, 100, false, false, false, false, new string[3], 0);  // Adjust default values as needed
    }



    // Path to main board and minigame scenes
    private const string MainBoardScenePath = "res://scenes/main.tscn";
    private const string MinigameScenePath = "res://scenes/minigame1.tscn";
    private const string MenuScenePath = "res://scenes/Menu.tscn";

    private PackedScene mainBoardScene;
    private PackedScene minigameScene;
    private PackedScene menuScene;

    // Instance of the currently active scene
    private Node currentScene;
    private Node StartGame;
    public int TurnCount { get; set; }

    
    public override void _Ready()
    {      
        Instance = this;
            mainBoardScene = (PackedScene)ResourceLoader.Load(MainBoardScenePath);
            minigameScene = (PackedScene)ResourceLoader.Load(MinigameScenePath);
            menuScene = (PackedScene)ResourceLoader.Load(MenuScenePath);
            

    }
      public void SwitchToMenu()
    {
        // If we are coming back from a minigame, restore the board state here
        ChangeScene(menuScene);
        RestoreBoardState();
    }
    public void SwitchToMainBoard()
    {
        // If we are coming back from a minigame, restore the board state here
        ChangeScene(mainBoardScene);
        RestoreBoardState();
    }
    public void SwitchToMinigame()
    {
        // Save the board state before switching
        SaveBoardState();

        // Load the minigame scene
        ChangeScene(minigameScene);
    }
    private void ChangeScene(PackedScene newScene)
    {       
        // Remove the current scene if there is one
        if (currentScene != null)
        {
            currentScene.QueueFree();
        }

        // Instance and add the new scene
        currentScene = newScene.Instantiate();
        AddChild(currentScene);
    }

    private void SaveBoardState()
    {
        // Save any necessary data about the board's state
        // For example, you can get player positions, scores, etc. and store them in BoardState
        // Example:
        
    }

    private void RestoreBoardState()
    {
        // Restore the saved state to the board when switching back
        // Example:
        // if (BoardState.ContainsKey("PlayerPosition"))
        // {
        //     boardScene.GetNode<Player>("Player").Position = (Vector2)BoardState["PlayerPosition"];
        // }
    }


}
