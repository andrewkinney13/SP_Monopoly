using System;
using System.Collections.Generic;
using Unity.VisualScripting;

// Class for spaces that are purchasable by the player
public class Property : Space
{
    // ============================== Protected Data Members =========================== //
    protected int m_purchasePrice;
    protected Player m_owner = null;
    protected bool m_purchased = false;
    protected bool m_mortgaged = false;
    protected List<int> m_rentPrices = new List<int>();

    // ============================== Constructor ====================================== //
    public Property(string name, int index, Board.Actions action, int purchasePrice, string description) : 
        base(name, index, action, description)
    {
        PurchasePrice = purchasePrice;
    }

    // ============================== Virtual Methods ================================== //
    
    // Rent
    public virtual int RentPrice { get { throw new System.Exception("Virtual base class rent price method being called"); } }

    // ============================== Override Methods ================================= //

    // If mortgaged
    public override bool IsMortgaged
    {
        get { return m_mortgaged; }
        set { m_mortgaged = value; }
    }

    // ============================== Properties ======================================= //
    public int PurchasePrice
    {
        get { return m_purchasePrice; } 
        set { m_purchasePrice = value; }
    }
    public int MortgageValue
    {
        get { return m_purchasePrice / 2 ; }
    }
    public Player Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }
    public bool IsPurchased
    { 
        get { return m_purchased; } 
        set { m_purchased = value; }
    }
}
