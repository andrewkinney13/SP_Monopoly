using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : Property
{
    // Data members

    // Constructor
    public Utility(string name, int index, Board.Actions action, int purchasePrice, int rentPrice, string description)
        : base (name, index, action, purchasePrice, description)
    {
        
    }
}
