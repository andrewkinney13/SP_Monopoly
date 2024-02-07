/// <summary>
/// 
/// CLASS
///     Railroad : Property - inherited property class for railroad spaces.
///     
/// DESCRIPTION
///     Controls functionality for spaces who are purchasable by players, but are 
///     railroads.
/// 
/// </summary>
public class Railroad : Property
{

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     Railroad - constructor for railroad spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public Railroad(string a_name, int a_index, Board.Actions a_action, 
    ///     int a_purchasePrice, int a_rentPrice, string a_description)
    ///     : base (a_name, a_index, a_action, a_purchasePrice, a_description);
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_purchasePrice     --> How much it costs to purchase this property.
    ///         a_rentPrice         --> Initial rent price for landing on this space.
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     Inheriting from Property, which inherits from Space, this constructor assigns uniuqe
    ///     data of railroads, and sends everything else to the base class constructors.
    /// 
    /// </summary>
    public Railroad(string a_name, int a_index, Board.Actions a_action, 
        int a_purchasePrice, int a_rentPrice, string a_description)
        : base (a_name, a_index, a_action, a_purchasePrice, a_description)
    {
        // Initialize land on prices
        m_rentPrices.Add(a_rentPrice);
        m_rentPrices.Add(a_rentPrice * 2);
        m_rentPrices.Add(a_rentPrice * 4);
        m_rentPrices.Add(a_rentPrice * 8);
    }

    // ======================================== Override Methods =========================================== //

    
    public override int RentPrice { get { return m_rentPrices[AlliedRailroads - 1]; } }

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

            // Get railroad specific data
            retString += "\nRENT: $" + m_rentPrices[0] + "\n" +
                "If 2 Railroads are owned: $" + m_rentPrices[1] + "\n" +
                "If 3 Railroads are owned: $" + m_rentPrices[2] + "\n" +
                "If 4 Railroads are owned: $" + m_rentPrices[3] + "\n" +
                "Mortgage Value: $" + MortgageValue;

            return retString;
        }
    }

    // ======================================== Properties ================================================= //

    // Returns how many railroads in total, the player owns. 
    public int AlliedRailroads
    {
        get
        {
            // Parse every property and find out how many are railroads
            int railroadCount = 0;
            foreach (Property property in Owner.Properties)
            {
                // Only count if of type railroad
                if (property is Railroad)
                    railroadCount++;
            }
            return railroadCount;
        }
    }
}
