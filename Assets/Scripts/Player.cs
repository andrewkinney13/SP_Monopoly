using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string m_name;
    private string m_icon;
    private float m_cash;
    private List<Property> m_properties = new List<Property>();

    // Constructor
    public Player(string name, string icon) 
    {
        Name = name;
        Icon = icon;
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
}
