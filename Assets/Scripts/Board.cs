
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Class to handle the board and basically the entire game's C# logic at a high level
public class Board 
{
    // Data members
    private List<Space> m_spaces = new List<Space>();
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board
    private List<Player> m_players = new List<Player>();
    private int m_turnNum;

    // Potential actions a player might have to make
    public enum Actions
    {
        EndTurn = 0,
        DetermineOrder = 1,
        RollDice = 2,
        LandedOn_UnownedProperty = 3,
        LandedOn_OwnedColorProperty = 4,
        LandedOn_OwnedUtility = 5,
        LandedOn_OwnedRailroad = 6,
        LandedOn_ChanceOrCommunityChest = 7,
        LandedOn_VisitingJail = 8,
        LandedOn_FreeParking = 9,
        LandedOn_GoToJail = 10,
        LandedOn_Tax = 11,
        LandedOn_Go = 12,
        ERROR = 999
    }

    // Constructor
    public Board()
    { 

    }

    // Creates the spaces 
    public void InitializeBoard()
    {
        // Initialize the players and spaces
        InitializeSpaces();
        InitializePlayers();

        // Set the turn num to 0
        m_turnNum = 0;
    }

    // Obtains spaces from text file
    public void InitializeSpaces()
    {
        // Obtain space data from file
        string spacesFilePath = Path.Combine(Application.streamingAssetsPath, "spaceData.txt");
        string[] lines = File.ReadAllLines(spacesFilePath);

        // Obtain space details from the data
        List<string> names = new List<string>();
        List<Actions> actions = new List<Actions>();
        foreach (string line in lines)
        {
            // Split the string
            string[] vals = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Obtain the name from first index
            names.Add(vals[0]);

            // Translate second index from string to action
            actions.Add(CastActionString(vals[1]));
        }

        // Add data to the list of spaces
        for (int i = 0; i < SPACE_NUM; i++)
        {
            Space currentSpace = new Space(names[i], i, actions[i]);
            m_spaces.Add(currentSpace);
        }
    }

    // Obtains the players from PlayerFile class
    public void InitializePlayers()
    {
        // Create player file object
        PlayerFile playerFile = new PlayerFile();

        // Create temp dicts to store players
        Dictionary<string, string> tempPlayers;

        // Obtain players from the file
        tempPlayers = playerFile.ReadPlayersFromFile();

        // Add them to the players list
        int playerNum = 0;
        foreach (string name in tempPlayers.Keys)
        {
            Player currentPlayer = new Player(name, tempPlayers[name], playerNum);
            m_players.Add(currentPlayer);
            playerNum++;
        }
    }

    // Returns space object at a particular index
    public Space GetSpace(int index)
    {
        if (index < 0 && index >= SPACE_NUM)
        {
            throw new ArgumentException("Space index out of range");
        }
        return m_spaces[index];
    }

    // Returns the current player
    public Player CurrentPlayer
    {
        get { return m_players[m_turnNum]; }
    }

    // Returns the amount of players in the game
    public int PlayerCount 
    { 
        get { return m_players.Count; } 
    }
    
    // Throws an exception if index out of bounds
    public void CheckPlayerNum(int playerNum)
    {
        if ((playerNum < 0) || playerNum > PlayerCount)
        {
            throw new Exception("Player num out of bounds!");
        }
    }

    // Returns player's icon name
    public string GetPlayerIconName(int playerNum)
    {
        // Check player num 
        CheckPlayerNum(playerNum);

        // Return the icon name
        return m_players[playerNum].Icon;
    }

    // Returns player's name
    public string GetPlayerName(int playerNum)
    {
        // Check player num 
        CheckPlayerNum(playerNum);

        // Return the icon name
        return m_players[playerNum].Name;
    }

    // Returns player's description
    public string GetPlayerDescription(int playerNum)
    {
        // Check player num 
        CheckPlayerNum(playerNum);

        // Return the icon name
        return m_players[playerNum].Description;
    }

    // Determines what action the current player must make depending on the state of their turn
    public Actions DetermineAction()
    {
        if (!CurrentPlayer.TurnInitialized)
        {
            return Actions.DetermineOrder;
        }
        if (!CurrentPlayer.RolledDice)
        {
            return Actions.RollDice;
        }
        if (!CurrentPlayer.SpaceActionCompleted)
        {
            return m_spaces[CurrentPlayer.CurrentSpace].ActionType;
        }
        else
        {
            return Actions.EndTurn;
        }
    }

    // Current player purchases the current space they're on
    public void PropertyPurchase()
    {
        CurrentPlayer.Properties.Add(m_spaces[CurrentPlayer.CurrentSpace]);
    }

    // Returns an appropriate title for the current landed on action
    public string GetLandedOnTitle()
    {
        return "Would you like to buy " + m_spaces[CurrentPlayer.CurrentSpace].Name + " for $COST_TBD?";
    }

    // Updates whose turn it is
    public void UpdateTurn()
    {
        // Reset the current player's attributes
        ResetCurrentPlayer();

        // Update whose turn is is
        if (m_turnNum < m_players.Count - 1)
        {
            m_turnNum++;
        }
        else
        {
            m_turnNum = 0;
        }
    }

    // Resets the attributes of a player at the end of their turn
    public void ResetCurrentPlayer()
    {
        CurrentPlayer.RolledDice = false;
        CurrentPlayer.RolledDoubles = false;
        CurrentPlayer.DiceRollResult = 0;
        CurrentPlayer.TurnCompleted = false;
        CurrentPlayer.SpaceActionCompleted = false;
    }

    // A player's order was determined 
    public void OrderDetermined(int diceResult)
    {
        // Update the players attributes
        CurrentPlayer.OrderDeterminingDiceResult = diceResult;
        CurrentPlayer.TurnInitialized = true;
    }

    // Returns whether or not all the players were initialized
    public bool AllPlayersInitialized()
    {
        // Check each player to see if they were not initialized
        foreach (Player player in m_players) 
        {
            if (!player.TurnInitialized)
            {
                return false;
            }
        }

        // All players initialized
        return true;
    }

    // Reorders the players based on what space they're on, resets thier current space
    public void InitializePlayerOrder()
    {
        // Sort the player
        m_players.Sort(SortPlayers);

        // Initialize the new values
        int playerNum = 0;
        foreach (Player player in m_players)
        {
            // New player num
            player.PlayerNum = playerNum;
            playerNum++;
        }
    }

    // Sorts players based on their current space, as determined by a dice roll
    static int SortPlayers(Player player1, Player player2)
    {
        if (player1.OrderDeterminingDiceResult < player2.OrderDeterminingDiceResult)
        {
            return 1;
        }
        else if (player1.OrderDeterminingDiceResult > player2.OrderDeterminingDiceResult)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    // Returns a message about the player order
    public string GetOrderDeterminedMessage()
    {
        // Iterate through the players to update their properties and create output message
        string message = "";
        int playerNum = 0;
        foreach (Player player in m_players)
        {
            // Append output for popup
            message += (playerNum + 1) + ": " + player.Name + ", rolled " + player.OrderDeterminingDiceResult + "\n";

            // Append what player we're on
            playerNum++;
        }

        // Return the message
        return message;
    }

    // Player rolled the dice
    public void DiceRolled(int diceResult, bool wereDoubles)
    {
        // Update the current space of player
        int destinationSpace = diceResult + CurrentPlayer.CurrentSpace;
        if (destinationSpace > 39)
        {
            destinationSpace -= 39;
        }
        CurrentPlayer.CurrentSpace = destinationSpace;

        // Update current players "rolled doubles"
        CurrentPlayer.RolledDoubles = wereDoubles;

        // Update the players rolled dice boolean
        CurrentPlayer.RolledDice = true;
    }

    // Casts a string version of an action into the enum type
    public Actions CastActionString(string str)
    {
        // Match string with action
        switch (str)
        {
            case "EndTurn":
                return Actions.EndTurn;
            case "DetermineOrder":
                return Actions.DetermineOrder;
            case "RollDice":
                return Actions.RollDice;
            case "UnownedProperty":
                return Actions.LandedOn_UnownedProperty;
            case "OwnedColorProperty":
                return Actions.LandedOn_OwnedColorProperty;
            case "OwnedUtility":
                return Actions.LandedOn_OwnedUtility;
            case "OwnedRailroad":
                return Actions.LandedOn_OwnedRailroad;
            case "ChanceOrCommunityChest":
                return Actions.LandedOn_ChanceOrCommunityChest;
            case "VisitingJail":
                return Actions.LandedOn_VisitingJail;
            case "FreParking":
                return Actions.LandedOn_FreeParking;
            case "GoToJail":
                return Actions.LandedOn_GoToJail;
            case "Tax":
                return Actions.LandedOn_Tax;
            case "Go":
                return Actions.LandedOn_Go;
            default:
                return Actions.ERROR;
        }
    }
}
