using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Property
{
    // Data members
    private int m_rentMultiplier;

    // Constructor
    public Utility(string name, int index, Board.Actions action, int purchasePrice, int multiplier, string description)
        : base (name, index, action, purchasePrice, description)
    {
        
    }

    // What is multiplied by dice
    public int RentMultiplier
    {
        get { return m_rentMultiplier; }
        set { m_rentMultiplier = value; }
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
            retString += "\nIf one Utility is owned, rent is\n" +
                "4 times amount shown on dice.\n" +
                "If both Utilities are owned, rent is 10 times the amount shown on dice.\n" +
                "Mortgage Value: $" + MortgageValue;
            return retString;
        }
    }

    // Returns rent when a user lands on this space
    public override int RentPrice(int diceVal)
    {
        return RentMultiplier * diceVal;
    }
}
