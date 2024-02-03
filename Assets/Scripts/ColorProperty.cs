using System.Collections.Generic;

/// <summary>
/// 
/// CLASS
///     ColorProperty : Property - inherited property class for color spaces.
///     
/// DESCRIPTION
///     Controls functionality for spaces who are purchasable by players, but are part of 
///     color sets that can have houses and hotels developed on them.
/// 
/// </summary>
public class ColorProperty : Property
{
    // ======================================== Private Data Members ======================================= //
    int m_houseCost;
    int m_numHouses;
    string m_color;

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     ColorProperty - constructor for color property spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public ColorProperty(string a_name, int a_index, Board.Actions a_action, int a_purchasePrice, int a_houseCost, 
    ///     List<int> a_rentPrices, string a_description, string a_color) 
    ///     : base (a_name, a_index, a_action, a_purchasePrice, a_description)
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_purchasePrice     --> How much it costs to purchase this property.
    ///         a_houseCost         --> How much houses and hotels cost to buy on this property.
    ///         a_rentPrices        --> Rent price for all cases of landing on this property.
    ///         a_description       --> Description of this space.
    ///         a_color             --> Color of the property. 
    ///     
    /// DESCRIPTION
    ///     Inheriting from Property, which inherits from Space, this constructor assigns uniuqe
    ///     data of color properties, and sends everything else to the base class constructors.
    /// 
    /// </summary>
    public ColorProperty(string a_name, int a_index, Board.Actions a_action, int a_purchasePrice, int a_houseCost, 
        List<int> a_rentPrices, string a_description, string a_color) 
        : base (a_name, a_index, a_action, a_purchasePrice, a_description)
    {
        Houses = 0;
        m_houseCost = a_houseCost;
        m_rentPrices = a_rentPrices;
        m_color = a_color;
    }

    // ======================================== Override Methods =========================================== //

    /// <summary>
    /// 
    /// NAME
    ///     RentPrice - accessor for this property's rent price.
    ///     
    /// DESCRIPTION
    ///     Derived implementation of this property's rent price. Determined by
    ///     how much the space is developed; or if the full color set is owned.
    /// 
    /// RETURNS
    ///     The rent price for landing on this property.
    /// 
    /// </summary>
    public override int RentPrice
    {
        get
        {
            // Rent doubles for undeveloped properties if whole set owned
            if (ColorSetOwned && Houses == 0)
                return m_rentPrices[0] * 2;

            // Otherwise standard price
            return m_rentPrices[Houses];
        }
    }

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
            if (IsPurchased)
            {
                if (Owner.InJail)
                    return Board.Actions.LandedOn_JailedOwnerProperty;
                
                if (IsMortgaged)
                    return Board.Actions.LandedOn_MortgagedProperty;
                
                return Board.Actions.LandedOn_OwnedColorProperty;
            }
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
    ///     it's current data. 
    /// 
    /// RETURNS
    ///     String of details about the property. 
    /// 
    /// </summary>
    public override string Description
    {
        get
        {
            // Owned or not
            string retString = "Owner: ";
            if (IsPurchased)
                retString += Owner.Name;
            else
                retString += "No one";
            

            // Mortgaged or not
            retString += "\nMortgaged: ";
            if (IsMortgaged)
                retString += "Yes";
            else
                retString += "No";
            

            // Houses
            if (Houses == 5)
                retString += "\nHotels: 1\n";
            else
                retString += "\nHouses: " + Houses + "\n";
            
            // Add rest of description, static
            retString += "Purchase cost: $" + PurchasePrice + "\n" +
                "RENT: $" + m_rentPrices[0] + "\n" +
                "With 1 House: $" + m_rentPrices[1] + "\n" +
                "With 2 Houses: $" + m_rentPrices[2] + "\n" +
                "With 3 Houses: $" + m_rentPrices[3] + "\n" +
                "With 4 Houses: $" + m_rentPrices[4] + "\n" +
                "With HOTEL: $" + m_rentPrices[5] + "\n" +
                "Mortgage Value: $" + MortgageValue + "\n" +
                "Houses cost $" + HouseCost + " each\n" +
                "Hotels, $" + HouseCost + " each, plus 4 houses";

            return retString;
        }
    }
    /* public override string Description */

    // ======================================== Properties ================================================= //

    // Accessor for much houses cost to purchase
    public int HouseCost { get { return m_houseCost; } }

    // Accessor for how many houses are on this property
    public int Houses
    {
        get { return m_numHouses; }
        set { m_numHouses = value; }
    }

    // Accessor for the color of this property 
    public string Color { get { return m_color; } }

    /// <summary>
    /// 
    /// NAME
    ///     ColorSetOwned - accessor for this property's description.
    ///     
    /// DESCRIPTION
    ///     Determines whether or not the owner has the entire
    ///     color set collection for a particular color group. 
    /// 
    /// RETURNS
    ///     Whether or not the full color set is owned.
    /// 
    /// </summary>
    public bool ColorSetOwned
    {
        get
        {
            // Check every property owner has
            int colorSetCount = 0;
            foreach (Property property in Owner.Properties)
            {
                // Check only color properties
                if (property is ColorProperty)
                {
                    ColorProperty colorProperty = (ColorProperty)property;
                    if (colorProperty.Color == m_color)
                        colorSetCount++;
                }
            }

            // If 3, automatically full set
            if (colorSetCount == 3)
                return true;

            // Brown and dark blue only need 2 
            if (m_color == "Brown" || m_color == "Dark Blue")
            {
                if (colorSetCount == 2)
                    return true;
            }

            return false;
        }
    }
    /* public bool ColorSetOwned */
}
