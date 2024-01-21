using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tax : Space
{
    // Data members
    private int m_taxCost;

    // Constructor
    public Tax(string name, int index, Board.Actions action, int taxCost, string description) : base(name, index, action, description)
    {
        m_taxCost = taxCost;
    }

    // Getters and setters
    public int TaxCost 
    { 
        get { return m_taxCost; } 
    }



    
}
