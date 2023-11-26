
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Board 
{
    // Data members
    private List<Space> m_spaces = new List<Space>();
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board
    private Dictionary<string, Player> m_players = new Dictionary<string, Player>(); // {name : player} 

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
    }

    // Obtains spaces from text file
    public void InitializeSpaces()
    {
        // Obtain space details from file
        string namesFilePath = Path.Combine(Application.streamingAssetsPath, "spaceNames.txt");
        string[] names = File.ReadAllLines(namesFilePath);

        // Add those details to the list
        for (int i = 0; i < SPACE_NUM; i++)
        {
            Space currentSpace = new Space(names[i], i);
            m_spaces.Add(currentSpace);
        }
    }

    // Obtains the players from PlayerFile class
    public void InitializePlayers()
    {
        // Create player file object
        PlayerFile playerFile = new PlayerFile();
        
        // Create temp dicts to store players
        Dictionary<string, string> tempPlayers = new Dictionary<string, string>();

        // Obtain players from the file
        tempPlayers = playerFile.ReadPlayersFromFile();

        // Add them to the players list
        foreach (string name in tempPlayers.Keys)
        {
            Player currentPlayer = new Player(name, tempPlayers[name]);
            m_players[name] = currentPlayer;
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
}
