using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string m_name;
    private float m_cash;
    private List<Property> m_properties = new List<Property>();

    public Player(string m_name) 
    {
        Name = m_name;
        m_cash = 1500.0f;
    }

    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
}
