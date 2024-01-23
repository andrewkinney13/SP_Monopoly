using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CardController
{
    // Data members
    private List<Card> m_chanceCardDeck = new List<Card>();
    private List<Card> m_communityChestCardDeck = new List<Card>();
    public enum Actions
    {
        collectMoney,
        payMoney,
        move,
        makeRepairs,
        getJailCard
    }

    // Constructor
    public CardController()
    {
        // Init cards lists
        InitializeCardLists();
    }

    // Take card functions
    public Card TakeChanceCard()
    {
        // Take the card, move it to back of list
        Card retCard = m_chanceCardDeck[0];
        m_chanceCardDeck.RemoveAt(0);
        m_chanceCardDeck.Add(retCard);
        return retCard;
    }
    public Card TakeCommunityChestCard()
    {
        // Take the card, move it to back of list
        Card retCard = m_communityChestCardDeck[0];
        m_communityChestCardDeck.RemoveAt(0);
        m_communityChestCardDeck.Add(retCard);
        return retCard;
    }


    // Initialze the lists of cards from data file
    void InitializeCardLists()
    {
        // Obtain file data
        string filePath = Path.Combine(Application.streamingAssetsPath, "cardData.txt");
        string[] lines = File.ReadAllLines(filePath);

        // Parse each line
        foreach (string line in lines)
        {
            // Obtain values
            string[] vals = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Create the card
            Actions action = CastActionString(vals[1]);
            Card card = new Card(action, vals[2]);

            // Depending on action, add properties
            switch(action)
            {
                // Card needs one value
                case Actions.collectMoney:
                case Actions.payMoney:
                    card.Value = int.Parse(vals[3]);
                    break;

                // Card needs two values
                case Actions.makeRepairs:
                    card.Value = int.Parse(vals[3]);
                    card.Value2 = int.Parse(vals[4]);
                    break;

                // Card has a location value
                case Actions.move:
                    card.Location = vals[3];
                    break;
            }

            // Add to approptiate list
            if (vals[0] == "Chance")
            {
                m_chanceCardDeck.Add(card);
            }
            else
            {
                m_communityChestCardDeck.Add(card);
            }
        }
    }

    // Casts action string from text file to actual action type
    Actions CastActionString(string action)
    {
        switch(action)
        {
            case "collectMoney":
                return Actions.collectMoney;
            case "payMoney":
                return Actions.payMoney;
            case "move":
                return Actions.move;
            case "makeRepairs":
                return Actions.makeRepairs;
            case "getJailCard":
                return Actions.getJailCard;
            default:
                throw new Exception("Card data type not found, type: " + action);
        }
    }
}
