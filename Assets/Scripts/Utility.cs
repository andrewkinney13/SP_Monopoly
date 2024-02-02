using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Property
{
    // ======================================== Private Data Members ============================= //
    int m_currentDiceRoll;
    bool m_diceRolled;

    // ======================================== Constructor ================================================ //
    public Utility(string name, int index, Board.Actions action, int purchasePrice, string description) : 
        base (name, index, action, purchasePrice, description) { }

    // ======================================== Override Methods =========================================== //

    // What action a player must do, landing on this property
    public override Board.Actions ActionType
    {
        get
        {
            if (IsPurchased && !DiceRolled)
            {
                if (Owner.InJail)
                {
                    return Board.Actions.LandedOn_JailedOwnerProperty;
                }
                if (IsMortgaged)
                {
                    return Board.Actions.LandedOn_MortgagedProperty;
                }
                return Board.Actions.DetermineUtilityCost;
            }
            if (IsPurchased && DiceRolled)
            {
                return Board.Actions.LandedOn_OwnedUtility;
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
            retString += "\nIf one Utility is owned, rent is\n" +
                "4 times amount shown on dice.\n" +
                "If both Utilities are owned, rent is 10 times the amount shown on dice.\n" +
                "Mortgage Value: $" + MortgageValue;
            return retString;
        }
    }

    // Returns rent when a user lands on this space
    public override int RentPrice
    {
        get { return RentMultiplier * CurrentDiceRoll; }
    }

    // ======================================== Properties ================================================= //

    // What is multiplied by dice
    public int RentMultiplier
    {
        get 
        { 
            // Utilities both owned gived 10x, just one owned gived 4x
            if (IsAllied)
            {
                return 10;
            }
            return 4;
        }
    }
    
    // What the current dice roll is
    public int CurrentDiceRoll
    {
        get { return m_currentDiceRoll; }
        set { m_currentDiceRoll = value; }
    }

    // Whether or not the player has rolled the dice
    public bool DiceRolled
    {
        get { return m_diceRolled; }
        set { m_diceRolled = value; }
    }

    // Checks if the owner owns the other utility
    public bool IsAllied
    {
        get
        {
            // Go through every property the owner has and count utility types
            int utilityCount = 0;
            foreach (Property property in Owner.Properties)
            {
                // Add utilities as found
                if (property is Utility)
                {
                    utilityCount++;
                }
            }

            // Only return true if 2 utility types found
            if (utilityCount == 2)
            {
                return true;
            }
            return false;
        }
    } 
}
