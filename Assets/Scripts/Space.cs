using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class each space on the board
public class Space
{
    // Data Members
    protected string m_name;
    protected int m_index;
    protected string m_actionText = "Action!";
    protected Board.Actions m_actionType;
    protected string m_description;

    // Constructor
    public Space(string name, int index, Board.Actions action, string description)
    {
        Name = name;
        Index = index;
        ActionType = action;
        Description = description;
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
    public virtual string Description
    {
        get { return m_description; }
        set { m_description = value; }  
    }
    public virtual bool IsMortgaged
    {
        get { return false; }
        set { throw new System.Exception("Attempting to mortgage a non-property type!"); }
    }

    public string ActionText
    {
        get { return m_actionText; }
        set { m_actionText = value; }
    }

    // What action a player must do, landing on this property
    public virtual Board.Actions ActionType
    {
        get { return m_actionType; }
        set { m_actionType = value; }
    }

}
