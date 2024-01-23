using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorProperty : Property
{
    // Data members   
    int m_houseCost;
    int m_numHouses;
    string m_color;

    // Constructor
    public ColorProperty(string name, int index, Board.Actions action, int purchasePrice, int houseCost, List<int> rentPrices, string description, string color) 
        : base (name, index, action, purchasePrice, description)
    {
        Houses = 0;
        m_houseCost = houseCost;
        m_rentPrices = rentPrices;
        m_color = color;
    }

    public override int RentPrice
    {
        get
        {
            // Rent doubles for undeveloped properties if whole set owned
            if (ColorSetOwned && Houses == 0)
            {
                return m_rentPrices[0] * 2;
            }

            // Otherwise standard price
            return m_rentPrices[Houses];
        }
        
    }

    // What action a player must do, landing on this property
    public override Board.Actions ActionType
    {
        get
        {
            if (IsPurchased)
            {
                if (Owner.InJail)
                {
                    return Board.Actions.LandedOn_JailedOwnerProperty;
                }
                if (IsMortgaged)
                {
                    return Board.Actions.LandedOn_MortgagedProperty;
                }
                return Board.Actions.LandedOn_OwnedColorProperty;
            }    
            return Board.Actions.LandedOn_UnownedProperty;
        }
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
    public string Color
    {
        get { return m_color; }
    }

    // Description
    public override string Description
    {
        get 
        {
            // Owned or not
            string retString = "Owner: ";
            if (IsPurchased)
            {
                retString += Owner.Name;
            }
            else
            {
                retString += "No one";
            }

            // Mortgaged or not
            retString += "\nMortgaged: ";
            if (IsMortgaged)
            {
                retString += "Yes";
            }
            else
            {
                retString += "No";
            }

            // Houses
            if (Houses == 5)
            {
                retString += "\nHotels: 1\n";
            }
            else
            {
                retString += "\nHouses: " + Houses + "\n";
            }
            
            retString += "Purchase cost: $" + PurchasePrice + "\n" +
                "RENT: $" + m_rentPrices[0] + "\n" +
                "With 1 House: $" + m_rentPrices[1] + "\n" +
                "With 2 Houses: $" + m_rentPrices[2] + "\n" +
                "With 3 Houses: $" + m_rentPrices[3] + "\n" +
                "With 4 Houses: $" + m_rentPrices[4] + "\n" +
                "With HOTEL: $" + m_rentPrices[5] + "\n" +
                "Mortgage Value: $" + MortgageValue + "\n" +
                "Houses cost $" + HouseCost + " each\n" +
                "Hotels, $" + HouseCost + " each, plus 4 houses";

            return retString;
        }
    }

    // Checks if all properties are owned by the owner, rent is doubled if so
    public bool ColorSetOwned
    {
        get
        {
            // Check every property owner has
            int colorSetCount = 0;
            foreach (Property property in Owner.Properties)
            {
                // Check only color properties
                if (property is ColorProperty)
                {
                    ColorProperty colorProperty = (ColorProperty)property;
                    if (colorProperty.Color == m_color)
                    {
                        colorSetCount++;
                    }
                }
            }

            // If 3, automatically full set
            if (colorSetCount == 3)
            {
                return true;
            }    

            // Brown and dark blue only need 2 
            if (m_color == "Brown" || m_color == "Dark Blue")
            {
                if (colorSetCount == 2)
                {
                    return true;
                }    
            }
            return false;
        }
    }
}
