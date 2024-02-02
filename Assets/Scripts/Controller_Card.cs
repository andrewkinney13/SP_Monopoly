using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Controller_Card
{
    // ======================================== Public Data Members ======================================== //
    public enum Actions
    {
        collectMoney,
        payMoney,
        move,
        makeRepairs,
        getJailCard
    }

    // ======================================== Private Data Members ============================= //
    List<Card> m_chanceCardDeck = new List<Card>();
    List<Card> m_communityChestCardDeck = new List<Card>();


    // ======================================== Constructor ================================================ //
    public Controller_Card()
    {
        // Init cards lists
        InitializeCardLists();

        // Suffle card lists
        ShuffleCardDeck(m_chanceCardDeck);
        ShuffleCardDeck(m_communityChestCardDeck);
    }

    // ======================================== Public Methods ============================================= //

    // Take card methods
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

    // ======================================== Private Methods ============================================ //

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

    // Shuffles a deck
    void ShuffleCardDeck(List<Card> cards)
    {
        System.Random random = new System.Random();

        // Loop through each card, swap with a random index
        for (int i = cards.Count - 1; i >= 0; i--) 
        {
            // Get index
            int randIndex = random.Next(0, i + 1);

            // Swap elements
            Card tempCard = cards[i];
            cards[i] = cards[randIndex];
            cards[randIndex] = tempCard;
        }
    }
}
