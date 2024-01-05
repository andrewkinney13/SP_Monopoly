using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorProperty : Property
{
    // Data members   
    List<int> m_landOnPrices = new List<int>();
    int m_houseCost;

    // Constructor
    public ColorProperty(string name, int index, Board.Actions action, int purchasePrice, int houseCost, List<int> landOnPrices, string description) 
        : base (name, index, action, purchasePrice, description)
    {
        m_houseCost = houseCost;
        m_landOnPrices = landOnPrices;
    }

    public int LandOnPrice(int houseNum)
    {
        return m_landOnPrices[houseNum]; 
    }
    public int HouseCost
    {
        get { return m_houseCost; } 
    }
    public override string Description
    {
        get { return "Cost: " + PurchasePrice; }
    }
}
