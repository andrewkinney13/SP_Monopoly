using System.Collections.Generic;

/// <summary>
/// 
/// CLASS
///     Property : Space - spaces that are properties.
///     
/// DESCRIPTION
///     Inherited class from Space, these are spaces which can be purchased
///     by the player. 
/// 
/// </summary>
public class Property : Space
{
    // ======================================== Private Data Members ======================================= //
    protected int m_purchasePrice;
    protected Player m_owner = null;
    protected bool m_purchased = false;
    protected bool m_mortgaged = false;
    protected List<int> m_rentPrices = new List<int>();

    // ======================================== Constructor ================================================ //
    /// <summary>
    /// 
    /// NAME
    ///     Property - constructor for property spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public ColorProperty(string a_name, int a_index, Board.Actions a_action, int a_purchasePrice, int a_houseCost, 
    ///     List<int> a_rentPrices, string a_description, string a_color) 
    ///     : base (a_name, a_index, a_action, a_purchasePrice, a_description);
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_purchasePrice     --> how much it costs to buy the property. 
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     Inheriting from Space, this constructor assigns uniuqe
    ///     data of property space, and sends everything else to the base class constructors.
    /// 
    /// </summary>
    public Property(string a_name, int a_index, Board.Actions a_action, int a_purchasePrice, string a_description) : 
        base(a_name, a_index, a_action, a_description)
    {
        PurchasePrice = a_purchasePrice;
    }

    // ======================================== Virtual Methods ============================================ //
    
    // Rent
    public virtual int RentPrice { get { throw new System.Exception("Virtual base class rent price method being called"); } }

    // ======================================== Override Methods =========================================== //

    // If mortgaged
    public override bool IsMortgaged
    {
        get { return m_mortgaged; }
        set { m_mortgaged = value; }
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

    // Generic information about who own's the property, and if it's mortgaged
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
            return retString;
        }
    }

    // ======================================== Properties ================================================= //

    // How much the property costs for a player to buy
    public int PurchasePrice
    {
        get { return m_purchasePrice; } 
        set { m_purchasePrice = value; }
    }

    // How much the property mortgages for
    public int MortgageValue { get { return m_purchasePrice / 2 ; } }

    // Player who owns the property
    public Player Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    // Flag to indicate whether property has been purchased
    public bool IsPurchased
    { 
        get { return m_purchased; } 
        set { m_purchased = value; }
    }
}
