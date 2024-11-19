using Godot;
using System.Collections.Generic;
using System;


public partial class GlobalVariables : Node
{  
    public static GlobalVariables Instance { get; private set; }

    public Player[] Playerlist { get; set; }
    public int playeramount { get; set; }
    public int Winner { get; set; }
        // Dictionary to store the state of each player
    public Dictionary<int, PlayerState> PlayerStates = new Dictionary<int, PlayerState>();
   BoardState boardState;

    // Method to save the state of a player
    public void SavePlayerState(int playerNumber, Player player)
    {
        // Create a new PlayerState and store it in the dictionary for the given player
        PlayerStates[playerNumber] = new PlayerState(player);
    }
    public void SaveBoardState(Node2D[] spaces, string[] name, string[] originalname){
        boardState = new BoardState(name, originalname);
    }

    // Method to retrieve a player's state, or create a default if it doesn't exist
    public PlayerState GetPlayerState(int playerNumber)
    {
        if (PlayerStates.TryGetValue(playerNumber, out PlayerState playerState))
        {
            return playerState;
        }
        
        // Return a default PlayerState if the player's state hasn't been saved
        return playerState;  // Adjust default values as needed
    }
    public BoardState GetBoardState(){
        GD.Print("in getboardstate");
        return boardState;
    }



        // Path to main board and minigame scenes
        private const string MainBoardScenePath = "res://scenes/main.tscn";
        private const string Minigame1ScenePath = "res://scenes/minigame1.tscn";
         private const string Minigame2ScenePath = "res://scenes/minigame2.tscn";
          private const string Minigame3ScenePath = "res://scenes/minigame3.tscn";
           private const string Minigame4ScenePath = "res://scenes/minigame4.tscn";
            private const string Minigame5ScenePath = "res://scenes/minigame5.tscn";
        private const string MenuScenePath = "res://scenes/Menu.tscn";
        private const string EndscreenPath = "res://scenes/End_Screen.tscn";

        private PackedScene mainBoardScene;
        private PackedScene minigame1Scene;
        private PackedScene minigame2Scene;
        private PackedScene minigame3Scene;
        private PackedScene minigame4Scene;
        private PackedScene minigame5Scene;
        private PackedScene menuScene;
        private PackedScene endscreen;
        public Player player1   { get; set; }
        public Player player2   { get; set; }
        public Player player3   { get; set; }
        public Player player4   { get; set; }
        // Instance of the currently active scene
        private Node currentScene;
        private Node StartGame;
        public int TurnCount { get; set; }
        public List<Player> playersalive { get; set; }

        
        public override void _Ready()
        {      
            Instance = this;
                mainBoardScene = (PackedScene)ResourceLoader.Load(MainBoardScenePath);
                minigame1Scene = (PackedScene)ResourceLoader.Load(Minigame1ScenePath);
                minigame2Scene = (PackedScene)ResourceLoader.Load(Minigame2ScenePath);
                minigame3Scene = (PackedScene)ResourceLoader.Load(Minigame3ScenePath);
                minigame4Scene = (PackedScene)ResourceLoader.Load(Minigame4ScenePath);
                minigame5Scene = (PackedScene)ResourceLoader.Load(Minigame5ScenePath);
                
                menuScene = (PackedScene)ResourceLoader.Load(MenuScenePath);
                endscreen = (PackedScene)ResourceLoader.Load(EndscreenPath);
                

        }
        public void SwitchToMenu()
        {
            // If we are coming back from a minigame, restore the board state here
            ChangeScene(menuScene);
            RestorePlayerStates();
        }
        public void SwitchToMainBoard()
        {
            // If we are coming back from a minigame, restore the board state here
            ChangeScene(mainBoardScene);
            RestorePlayerStates();
        }

        public void SwitchToMinigame1()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(minigame1Scene);
        }
         public void SwitchToMinigame2()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(minigame2Scene);
        }
         public void SwitchToMinigame3()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(minigame3Scene);
        }
         public void SwitchToMinigame4()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(minigame4Scene);
        }
         public void SwitchToMinigame5()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(minigame5Scene);
        }
                public void SwitchToendscreen()
        {
            // Save the board state before switching
            SavePlayerStates();

            // Load the minigame scene
            ChangeScene(endscreen);
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

    private void SavePlayerStates()
    {
        // Save any necessary data about the board's state
        // For example, you can get player positions, scores, etc. and store them in PlayerStates
        // Example:
        
    }

    private void RestorePlayerStates()
    {
        // Restore the saved state to the board when switching back
        // Example:
        // if (PlayerStates.ContainsKey("PlayerPosition"))
        // {
        //     boardScene.GetNode<Player>("Player").Position = (Vector2)PlayerStates["PlayerPosition"];
        // }
    }


}
