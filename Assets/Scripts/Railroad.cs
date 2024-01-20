using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Railroad : Property
{
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
    public override int RentPrice
    {
        get
        {
            return m_rentPrices[AlliedRailroads - 1];
        }
    }

    // What action a player must do, landing on this property
    public override Board.Actions ActionType
    {
        get
        {
            if (IsPurchased)
            {
                return Board.Actions.LandedOn_OwnedRailroad;
            }
            if (IsMortgaged)
            {
                return Board.Actions.LandedOn_MortgagedProperty;
            }
            return Board.Actions.LandedOn_UnownedProperty;
        }
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
            retString += "\nRENT: $" + m_rentPrices[0] + "\n" +
                "If 2 Railroads are owned: $" + m_rentPrices[1] + "\n" +
                "If 3 Railroads are owned: $" + m_rentPrices[2] + "\n" +
                "If 4 Railroads are owned: $" + m_rentPrices[3] + "\n" +
                "Mortgage Value: $" + MortgageValue;

            return retString;
        }
    }

    // How many railroads are also owned
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
                {
                    railroadCount++;
                }
            }
            return railroadCount;
        }
    }
}
