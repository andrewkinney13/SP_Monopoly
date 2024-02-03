/// <summary>
/// 
/// CLASS
///     Card - Stores the data of Community Chest and Chance cards.
///     
/// DESCRIPTION
///     Community Chest and Chance cards have some important data associated with them 
///     that this class stores and organizes. Depending on the "action type" of a card
///     (what the card makes the player do), certain data is stored in that card 
///     object for the game to access. 
/// 
/// </summary>
public class Card
{
    // ======================================== Private Data Members ======================================= //
    Controller_Card.Actions m_actionType;
    string m_description;
    int m_value;
    int m_value2;
    string m_location;

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     Card - Stores the data of Community Chest and Chance cards.
    /// 
    /// SYSNOPSIS
    ///     public Card(Controller_Card.Actions a_actionType, string a_description);
    ///         a_actionType        --> what kind of action this card makes the 
    ///                                 player do after picking it up.
    ///         a_descrtiption      --> description of the action.
    ///     
    /// DESCRIPTION
    ///     Community Chest and Chance cards have some important data associated with them 
    ///     that this class stores and organizes. Depending on the "action type" of a card
    ///     (what the card makes the player do), certain data is stored in that card 
    ///     object for the game to access. 
    /// 
    /// </summary>
    public Card(Controller_Card.Actions a_actionType, string a_description)
    {
        m_actionType = a_actionType;
        m_description = a_description;
    }

    // ======================================== Properties ================================================= //

    // Access card's action type
    public Controller_Card.Actions ActionType { get { return m_actionType; } }

    // Access description of the card
    public string Description { get { return m_description; } }

    /// <summary>
    /// 
    /// NAME
    ///     Value - get and set first value of card.
    ///     
    /// DESCRIPTION
    ///     Cards that use numeric values store them here, such as cards 
    ///     that take from or give money to the player. Also the house cost
    ///     of the repair card is stored here.
    /// 
    /// RETURNS
    ///     First int value associated with card.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if wrong card type is accessing this
    ///     property.
    /// 
    /// </summary>
    public int Value
    {
        set { m_value = value; }
        get
        {
            // Check access exception
            if (m_actionType == Controller_Card.Actions.getJailCard
                || m_actionType == Controller_Card.Actions.move)
                throw new System.Exception("Accessing invalid property (Value) for this card");
            return m_value;
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     Value2 - get and set second value of card.
    ///     
    /// DESCRIPTION
    ///     "Make repairs" card uses this property to store how much 
    ///     hotels cost to repair. 
    /// 
    /// RETURNS
    ///     Second int value associated with card.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if wrong card type is accessing this
    ///     property.
    /// 
    /// </summary>
    public int Value2
    {
        set { m_value2 = value; }
        get
        {
            // Check access exception (only for repairing cards)
            if (m_actionType != Controller_Card.Actions.makeRepairs)
                throw new System.Exception("Accessing invalid property (Value2) for this card");
            return m_value2;
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     Location - get and set location value of card.
    ///     
    /// DESCRIPTION
    ///     "Move" type cards need a string to store the location to
    ///     which they move the player to. This property stores that location.
    /// 
    /// RETURNS
    ///     Location to move to associated with this move card.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if wrong card type is accessing this
    ///     property.
    /// 
    /// </summary>
    public string Location
    {
        set { m_location = value; }
        get
        {
            // Check access exception (only for moving actions)
            if (m_actionType != Controller_Card.Actions.move)
                throw new System.Exception("Accessing invalid property (Value) for this card");
            return m_location;
        }
    }
}
