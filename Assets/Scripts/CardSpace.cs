using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpace : Space
{
    // ============================== Private Data Members ============================= //
    Controller_Card m_cardController;

    // ============================== Constructor ====================================== //
    public CardSpace(string name, int index, Board.Actions action, Controller_Card cardController, string description) : base(name, index, action, description)
    {
        // Assign card controller reference
        m_cardController = cardController;
    }

    // ============================== Public Methods =================================== //

    // Taking card method
    public Card TakeCard()
    {
        // Return appropriate card from respective deck
        if (Name == "Chance")
        {
            return m_cardController.TakeChanceCard();
        }
        else
        {
            return m_cardController.TakeCommunityChestCard();
        }
    }
}
