using System.Collections.Generic;

// Class for spaces that are purchasable by the player
public class Property : Space
{
    // Data members
    private int m_purchasePrice;
    private Player m_owner;

    // Constructor
    public Property(string name, int index, Board.Actions action, int purchasePrice, string description) : base(name, index, action, description)
    {
        PurchasePrice = purchasePrice;
    }

    // Getters and setters
    public int PurchasePrice
    {
        get { return m_purchasePrice; } 
        set { m_purchasePrice = value; }
    }
    public int MortgageValue
    {
        get { return m_purchasePrice / 2 ; }
    }
}
