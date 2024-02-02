using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    // ======================================== Private Data Members ============================= //
    string m_name;
    string m_icon;
    int m_cash;
    int m_playerNum;
    int m_currentSpace;
    int m_diceRollResult;
    int m_orderDeterminingDiceResult;
    bool m_turnInitialized;
    bool m_rolledDice;
    bool m_rolledDoubles;
    bool m_spaceActionCompleted;
    bool m_turnCompleted;
    bool m_inJail;
    int m_comunityChestJailCards;
    int m_chanceJailCards;
    List<Space> m_properties = new List<Space>();

    // ======================================== Constructor ================================================ //
    public Player(string name, string icon, int playerNum) 
    {
        Name = name;
        Icon = icon;
        PlayerNum = playerNum;
        TurnInitialized = false;
        InJail = false;
        CommunityChestJailCards = 0;
        ChanceJailCards = 0;
        m_cash = 1500;
    }

    // ======================================== Properties ================================================= //
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public string Description
    {
        get 
        {
            string retString = "Cash: $" + m_cash + "\n";
            retString += "In Jail: ";
            if (InJail)
            {
                retString += "Yes";
            }
            else
            {
                retString += "No";
            }
            retString += "\nBankrupt: ";
            if (Bankrupt)
            {
                retString += "Yes";
            }
            else
            {
                retString += "No";
            }
            
            retString += "\nProperties:\n";
            int i = 0;
            foreach (Space space in m_properties) 
            {
                retString += space.Name;
                i++;
                if (i != Properties.Count)
                {
                    retString += ", ";
                }
            }
            return retString;
        }
    }
    public string Icon
    {
        get { return m_icon; }
        set { m_icon = value; }
    }
    public int Cash
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
            if (value >= 0 && value <= 39)
            {
                m_currentSpace = value;
            }
            else
            {
                throw new System.Exception("Space index out of range...");
            }
        }
    }
    public int DiceRollResult
    {
        get { return m_diceRollResult; }
        set { m_diceRollResult = value; }
    }
    public int OrderDeterminingDiceResult
    {
        get { return m_orderDeterminingDiceResult; }
        set { m_orderDeterminingDiceResult = value; }
    }
    public bool TurnInitialized
    { 
        get { return m_turnInitialized; }
        set { m_turnInitialized = value; }
    }
    public bool RolledDice
    {
        get { return m_rolledDice; }
        set { m_rolledDice = value; }
    }
    public bool RolledDoubles
    {
        get { return m_rolledDoubles; }
        set { m_rolledDoubles = value; }
    }
    public bool SpaceActionCompleted
    {
        get { return m_spaceActionCompleted; }
        set { m_spaceActionCompleted = value; }
    }
    public bool TurnCompleted
    {
        get { return m_turnCompleted; }
        set { m_turnCompleted = value; }
    }
    public List<Space> Properties
    {   
        get { return m_properties; } 
    }
    public bool OnOwnedProperty
    {
        get
        {
            // Check each property
            foreach (Property property in Properties)
            {
                // Index and current space match
                if (property.Index == CurrentSpace)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool InJail
    {
        get { return m_inJail; }
        set { m_inJail = value; }
    }
    public int ChanceJailCards
    {
        get { return m_chanceJailCards; }
        set { m_chanceJailCards = value; }
    }
    public int CommunityChestJailCards
    {
        get { return m_comunityChestJailCards; }
        set { m_comunityChestJailCards = value; }

    }
    public bool Bankrupt
    {
        get 
        {
            return Cash <= 0;
        }
    }
}
