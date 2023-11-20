
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        // Obtain names list from file
        string namesFile = "spaceNames.txt";
        string[] names = File.ReadAllLines(namesFile);

        for (int i = 0; i < SPACE_NUM; i++) 
        {
            Space currentSpace = new Space(names[i], i);
            m_spaces.Add(currentSpace);
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

    public void AddPlayer()
    {

    }
}
