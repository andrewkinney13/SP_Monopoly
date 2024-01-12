using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorProperty : Property
{
    // Data members   
    int m_houseCost;
    int m_numHouses = 0;

    // Constructor
    public ColorProperty(string name, int index, Board.Actions action, int purchasePrice, int houseCost, List<int> rentPrices, string description) 
        : base (name, index, action, purchasePrice, description)
    {
        m_houseCost = houseCost;
        m_rentPrices = rentPrices;
    }

    public override int RentPrice(int houseNum)
    {
        return m_rentPrices[houseNum];
    }
    public int HouseCost
    {
        get { return m_houseCost; }
    }
    public int Houses
    {
        get { return m_numHouses; }
        set { m_numHouses = value; }
    }

    // Description
    public override string Description
    {
        get 
        {
            string retString = "Owner: ";
            if (IsPurchased)
            {
                retString += Owner.Name;
            }
            else
            {
                retString += "No one";
            }
            retString += "\nHouses: " + Houses + "\n";
            retString += "Purchase cost: $" + PurchasePrice + "\n" +
                "RENT: $" + RentPrice(0) + "\n" +
                "With 1 House: $" + RentPrice(1) + "\n" +
                "With 2 Houses: $" + RentPrice(2) + "\n" +
                "With 3 Houses: $" + RentPrice(3) + "\n" +
                "With 4 Houses: $" + RentPrice(4) + "\n" +
                "With HOTEL: $" + RentPrice(5) + "\n" +
                "Mortgage Value: $" + MortgageValue + "\n" +
                "Houses cost $" + HouseCost + " each\n" +
                "Hotels, $" + HouseCost + " each, plus 4 houses";

            return retString;
        }
    }


    // Updating the action type 
    public override void UpdateActionType()
    {
        if (IsMortgaged)
        {
            ActionType = Board.Actions.LandedOn_MortgagedProperty;
        }
        else
        {
            ActionType = Board.Actions.LandedOn_OwnedColorProperty;
        }   
    }
}
