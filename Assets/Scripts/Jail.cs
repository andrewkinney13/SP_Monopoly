using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jail : Space
{
    // Constructor
    public Jail(string name, int index, Board.Actions action, string description) : base(name, index, action, description)
    {
    }
}
