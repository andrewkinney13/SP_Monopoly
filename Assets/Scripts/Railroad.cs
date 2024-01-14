using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Railroad : Property
{
    // Data members

    // Constructor
    public Railroad(string name, int index, Board.Actions action, int purchasePrice, int rentPrice, string description)
        : base (name, index, action, purchasePrice, description)
    {
        // Initialize land on prices
        m_rentPrices.Add(rentPrice);
        m_rentPrices.Add(rentPrice * 2);
        m_rentPrices.Add(rentPrice * 4);
        m_rentPrices.Add(rentPrice * 8);

    }

    // Returns land on price for how many railroads owned
    public override int RentPrice(int railroadsNum)
    {
        return m_rentPrices[railroadsNum - 1];
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
            retString += "\nMortgaged: ";
            if (IsMortgaged) 
            {
                retString += "Yes";
            }
            else
            {
                retString += "No";
            }
            retString += "\nRENT: $" + RentPrice(1) + "\n" +
                "If 2 Railroads are owned: $" + RentPrice(2) + "\n" +
                "If 3 Railroads are owned: $" + RentPrice(3) + "\n" +
                "If 4 Railroads are owned: $" + RentPrice(4) + "\n" +
                "Mortgage Value: $" + MortgageValue;

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
            ActionType = Board.Actions.LandedOn_OwnedRailroad;
        }
    }
}
