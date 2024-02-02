using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tax : Space
{
    // ======================================== Private Data Members ============================= //
    int m_taxCost;

    // ======================================== Constructor ================================================ //
    public Tax(string name, int index, Board.Actions action, int taxCost, string description) : base(name, index, action, description)
    {
        m_taxCost = taxCost;
    }

    // ======================================== Properties ================================================= //
    public int TaxCost 
    { 
        get { return m_taxCost; } 
    }
}
