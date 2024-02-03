using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 
/// CLASS
///     Controller_Card - class to control the decks of cards used by Community Chest and Chance.
///     board spaces. 
///     
/// DESCRIPTION
///     This class makes the card spaces on the board more simple, by only having one 
///     instance, and containing the decks of cards used by the game. This class creates
///     the decks, shuffles them, and allows the card spaces to pick up cards from the decks.
/// 
/// </summary>
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

    // ======================================== Private Data Members ======================================= //
    List<Card> m_chanceCardDeck = new List<Card>();
    List<Card> m_communityChestCardDeck = new List<Card>();


    // ======================================== Constructor ================================================ //

    // Constructor creates the decks and shuffles them both
    public Controller_Card()
    {
        InitializeCardLists();
        ShuffleCardDeck(m_chanceCardDeck);
        ShuffleCardDeck(m_communityChestCardDeck);
    }

    // ======================================== Public Methods ============================================= //

    // Returns a card from the Chance deck
    public Card TakeChanceCard()
    {
        // Take the card, move it to back of list
        Card retCard = m_chanceCardDeck[0];
        m_chanceCardDeck.RemoveAt(0);
        m_chanceCardDeck.Add(retCard);
        return retCard;
    }

    // Returns a card from the Community Chest deck
    public Card TakeCommunityChestCard()
    {
        // Take the card, move it to back of list
        Card retCard = m_communityChestCardDeck[0];
        m_communityChestCardDeck.RemoveAt(0);
        m_communityChestCardDeck.Add(retCard);
        return retCard;
    }

    // ======================================== Private Methods ============================================ //

    /// <summary>
    /// 
    /// NAME
    ///     InitializeCardLists - creates the decks of cards.
    ///     
    /// DESCRIPTION
    ///     Reads in data from a text file with all the data about cards, creates cards 
    ///     according to their ation type. 
    /// 
    /// </summary>
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
                m_chanceCardDeck.Add(card);
            
            else
                m_communityChestCardDeck.Add(card);
        }
    }
    /* void InitializeCardLists() */

    /// <summary>
    /// 
    /// NAME
    ///     CastActionString - casts string into actual Action type.
    /// 
    /// SYSNOPSIS
    ///      Actions CastActionString(string a_action); 
    ///         a_action        --> string form of the action.
    ///     
    /// DESCRIPTION
    ///     Casts a string version of the action from text file into
    ///     the actual enum type.
    /// 
    /// </summary>
    Actions CastActionString(string a_action)
    {
        switch(a_action)
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
                throw new Exception("Card data type not found, type: " + a_action);
        }
    }
    /* Actions CastActionString(string a_action) */

    /// <summary>
    /// 
    /// NAME
    ///     ShuffleCardDeck - shuffles the passed in list of cards.
    /// 
    /// SYSNOPSIS
    ///      void ShuffleCardDeck(List<Card> a_cards);
    ///         a_cards         --> deck of cards to be shuffled.
    ///     
    /// DESCRIPTION
    ///     Shuffles the deck of community chest and chance cards 
    ///     by swapping random indexes of the lists with each other.
    /// 
    /// </summary>// Shuffles a deck
    void ShuffleCardDeck(List<Card> a_cards)
    {
        System.Random random = new System.Random();

        // Loop through each card, swap with a random index
        for (int i = a_cards.Count - 1; i >= 0; i--) 
        {
            // Get index
            int randIndex = random.Next(0, i + 1);

            // Swap elements
            Card tempCard = a_cards[i];
            a_cards[i] = a_cards[randIndex];
            a_cards[randIndex] = tempCard;
        }
    }
    /* void ShuffleCardDeck(List<Card> cards) */
}
