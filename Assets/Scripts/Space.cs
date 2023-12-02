using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class each space on the board
public class Space
{
    // Data Members
    private string m_name;
    private int m_index;
    private string m_description;
    private string m_actionText = "Action!";

    // Constructor
    public Space(string name, int index)
    {
        Name = name;
        Index = index;
        Description = "Owner: \nCost:\nHouses:\nHotels:\n";
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
    public string ActionText
    {
        get { return m_actionText; }
        set { m_actionText = value; }
    }

    // Land on space (to inherit)
    public void Action()
    {
        Debug.Log("Action!");
    }
}
