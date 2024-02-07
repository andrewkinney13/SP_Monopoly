using System.Collections.Generic;

/// <summary>
/// 
/// CLASS
///     Player - class for player objects.
///     
/// DESCRIPTION
///     This class stores data for the player, and all
///     relevent flags a player might have for the 
///     entire game or just their turn. Also stores
///     their properties.
/// 
/// </summary>
public class Player
{
    // ======================================== Private Data Members ======================================= //
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

    /// <summary>
    /// 
    /// NAME
    ///     Player - constructor for Player objects.
    /// 
    /// SYSNOPSIS
    ///     public Player(string a_name, string a_icon, int a_playerNum); 
    ///         a_name          --> name of the player.
    ///         a_icon          --> player's icon's name.
    ///         a_playerNum     --> players index in the game.
    ///     
    /// DESCRIPTION
    ///     Constructs a player object based on parameters. Also
    ///     initializes some of the flags and numeric data for a
    ///     new player.
    /// 
    /// </summary>
    public Player(string a_name, string a_icon, int a_playerNum) 
    {
        Name = a_name;
        Icon = a_icon;
        PlayerNum = a_playerNum;
        TurnInitialized = false;
        InJail = false;
        CommunityChestJailCards = 0;
        ChanceJailCards = 0;
        m_cash = 1500;
    }

    // ======================================== Properties ================================================= //

    // Name of the player
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    // Description of the player (relevent flags, properties)
    public string Description
    {
        get 
        {
            // Cash
            string retString = "Cash: $" + m_cash + "\n";

            // Jail status
            retString += "In Jail: ";
            if (InJail)
                retString += "Yes";
            else
                retString += "No";

            // Bankrupt status
            retString += "\nBankrupt: ";
            if (Bankrupt)
                retString += "Yes";
            else
                retString += "No";
            
            // Properties they have
            retString += "\nProperties:\n";
            int i = 0;
            foreach (Space space in m_properties) 
            {
                retString += space.Name;
                i++;
                if (i != Properties.Count)
                    retString += ", ";
            }
            return retString;
        }
    }

    // Name of the icon of the player
    public string Icon
    {
        get { return m_icon; }
        set { m_icon = value; }
    }

    // Amount of cash player has
    public int Cash
    {
        get { return m_cash; }
        set { m_cash = value; }
    }

    // Player's index within the game
    public int PlayerNum
    {
        get { return m_playerNum; } 
        set { m_playerNum = value; }
    }

    // The current space the player is on
    public int CurrentSpace
    {
        get { return m_currentSpace; }
        set
        {
            if (value >= 0 && value <= 39)
                m_currentSpace = value;
            else
                throw new System.Exception("Space index out of range...");
        }
    }

    // Result of the player's last dice roll
    public int DiceRollResult
    {
        get { return m_diceRollResult; }
        set { m_diceRollResult = value; }
    }

    // Flag to indicate whether or not player has are determining order with this dice roll
    public int OrderDeterminingDiceResult
    {
        get { return m_orderDeterminingDiceResult; }
        set { m_orderDeterminingDiceResult = value; }
    }

    // Flag to indicate whether or not player has initialized their turn yet at the start of a game
    public bool TurnInitialized
    { 
        get { return m_turnInitialized; }
        set { m_turnInitialized = value; }
    }

    // Flag to indicate whether or not player has rolled the dice yet
    public bool RolledDice
    {
        get { return m_rolledDice; }
        set { m_rolledDice = value; }
    }

    // Flag to indicate whether or not player has rolled doubles on their last roll
    public bool RolledDoubles
    {
        get { return m_rolledDoubles; }
        set { m_rolledDoubles = value; }
    }

    // Flag to indicate whether or not player has completed the action for the space they're on
    public bool SpaceActionCompleted
    {
        get { return m_spaceActionCompleted; }
        set { m_spaceActionCompleted = value; }
    }

    // Flag to indicate whether or not player has completed all actions of their turn
    public bool TurnCompleted
    {
        get { return m_turnCompleted; }
        set { m_turnCompleted = value; }
    }

    // List of player's owned properties
    public List<Space> Properties { get { return m_properties; } }

    // Wether or not the player is on their own property
    public bool OnOwnedProperty
    {
        get
        {
            // Check each property
            foreach (Property property in Properties)
            {
                // Index and current space match
                if (property.Index == CurrentSpace)
                    return true;
            }
            return false;
        }
    }

    // Flag to indicate if player is in jail
    public bool InJail
    {
        get { return m_inJail; }
        set { m_inJail = value; }
    }

    // Returns the chance cards the player has
    public int ChanceJailCards
    {
        get { return m_chanceJailCards; }
        set { m_chanceJailCards = value; }
    }

    // Returns the community chest jail cards the player has
    public int CommunityChestJailCards
    {
        get { return m_comunityChestJailCards; }
        set { m_comunityChestJailCards = value; }

    }

    // Returns if the player has been bankrupted
    public bool Bankrupt { get { return Cash <= 0; } }
}
