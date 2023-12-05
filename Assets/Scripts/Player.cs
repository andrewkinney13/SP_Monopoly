using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string m_name;
    private string m_icon;
    private float m_cash;
    private int m_currentSpace;
    private int m_playerNum;

    // Constructor
    public Player(string name, string icon, int playerNum) 
    {
        Name = name;
        Icon = icon;
        PlayerNum = playerNum;

        m_cash = 1500.0f;
    }

    // Getters and setters
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public string Icon
    {
        get { return m_icon; }
        set { m_icon = value; }
    }
    public float Cash
    {
        get { return m_cash; }
        set { m_cash = value; }
    }
    public int PlayerNum
    {
        get { return m_playerNum; } 
        set { m_playerNum = value; }
    }
    public int CurrentSpace
    {
        get { return m_currentSpace; }
        set
        {
            if (value < 0 && value > 39)
            {
                m_currentSpace = value;
            }
            else
            {
                throw new System.Exception("Space index out of range...");
            }
        }
    }
    
}
