/// <summary>
/// 
/// CLASS
///     Tax : Space - inherited space class for tax spaces.
///     
/// DESCRIPTION
///     Controls functionality for spaces which make the player
///     pay a tax when landed on.
/// 
/// </summary>
public class Tax : Space
{
    // ======================================== Private Data Members ======================================= //
    int m_taxCost;

    // ======================================== Constructor ================================================ //
    /// <summary>
    /// 
    /// NAME
    ///     Tax - constructor for tax spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public Tax(string a_name, int a_index, Board.Actions a_action, 
    ///     int a_purchasePrice, int a_rentPrice, string a_description)
    ///     : base (a_name, a_index, a_action, a_purchasePrice, a_description);
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_taxCost           --> How much the tax costs for this space.
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     Inheriting from space, this constructor assigns uniuqe
    ///     data of tax spaces, and sends everything else to the base class constructor.
    /// 
    /// </summary>
    public Tax(string a_name, int a_index, Board.Actions a_action, int a_taxCost, string a_description) 
        : base(a_name, a_index, a_action, a_description)
    {
        m_taxCost = a_taxCost;
    }

    // ======================================== Properties ================================================= //
    
    // Returns how much the tax costs for this tax space
    public int TaxCost { get { return m_taxCost; } }
}
