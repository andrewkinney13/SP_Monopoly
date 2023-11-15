using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Board 
{
    // Data members
    private List<Space> m_spaces = new List<Space>();
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board

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
}
