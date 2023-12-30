

using System.Collections.Generic;

// Class for spaces that are purchasable by the player
public class Property : Space
{
    // Data members
    private int m_price;
    private Player m_owner;

    // Constructor
    public Property(string name, int index, Board.Actions action) : base(name, index, action) 
    { 

    }
}
