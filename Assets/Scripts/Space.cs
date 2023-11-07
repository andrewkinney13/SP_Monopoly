using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class each space on the board
public class Space
{
    // Data Members
    private string m_name;
    private int m_index;
    private string m_description;

    // Constructor
    public Space(string name, int index)
    {
        Name = name;
        Index = index;
        Description = Name + ", at index: " + Index;
    }

    // Getters and Setters
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public int Index
    {
        get { return m_index; }
        set { m_index = value; }
    }
    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }
    

}
