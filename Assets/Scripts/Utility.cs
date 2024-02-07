/// <summary>
/// 
/// CLASS
///     Utility : Property - inherited property class for utility spaces.
///     
/// DESCRIPTION
///     Controls functionality for spaces who are purchasable by players, but are 
///     utilities.
/// 
/// </summary>
public class Utility : Property
{
    // ======================================== Private Data Members ======================================= //
    int m_currentDiceRoll;
    bool m_diceRolled;

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     Utility - constructor for utility spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public Utility(string a_name, int a_index, Board.Actions a_action, 
    ///     int a_purchasePrice, int a_rentPrice, string a_description)
    ///     : base (a_name, a_index, a_action, a_purchasePrice, a_description);
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_purchasePrice     --> How much it costs to purchase this property.
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     Inheriting from Property, which inherits from Space, this constructor 
    ///     sends everything to the base class constructors.
    /// 
    /// </summary>
    public Utility(string a_name, int a_index, Board.Actions a_action, int a_purchasePrice, string a_description) : 
        base (a_name, a_index, a_action, a_purchasePrice, a_description) { }

    // ======================================== Override Methods =========================================== //

    /// <summary>
    /// 
    /// NAME
    ///     ActionType - accessor for this property's action when landed on.
    ///     
    /// DESCRIPTION
    ///     This method determines what a player must do when they land on this
    ///     property, based on various factors of the property itself and the owner. 
    ///     Order of what is being checked is important.
    /// 
    /// RETURNS
    ///     What a player must do after landing on this property.
    /// 
    /// </summary>
    public override Board.Actions ActionType
    {
        get
        {
            if (IsPurchased && !DiceRolled)
            {
                if (Owner.InJail)
                    return Board.Actions.LandedOn_JailedOwnerProperty;

                if (IsMortgaged)
                    return Board.Actions.LandedOn_MortgagedProperty;

                return Board.Actions.DetermineUtilityCost;
            }
            if (IsPurchased && DiceRolled)
                return Board.Actions.LandedOn_OwnedUtility;

            return Board.Actions.LandedOn_UnownedProperty;
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     Description - accessor for this property's description.
    ///     
    /// DESCRIPTION
    ///     This method compiles a description of the property, based on
    ///     it's current data. Uses property base class implementation
    ///     for generic information.
    /// 
    /// RETURNS
    ///     String of details about the property. 
    /// 
    /// </summary>
    public override string Description
    {
        get
        {
            // Get generic information from base class
            string retString = base.Description;

            // Get Utility information
            retString += "\nIf one Utility is owned, rent is\n" +
                "4 times amount shown on dice.\n" +
                "If both Utilities are owned, rent is 10 times the amount shown on dice.\n" +
                "Mortgage Value: $" + MortgageValue;
            return retString;
        }
    }

    // Returns rent cost when a user lands on this space
    public override int RentPrice { get { return RentMultiplier * CurrentDiceRoll; } }

    // ======================================== Properties ================================================= //

    // Determines the rent multiplier 
    public int RentMultiplier
    {
        get 
        { 
            // Utilities both owned gived 10x, just one owned gived 4x
            if (IsAllied)
                return 10;

            return 4;
        }
    }
    
    // What the current dice roll is
    public int CurrentDiceRoll
    {
        get { return m_currentDiceRoll; }
        set { m_currentDiceRoll = value; }
    }

    // Whether or not the player has rolled the dice
    public bool DiceRolled
    {
        get { return m_diceRolled; }
        set { m_diceRolled = value; }
    }

    // Checks if the owner owns both utilities
    public bool IsAllied
    {
        get
        {
            // Go through every property the owner has and count utility types
            int utilityCount = 0;
            foreach (Property property in Owner.Properties)
            {
                // Add utilities as found
                if (property is Utility)
                    utilityCount++;
            }

            // Only return true if 2 utility types found
            if (utilityCount == 2)
                return true;
            return false;
        }
    } 
}
