using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    // Data members
    CardController.Actions m_actionType;
    string m_description;
    int m_value;
    int m_value2;
    string m_location;

    // Constructor
    public Card(CardController.Actions actionType, string description)
    {
        m_actionType = actionType;
        m_description = description;
    }

    // Getters and setters
    public CardController.Actions ActionType
    {
        get { return m_actionType; }
    }
    public string Description
    {
        get { return m_description; }
    }
    public int Value
    {
        set { m_value = value; }
        get
        {
            // Check access exception
            if (m_actionType == CardController.Actions.getJailCard
                || m_actionType == CardController.Actions.move)
                throw new System.Exception("Accessing invalid property (Value) for this card");
            return m_value;
        }
    }
    public int Value2
    {
        set { m_value2 = value; }
        get
        {
            // Check access exception
            if (m_actionType != CardController.Actions.makeRepairs)
                throw new System.Exception("Accessing invalid property (Value2) for this card");
            return m_value2;
        }
    }
    public string Location
    {
        set { m_location = value; }
        get
        {
            // Check access exception
            if (m_actionType != CardController.Actions.move)
                throw new System.Exception("Accessing invalid property (Value) for this card");
            return m_location;
        }
    }
}
