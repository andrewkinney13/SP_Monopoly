using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 
/// CLASS
///     Board
/// 
/// SYSNOPSIS
///     Board - Handles all programatic game logic
///     
/// DESCRIPTION
///     This class implements the majority of the scripting required to enforce
///     Monopoly's rules, and serves as the highest-level class for the "Model"
///     of this game.
/// 
/// </summary>
public class Board
{
    // ======================================== Public Data Members ======================================== //

    // Types of actions a space can require a player to do
    public enum Actions
    {
        EndTurn,
        DetermineOrder,
        RollDice,
        LandedOn_UnownedProperty,
        LandedOn_OwnedColorProperty,
        LandedOn_OwnedRailroad,
        LandedOn_OwnedUtility,
        LandedOn_ChanceOrCommunityChest,
        LandedOn_VisitingJail,
        LandedOn_FreeParking,
        LandedOn_GoToJail,
        LandedOn_Tax,
        LandedOn_Go,
        LandedOn_MortgagedProperty,
        DetermineUtilityCost,
        LandedOn_JailedOwnerProperty,
        ERROR
    }

    // ======================================== Private Data Members ============================= //
    Controller_Card m_cardController = new Controller_Card();
    List<Space> m_spaces = new List<Space>();
    List<Player> m_players = new List<Player>();
    const int MAX_SPACE = 39; 
    const int MAX_PLAYERS = 6;
    int m_turnNum;

    // ======================================== Constructor ================================================ //
    public Board() { }

    // ======================================== Properties ================================================= //

    // Returns a player object reference who has the turn
    public Player CurrentPlayer { get { return m_players[m_turnNum]; } }

    // Returns the amount of players in the game
    public int PlayerCount { get { return m_players.Count; } }

    /// <summary>
    /// 
    /// NAME
    ///     GetSpace - accessing space reference via index.
    /// 
    /// SYSNOPSIS
    ///     public Space GetSpace(int a_index);
    ///         a_index         --> what index to access the space from.
    ///     
    /// RETURNS
    ///     Returns a space reference at particular index.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if index is out of bounds.
    /// 
    /// </summary>
    public Space GetSpace(int a_index)
    {
        // Check bounds
        if (a_index < 0 || a_index > MAX_SPACE)
            throw new ArgumentException("Space index out of range");
        
        return m_spaces[a_index];
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPlayer - accessing player reference via index.
    /// 
    /// SYSNOPSIS
    ///     public Player GetPlayer(int a_index);
    ///         a_index         --> what index to access the player from.
    /// 
    /// RETURNS
    ///     Returns a player object reference.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if index is out of bounds.
    /// 
    /// </summary>
    public Player GetPlayer(int a_index)
    {
        // Check bounds
        if (a_index < 0 || a_index > m_players.Count - 1)
            throw new ArgumentException("Player index out of range");

        return m_players[a_index];
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPropertyHouses - access house property of a color property
    ///                         via index.
    /// 
    /// SYSNOPSIS
    ///     public int GetPropertyHouses(int a_index);
    ///         a_index         --> what index to access the property from, who's house 
    ///                             amount we're looking.
    /// RETURNS
    ///     Returns how many houses are on a given color property.
    ///     
    /// EXCEPTION
    ///     Will throw an exception if the index does not point at a Color Property
    ///     type space.
    /// 
    /// </summary>
    public int GetPropertyHouses(int a_index)
    {
        // Check type
        if (!(m_spaces[a_index] is ColorProperty))
        {
            throw new Exception("Trying to find number of houses of a property which cannot " +
                "have houses on it! (non- ColorProperty type");
        }

        ColorProperty property = (ColorProperty)m_spaces[a_index];
        return property.Houses;
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPlayerIconName - accessing player icon name via index.
    /// 
    /// SYSNOPSIS
    ///     public string GetPlayerIconName(int a_index);
    ///         a_index         --> what player index to access the icon
    ///                             name from.
    ///                             
    /// RETURNS
    ///     Returns name of a player's icon.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if the index is out of bounds.
    /// 
    /// </summary>
    public string GetPlayerIconName(int a_index)
    {
        // Check if it's in bounds
        if ((a_index < 0) || a_index > PlayerCount)
            throw new Exception("Player num out of bounds!");
        
        return m_players[a_index].Icon;
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPlayerByName - accessing player by name.
    /// 
    /// SYSNOPSIS
    ///     public string GetPlayerByName(string a_name);
    ///         a_name          --> name of the player.
    /// 
    /// RETURNS
    ///     Returns player object reference with specified name.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if the player reference 
    ///     associated with the name is never found.
    /// 
    /// </summary>
    public Player GetPlayerByName(string a_name)
    {
        // Seach for player match
        foreach (Player player in m_players)
        {
            if (player.Name == a_name)
            {
                return player;
            }
        }

        // Player not found
        throw new Exception("Player: " + a_name + " not found searching by name...");
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPropertyByName - accessing property by name.
    /// 
    /// SYSNOPSIS
    ///     public string GetPropertyByName(string a_name);
    ///         a_name          --> name of the property.
    ///                             
    /// RETURNS
    ///     Returns property object with specified name.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if the property reference 
    ///     associated with the name is never found.
    /// 
    /// </summary>
    public Property GetPropertyByName(string a_name)
    {
        // Seeach for space match
        foreach (Space space in m_spaces)
        {
            // Only check property spaces
            if (space is Property)
            {
                Property property = (Property)space;
                if (property.Name == a_name)
                {
                    return property;
                }
            }
        }

        // Property never found
        throw new Exception("Property: " + a_name + " not found searching by name...");
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetPlayerPropertiesAndCardsStrings - access list of players' properties and cards.
    /// 
    /// SYSNOPSIS
    ///     public List<string> GetPlayerPropertiesAndCardsStrings(Player a_player);
    ///         a_player        --> object reference of a player.
    ///        
    /// DESCRIPTION
    ///     Iterates through all of the player's owned properties, and cards they have, 
    ///     to compile a comprehensive list of everything elligible to trade. 
    /// 
    /// RETURNS
    ///     Returns list of names of the properties and Get out of Jail cards
    ///     a player owns.
    /// 
    /// </summary>
    public List<string> GetPlayerElligibleTradeStrings(Player a_player)
    {
        // Iterate over all owned properties
        List<string> items = new List<string>();
        foreach (Property property in a_player.Properties)
        {
            // Check if color property with houses 
            if (property is ColorProperty)
            {
                // Skip any properties with houses
                ColorProperty colorProperty = (ColorProperty)property;
                if (colorProperty.Houses != 0)
                    continue;
            }

            items.Add(property.Name);
        }

        // Iterate over cards
        for (int i = 0; i < a_player.CommunityChestJailCards; i++)
        {
            items.Add("Community Chest Jail Card");
        }
        for (int i = 0; i < a_player.ChanceJailCards; i++)
        {
            items.Add("Chance Jail Card");
        }

        return items;
    }
    /* public List<string> GetPlayerElligibleTradeStrings(Player a_player) */

    /// <summary>
    /// 
    /// NAME
    ///     GetOrderDeterminedMessage - access list of players' properties and cards.
    ///        
    /// DESCRIPTION
    ///     Iterates through all of the player's owned properties, and cards they have, 
    ///     to compile a comprehensive list of everything.
    /// 
    /// RETURNS
    ///     Returns a message about the newly created player order.
    /// 
    /// </summary>
    public string GetOrderDeterminedMessage()
    {
        // Iterate through the players to update their properties and create output message
        string message = "";
        int playerNum = 0;
        foreach (Player player in m_players)
        {
            // Append output for popup
            message += (playerNum + 1) + ": " + player.Name + ", rolled " + player.OrderDeterminingDiceResult + "\n";

            // Append what player we're on
            playerNum++;
        }

        return message;
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetLandedOnUnownedPropertyTitle - creates title for landing on unowned property.
    ///        
    /// DESCRIPTION
    ///     Obtains the space the current player is on. Uses its properties
    ///     to create a message prompting player to buy it or not.
    /// 
    /// RETURNS
    ///     Returns a message asking about buying the property. 
    ///     
    /// EXCEPTION
    ///     Throws exception if trying to access this property from a non-Property type space.
    /// 
    /// </summary>
    public string GetLandedOnUnownedPropertyTitle()
    {
        // Cast to property type and use derived property
        if (m_spaces[CurrentPlayer.CurrentSpace] is Property)
        {
            Property purchaseProperty = (Property)m_spaces[CurrentPlayer.CurrentSpace];
            return "Would you like to buy " + purchaseProperty.Name + " for $" + 
                purchaseProperty.PurchasePrice + "?";
        }

        throw new Exception("Trying to obtain unowned property title " +
            "from non-Property type space: " + m_spaces[CurrentPlayer.CurrentSpace].Name);
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetLandedOnOwnedPropertyTitle - creates title for landing on owned property.
    ///        
    /// DESCRIPTION
    ///     Method creates a prompt for player who landed on some other player's owned 
    ///     property. This title is based on factors such as the property type, 
    ///     other property the owner owns, and / or how many houses / houses 
    ///     are on the property.
    /// 
    /// RETURNS
    ///     Returns a message about the owned property a player landed on.
    ///     (A property this player does not own).
    ///     
    /// EXCEPTION
    ///     Exception thrown if the space type a player is on is not of a type
    ///     that can be owned by a player.
    /// 
    /// </summary>
    public string GetLandedOnOwnedPropertyTitle()
    {
        // Initialize string with basic attributes
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];
        string retString = "You landed on " + property.Name + ", owned by " + property.Owner.Name + ". ";

        // Space is a color property, with houses influencing price
        if (property is ColorProperty)
        {
            ColorProperty colorProperty = (ColorProperty)property;
            retString += "It has ";
            if (colorProperty.Houses == 0)
            {
                retString += "0 houses";
                if (colorProperty.ColorSetOwned)
                    retString += " but, color set is owned (rent doubled)";
                
            }
            else if (colorProperty.Houses >= 1 && colorProperty.Houses <= 4)
                retString += colorProperty.Houses + " house(s)";

            else
                retString += "1 hotel";
            
            return retString;
        }

        // Space is a railroad, with allied railroads influencing price
        else if (property is Railroad)
        {
            Railroad railroad = (Railroad)property;
            retString += railroad.Owner.Name + " owns " + railroad.AlliedRailroads + " other railroad(s)";
            return retString;
        }

        // Space is a utility, with dice rolling influencing price
        else if (property is Utility)
        {
            Utility utility = (Utility)property;
            retString += "You just rolled a " + utility.CurrentDiceRoll + "," +
                " and " + utility.Owner.Name;
            if (utility.IsAllied)
                retString += " owns both utilities";
            
            else
                retString += " owns only one utility";
            
            return retString;
        }

        // Invalid space type for this method
        else
        {
            throw new Exception("Attempting to get landed on title for an owned property, " +
                "from a non-owned property type space...");
        }
    }
    /* public string GetLandedOnOwnedPropertyTitle() */

    /// <summary>
    /// 
    /// NAME
    ///     GetBankruptMessage - creates title for going bankrupt.
    ///        
    /// DESCRIPTION
    ///     When the current player goes bankrupt, this method determines
    ///     by what means, and returns a string with the explanaition.
    ///     How player went bankrupt is determined by what type of space 
    ///     they're currently on.
    /// 
    /// RETURNS
    ///     Returns string message explaining how player went bankrupt.
    /// 
    /// </summary>
    public string GetBankruptMessage()
    {
        // Obtian current space reference
        Space currentSpace = m_spaces[CurrentPlayer.CurrentSpace];

        // At the hands of a player
        if (currentSpace is Property)
        {
            // Cast space to property
            Property property = (Property)currentSpace;
            return "You were bankrupted by " + property.Owner.Name + "!";
        }

        // At the hands of the bank
        return "You were bankrupted by the Bank!";
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetLandedOnOwnedPropertyRent - creates title for landing on unowned property.
    ///        
    /// DESCRIPTION
    ///     Obtains most inherited RentPrice property of the property the
    ///     current player landed on.
    /// 
    /// RETURNS
    ///     Returns rent price for a property the player landed on.
    /// 
    /// EXCEPTION
    ///     Throws exception if the space being accessed is not of Property type.
    /// 
    /// </summary>
    public int GetLandedOnOwnedPropertyRent()
    {
        // Cast to property type and use derived property
        if (m_spaces[CurrentPlayer.CurrentSpace] is Property)
        {
            Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];
            return property.RentPrice;
        }

        throw new Exception("Trying to obtain rent price from an unownable " +
            "space: " + m_spaces[CurrentPlayer.CurrentSpace].Name);
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetLandedOnTaxCost - returns how much tax is for a Tax space.
    ///     
    /// DESCRIPTION
    ///     Based on the tax space the current player is on, returns the 
    ///     tax amount for that space.
    ///
    /// RETURNS
    ///     Returns tax amount for the current player's space, when they're on a
    ///     Tax type space.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if the space the current player is on is not 
    ///     of Tax type.
    /// 
    /// </summary>
    public int GetLandedOnTaxCost()
    {
        // Cast to tax and return property
        if (m_spaces[CurrentPlayer.CurrentSpace] is Tax)
        {
            Tax taxSpace = (Tax)m_spaces[CurrentPlayer.CurrentSpace];
            return taxSpace.TaxCost;
        }
        throw new Exception("Attempting to access tax cost of a non-Tax type space: " +
            m_spaces[CurrentPlayer.CurrentSpace].Name);
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetRepairCost - returns cost for how much repairing a property would be.
    /// 
    /// SYSNOPSIS
    ///     public string GetRepairCost(int a_houseCost, int a_hotelCost);
    ///         a_houseCost          --> how much each house costs to repair
    ///         a_hotelCost          --> how much each hotel costs to repair
    /// DESCRIPTION
    ///     This calculates the cost of "repairing" a player's ColorProperty developments,
    ///     as defined by the respective cards from the Community Chest or Chance piles.
    ///     Does this by finding every ColorProperty a player owns, and sums up a cost
    ///     total based on how much each of those properties is developed.
    ///
    /// RETURNS
    ///     Returns cost of repairing this property.
    /// 
    /// EXCEPTION
    ///     Will throw an exception if the space the current player is on is not 
    ///     of ColorProperty type.
    /// 
    /// </summary>
    public int GetRepairCost(int a_houseCost, int a_hotelCost)
    {
        // Go through every property owned by current player
        int sumCost = 0;
        foreach (Property property in CurrentPlayer.Properties)
        {
            // Skip any railroads or utilities
            if (!(property is ColorProperty))
                continue;

            // Cast to color property
            ColorProperty colorProperty = (ColorProperty)property;

            // Add to sum for houses or hotels
            if (colorProperty.Houses == 5)
            {
                sumCost += a_hotelCost;
            }
            else
            {
                sumCost += a_houseCost * colorProperty.Houses;
            }
        }
        return sumCost;
    }
    /* public int GetRepairCost(int a_houseCost, int a_hotelCost) */

    // ======================================== Public Methods ============================================= //

    // Initializes the spaces and the players
    public void InitializeBoard()
    {
        InitializeSpaces();
        InitializePlayers();
    }

    /// <summary>
    /// 
    /// NAME
    ///     UtilityCostDetermined - updates Utility flag.
    /// 
    /// SYSNOPSIS
    ///     public void UtilityCostDetermined(int a_diceRoll);
    ///         a_diceRoll          --> value of the dice roll that the player made.
    ///         
    /// DESCRIPTION
    ///     Updates Utility flag to indicate that the player who landed on a Utility has rolled
    ///     the dice to determine how much their rent will cost. It also saves what the 
    ///     dice roll was, to use later to calculate this cost based on the Utility's
    ///     multiplier.
    /// 
    /// </summary>
    public void UtilityCostDetermined(int a_diceRoll)
    {
        Utility utility = (Utility)m_spaces[CurrentPlayer.CurrentSpace];
        utility.DiceRolled = true;
        utility.CurrentDiceRoll = a_diceRoll;
    }

    /// <summary>
    /// 
    /// NAME
    ///     DetermineAction - returns what the current player should do this turn.
    ///         
    /// DESCRIPTION
    ///     Considering various attributes of the board, current player, and other players,
    ///     this method returns what Actions type the current player needs to complete at this 
    ///     time in their turn. Factors include if the player's turn has been initialized,
    ///     if they rolled doubles, are in jail, on an owned property, etc. This method considers
    ///     various weird combinations of situations, such as if a player rolled doubles, but landed
    ///     on Go to Jail (making them end their turn early, and not roll dice twice). There
    ///     are many such cases of weird game situations that this method considers.
    ///     
    /// RETURNS
    ///     Returns the Actions type that is associated with the current player.
    /// 
    /// </summary>
    public Actions DetermineAction()
    {
        // Roll dice to determine turn
        if (!CurrentPlayer.TurnInitialized)
            return Actions.DetermineOrder;
        
        // Roll dice to move
        if (!CurrentPlayer.RolledDice && !CurrentPlayer.InJail)
            return Actions.RollDice;

        // Landed on a space and hasn't acted yet
        if (!CurrentPlayer.SpaceActionCompleted)
        {
            // Check if the property is mortgaged, or player on own property
            if (CurrentPlayer.OnOwnedProperty)
            {
                // If rolled doubles, let them roll again
                if (CurrentPlayer.RolledDoubles)
                {
                    CurrentPlayer.SpaceActionCompleted = false;
                    return Actions.RollDice;
                }

                // If didn't roll doubles, end turn
                if (!CurrentPlayer.RolledDoubles)
                    return Actions.EndTurn;
            }

            // Otherwise do the space activity, if it's an end turn activity (free parking, just visiting) and 
            // player rolled doubles, don't end turn
            Actions spaceAction = m_spaces[CurrentPlayer.CurrentSpace].ActionType;
            if (!(spaceAction == Actions.EndTurn && CurrentPlayer.RolledDoubles))
                return m_spaces[CurrentPlayer.CurrentSpace].ActionType;
        }

        // If they rolled doubles, let them roll again and have another turn (unless in jail)
        if (CurrentPlayer.RolledDoubles && !CurrentPlayer.InJail)
        {
            CurrentPlayer.SpaceActionCompleted = false;
            return Actions.RollDice;
        }

        // Nothing to do, end turn
        else
            return Actions.EndTurn;
    }
    /* public Actions DetermineAction() */

    /// <summary>
    /// 
    /// NAME
    ///     UpdateTurn - updates which player has the current turn.
    ///         
    /// DESCRIPTION
    ///     Resets the current player's flags, as well as the flags of any properties
    ///     who have indicators that need to be reset every turn. Lets the "next"
    ///     player in the list have the current turn. Ignores bankrupt players when 
    ///     attempting to find the "next" player.
    /// 
    /// </summary>
    public void UpdateTurn()
    {
        // Reset the current player's attributes
        ResetCurrentPlayer();

        // Reset utility bools 
        ResetUtilities();

        // Update whose turn is is 
        int searchCount = 0;
        while (true)
        {
            // Find next player
            if (m_turnNum < m_players.Count - 1)
                m_turnNum++;
            else
                m_turnNum = 0;

            // Skip if bankrupt
            if (!CurrentPlayer.Bankrupt)
                break;
            
            // Prevent infinite loop
            if (searchCount > MAX_PLAYERS)
                throw new Exception("All players bankrupted, yet trying to access next turn...\n");
        }
    }
    /* public void UpdateTurn() */

    // Resets the attributes of a player at the end of their turn
    public void ResetCurrentPlayer()
    {
        CurrentPlayer.DiceRollResult = 0;
        CurrentPlayer.RolledDice = false;
        CurrentPlayer.RolledDoubles = false;
        CurrentPlayer.TurnCompleted = false;
        CurrentPlayer.SpaceActionCompleted = false;
    }

    // Current player rolled the dice to determine their order
    public void OrderDetermined(int a_diceResult)
    {
        CurrentPlayer.OrderDeterminingDiceResult = a_diceResult;
        CurrentPlayer.TurnInitialized = true;
    }

    // Checks every player to check if their order has been determined
    public bool AllPlayersInitialized()
    {
        // Check each player to see if they were not initialized
        foreach (Player player in m_players)
        {
            if (!player.TurnInitialized)
                return false;
        }

        // All players initialized
        return true;
    }

    /// <summary>
    /// 
    /// NAME
    ///     InitializePlayerOrder - initialized the turn data for players.
    ///         
    /// DESCRIPTION
    ///     Each player is sorted according to defined sorting function. 
    ///     New player number is assigned based on new index within 
    ///     the sorted player list.
    /// 
    /// </summary>
    public void InitializePlayerOrder()
    {
        // Sort the player
        m_players.Sort(SortPlayers);

        // Initialize the new values
        int playerNum = 0;
        foreach (Player player in m_players)
        {
            // New player num
            player.PlayerNum = playerNum;
            playerNum++;
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     GameOver - checks if game has ended.
    ///         
    /// DESCRIPTION
    ///     Checks each player to see if all but one are bankrupt,
    ///     this is the terminal state of the game.
    ///     
    /// RETURNS
    ///     Bool indicating whether the game is over or not.
    /// 
    /// </summary>
    public bool GameOver()
    {
        // Count amount of bankrupt players
        int bankruptPlayers = 0;
        foreach (Player player in m_players)
        {
            if (player.Bankrupt)
                bankruptPlayers++;
        }

        // Check if all but one are bankrupt
        if (bankruptPlayers == m_players.Count - 1)
            return true;
        
        return false;
    }

    /// <summary>
    /// 
    /// NAME
    ///     DiceRolled - updates board after a dice roll.
    ///     
    /// SYSNOPSIS
    ///     public void DiceRolled(int a_diceResult, bool a_wereDoubles);
    ///         a_diceResult        --> value of the dice.
    ///         a_wereDoubles       --> if the faces matched.
    ///                             
    /// DESCRIPTION
    ///     Updates a players position on the board according to the 
    ///     value of the dice roll they had. Updates player's flag 
    ///     indicating whether they rolled doubles. Updates player's
    ///     flag indicating whether or not they rolled the dice this
    ///     turn. 
    /// 
    /// </summary>
    public void DiceRolled(int a_diceResult, bool a_wereDoubles)
    {
        // Update the current space of player
        int destinationSpace = a_diceResult + CurrentPlayer.CurrentSpace;
        if (destinationSpace >= 40)
        {
            destinationSpace -= 40;
        }
        CurrentPlayer.CurrentSpace = destinationSpace;

        // Update current players "rolled doubles"
        CurrentPlayer.RolledDoubles = a_wereDoubles;

        // Update the players rolled dice boolean
        CurrentPlayer.RolledDice = true;
    }
    /* public void DiceRolled(int a_diceResult, bool a_wereDoubles) */

    /// <summary>
    /// 
    /// NAME
    ///     PickupCard - player picking up a card.
    ///     
    /// DESCRIPTION
    ///     Player is picking up a card from Community Chest or
    ///     Chance space.
    /// 
    /// RETURNS
    ///     Returns a Card object representing the card the player picked up.
    /// 
    /// EXCEPTION
    ///     Throws exception if the space the player is trying to pickup 
    ///     the card from is not of Card type.
    /// 
    /// </summary>
    public Card PickupCard()
    {
        // Cast to CardSpace and return a card
        if (m_spaces[CurrentPlayer.CurrentSpace] is CardSpace)
        {
            // Return card for the player
            CardSpace cardSpace = (CardSpace)m_spaces[CurrentPlayer.CurrentSpace];
            return cardSpace.TakeCard();
        }

        throw new Exception("Trying to pickup a card from a non-Card type space: " +
            m_spaces[CurrentPlayer.CurrentSpace].Name);
    }

    /// <summary>
    /// 
    /// NAME
    ///     GoingBankrupt - player going bankrupt, out of money.
    ///     
    /// DESCRIPTION
    ///     Player is going bankrupt. This method determines where the last of the 
    ///     player's money went, either bank or player. If another player bankrupted
    ///     them, the bankrpted player relinquishes all their properties to the person 
    ///     who did the bankrupting.
    ///    
    /// </summary>
    public void GoingBankrupt()
    {
        // Bankrupt from property
        Space currentSpace = m_spaces[CurrentPlayer.CurrentSpace];
        if (currentSpace is Property)
        {
            // Give owner all of the properties of bankrupt player
            Property currentProperty = (Property)currentSpace;
            foreach (Property property in CurrentPlayer.Properties)
            {
                property.Owner = currentProperty.Owner;
                property.IsMortgaged = false;
                property.IsPurchased = false;
            }
            currentProperty.Owner.Properties.AddRange(CurrentPlayer.Properties);
        }

        // Bankrupt from bank
        else
        {
            // Return all the properties to the board
            foreach (Property property in CurrentPlayer.Properties)
            {
                property.Owner = null;
                property.IsMortgaged = false;
                property.IsPurchased = false;
            }
        }

        // Remove all properties from player
        CurrentPlayer.Properties.Clear();
    }
    /* public void GoingBankrupt() */

    // Player going to jail, update flags and location
    public void GoToJail()
    {
        CurrentPlayer.InJail = true;
        CurrentPlayer.CurrentSpace = 10;
        CurrentPlayer.SpaceActionCompleted = true;
    }

    // Player paying to get out of jail, remove cash and update flag
    public void GetOutOfJailPay()
    {
        CurrentPlayer.Cash -= 75;
        CurrentPlayer.InJail = false;
    }

    // Player uses card to get out of jail, remove card and update flag
    public void GetOutOfJailWithCard()
    {
        // Determine which card type to use
        if (CurrentPlayer.CommunityChestJailCards >= 0)
            CurrentPlayer.CommunityChestJailCards--;
        else
            CurrentPlayer.ChanceJailCards--;

        CurrentPlayer.InJail = false;
    }

    /// <summary>
    /// 
    /// NAME
    ///     PurchaseProperty - player purchases a property.
    ///     
    /// DESCRIPTION
    ///     Player landed on an unowned property that they could afford, and 
    ///     chose to buy it. Their cash is updated, the property is added to their owned
    ///     properties list, and the property's attributes are updated to reflect 
    ///     that it is owned.
    /// 
    /// EXCEPTION
    ///     Exception can be thrown for many reasons, none of which are expected
    ///     to occur unless a bug in previous code allows such a situation. 
    ///    
    /// </summary>
    public void PurchaseProperty()
    {
        // Check space type validity
        if (!(m_spaces[CurrentPlayer.CurrentSpace] is Property))
            throw new Exception("Trying to purchase a non-property type space: " +
                m_spaces[CurrentPlayer.CurrentSpace]);  

        // Obtain the property
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];

        // Check if player can afford it (should be able to)
        if (CurrentPlayer.Cash < property.PurchasePrice)
        {
            throw new Exception("Player allowed to purchase a property they can't afford: " +
                property.Name + " Cost: $" + property.PurchasePrice + " Player cash: $" +
                CurrentPlayer.Cash);
        }

        // Make the purchase
        CurrentPlayer.Cash -= property.PurchasePrice;
        CurrentPlayer.Properties.Add(property);
        property.Owner = CurrentPlayer;
        property.IsPurchased = true;
        CurrentPlayer.Properties.Sort();
    }
    /* public void PurchaseProperty() */

    /// <summary>
    /// 
    /// NAME
    ///     PayRent - player is paying rent on a property they landed on.
    ///     
    /// DESCRIPTION
    ///     Player landed on a property they do not own, and is paying the
    ///     rent on the property here. Paid money goes to owner of the property.
    /// 
    /// EXCEPTION
    ///     If player is trying to pay rent on a property which should not 
    ///     need to have rent paid for landing on it (unowned, mortgaged, etc).
    ///    
    /// </summary>
    public void PayRent()
    {
        // Check type
        if (!(m_spaces[CurrentPlayer.CurrentSpace] is Property))
            throw new Exception("Attempting to pay rent on non-Property type: " +
               m_spaces[CurrentPlayer.CurrentSpace].Name);

        // Cast space
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];

        // Check if it's owned
        if (!property.IsPurchased)
            throw new Exception("Attempting to pay rent on an unowned property");

        // Check that it's not mortgaged
        if (property.IsMortgaged)
            throw new Exception("Attempting to pay rent on a mortgaged property");

        // Check that owner isn't in jail 
        if (property.Owner.InJail)
            throw new Exception("Attempting to pay rent on property whose owner " +
                "is incarcerated");

        // Take cash from player who landed and give it to owner
        CurrentPlayer.Cash -= property.RentPrice;
        property.Owner.Cash += property.RentPrice;
    }
    /* public void PayRent() */

    /// <summary>
    /// 
    /// NAME
    ///     PayTax - player is paying taxes for a tax space.
    ///     
    /// DESCRIPTION
    ///     Player landed on a Tax space, and is now paying
    ///     taxes.
    /// 
    /// EXCEPTION
    ///     If player is trying to pay taxes whilst not being on a tax space.
    ///    
    /// </summary>
    public void PayTax()
    {
        // Check type
        if (!(m_spaces[CurrentPlayer.CurrentSpace] is Tax))
        {
            throw new Exception("Attempting to pay tax on non-Tax space: " +
            m_spaces[CurrentPlayer.CurrentSpace].Name);
        }

        // Cast space and subtract cash
        Tax taxSpace = (Tax)m_spaces[CurrentPlayer.CurrentSpace];
        CurrentPlayer.Cash -= taxSpace.TaxCost;
    }

    /// <summary>
    /// 
    /// NAME
    ///     BuyHouse - player is buying a house for a property.
    ///     
    /// SYNOPSIS
    ///     public void BuyHouse(int a_propertyIndex)
    ///         a_propertyIndex         --> space index of the property that
    ///                                     they're buying a house for.
    ///     
    /// DESCRIPTION
    ///     Player is buying a house for their property.
    /// 
    /// EXCEPTION
    ///     House is unavailible to purchase.
    ///    
    /// </summary>
    public void BuyHouse(int a_propertyIndex)
    {
        // Check space type
        if (!(GetSpace(a_propertyIndex) is ColorProperty))
            throw new Exception("Attempting to buy a house on a non-ColorProperty type space: " +
                GetSpace(a_propertyIndex).Name);
           
        // Obtain property
        ColorProperty property = (ColorProperty)GetSpace(a_propertyIndex);

        // Check buying availible
        if (!HouseAvailible(CurrentPlayer, property) && !HotelAvailible(CurrentPlayer, property))
            throw new Exception("Player attempting to buy a house when they shouldn't be able to");

        // Make the purchase
        CurrentPlayer.Cash -= property.HouseCost;
        property.Houses++;
    }

    /// <summary>
    /// 
    /// NAME
    ///     SellHouse - player is selling house on a property.
    ///     
    /// SYNOPSIS
    ///     public void SellHouse(int a_propertyIndex)
    ///         a_propertyIndex         --> space index of the property that
    ///                                     they're buying a house for.
    ///     
    /// DESCRIPTION
    ///     Player is selling a house on their property.
    /// 
    /// EXCEPTION
    ///     Invalid space or selling unavailible.
    ///    
    /// </summary>
    public void SellHouse(int a_propertyIndex)
    {
        // Check space type
        if (!(GetSpace(a_propertyIndex) is ColorProperty))
            throw new Exception("Attempting to sell a house on a non-ColorProperty type space: " +
                GetSpace(a_propertyIndex).Name);

        // Obtain property
        ColorProperty property = (ColorProperty)GetSpace(a_propertyIndex);

        // Check selling availible
        if (!SellHouseAvailible(property))
            throw new Exception("Player attempting to sell a house when they shouldn't be able to");

        // Make the sale 
        CurrentPlayer.Cash += property.HouseCost / 2;
        property.Houses--;
    }

    /// <summary>
    /// 
    /// NAME
    ///     MortgageProperty - player is mortgaging a property.
    ///     
    /// SYNOPSIS
    ///     public void MortgageProperty(int a_propertyIndex)
    ///         a_propertyIndex         --> space index of the property that
    ///                                     they're mortgaging.
    ///     
    /// DESCRIPTION
    ///     Player is selling a house on their property.
    /// 
    /// EXCEPTION
    ///     Invalid mortgage attemted.
    ///    
    /// </summary>
    public void MortgageProperty(int a_propertyIndex)
    {
        // Check type
        if (!(GetSpace(a_propertyIndex) is Property))
            throw new Exception("Attempting to mortgage non-Property type space: " +
                GetSpace(a_propertyIndex).Name);

        // Obtain property
        Property property = (Property)GetSpace(a_propertyIndex);

        // Check that mortgaging is availible
        if (property.IsMortgaged)
            throw new Exception("Attempting to mortgage property which is already mortgaged: " +
                property.Name);

        // Add mortgage value to player's cash
        CurrentPlayer.Cash += property.MortgageValue;

        // Mark it as mortgaged
        property.IsMortgaged = true;
    }

    /// <summary>
    /// 
    /// NAME
    ///     UnmortgageProperty - player is mortgaging a property.
    ///     
    /// SYNOPSIS
    ///     public void UnmortgageProperty(int a_propertyIndex)
    ///         a_propertyIndex         --> space index of the property that
    ///                                     they're mortgaging.
    ///     
    /// DESCRIPTION
    ///     Player is selling a house on their property.
    /// 
    /// EXCEPTION
    ///     Invalid mortgage attemted.
    ///    
    /// </summary>
    public void UnmortgageProperty(int a_propertyIndex)
    {
        // Check type
        if (!(GetSpace(a_propertyIndex) is Property))
            throw new Exception("Attempting to unmortgage non-Property type space: " +
                GetSpace(a_propertyIndex).Name);

        // Obtain the property 
        Property property = (Property)GetSpace(a_propertyIndex);

        // Check unmortgage availible
        if (!UnmortgageAvailible(CurrentPlayer, (ColorProperty)property))
            throw new Exception("Attempting to ");

        // Add mortgage value to player's cash
        CurrentPlayer.Cash -= property.MortgageValue;

        // Mark it as not mortgaged
        property.IsMortgaged = false;
    }

    /// <summary>
    /// 
    /// NAME
    ///     HouseAvailible - is a house availible for a player's property.
    ///     
    /// SYNOPSIS
    ///     public bool HouseAvailible(Player a_player, ColorProperty a_property)
    ///         a_player                --> player trying to buy a house.
    ///         a_property              --> property getting house on it.
    ///     
    /// DESCRIPTION
    ///     Checks whether or not a player can buy a house on a property they own.
    ///     This method enforces the rule that houses are bought on properties one
    ///     at a time, and not all at once. 
    ///     
    /// RETURNS
    ///     True if house is availible, false if not.
    ///    
    /// </summary>
    public bool HouseAvailible(Player a_player, ColorProperty a_property)
    {
        // Total number of houses full, property is mortgaged,
        // not enough cash, or full color set unowned
        if (a_property.Houses >= 4 || a_property.HouseCost > a_player.Cash 
            || a_property.IsMortgaged || !a_property.ColorSetOwned)
        {
            return false;
        }

        // Total house number would exceed other house minimum by > 1
        int houseMin = 4;
        List<ColorProperty> colorSet = GetColorSet(a_property);

        // Find smallest num of houses
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses < houseMin)
            {
                houseMin = colorProperty.Houses;
            }
        }

        // Check that new house total would not exeed min by 1
        if (a_property.Houses + 1 - houseMin > 1)
        {
            return false;
        }

        return true;
    }
    /* public bool HouseAvailible(Player a_player, ColorProperty a_property) */

    /// <summary>
    /// 
    /// NAME
    ///     SellHouseAvailible - is a house availible to sell.
    ///     
    /// SYNOPSIS
    ///     public bool SellHouseAvailible(ColorProperty a_property)
    ///         a_property              --> property trying to sell house.
    ///     
    /// DESCRIPTION
    ///     Checks whether or not a player can sell a house on a property they own.
    ///     This method enforces the rule that houses are sold on properties one
    ///     at a time, and not all at once. 
    ///     
    /// RETURNS
    ///     True if selling house is availible, false if not.
    ///    
    /// </summary>
    public bool SellHouseAvailible(ColorProperty a_property)
    {
        // Non-zero num of houses, not a hotel
        if (a_property.Houses <= 0 || a_property.Houses >= 5)
        {
            return false;
        }

        // Total house number would be lower than the max by > 1
        int houseMax = 0;
        List<ColorProperty> colorSet = GetColorSet(a_property);

        // Find smallest num of houses
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses > houseMax)
            {
                houseMax = colorProperty.Houses;
            }
        }

        // Check that new house total would not exeed min by 1
        if (houseMax - (a_property.Houses - 1) > 1)
        {
            return false;
        }

        return true;
    }
    /* public bool SellHouseAvailible(ColorProperty a_property) */

    /// <summary>
    /// 
    /// NAME
    ///     HotelAvailible - is a hotel availible to sell.
    ///     
    /// SYNOPSIS
    ///     public bool HotelAvailible(Player a_player, ColorProperty a_property)
    ///         a_player                --> player buying the hotel.
    ///         a_property              --> property getting hotel on it.
    ///     
    /// DESCRIPTION
    ///     Checks whether or not a player can buy a hotel on a property they own.
    ///     This method enforces the rule that hotels can only be bought when there's already 
    ///     4 houses on a property, and all other properties also have 4 houses.
    /// 
    /// RETURNS
    ///     True if hotel is availible, false if not.
    ///    
    /// </summary>
    public bool HotelAvailible(Player a_player, ColorProperty a_property)
    {
        // House value is not 4, property is mortgaged, or not enough cash
        if (a_property.Houses != 4 || a_property.HouseCost > a_player.Cash 
            || a_property.IsMortgaged)
        {
            return false;
        }

        // Color set all has at least 4 houses
        List<ColorProperty> colorSet = GetColorSet(a_property);
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses < 4)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 
    /// NAME
    ///     UnmortgageAvailible - can player unmortgage property
    ///     
    /// SYNOPSIS
    ///     public bool UnmortgageAvailible(Player a_player, ColorProperty a_property)
    ///         a_player                --> player trying to mortgage.
    ///         a_property              --> property getting unmortgaged.
    ///     
    /// DESCRIPTION
    ///     Checks if a property can be bought back (unmortgaged).
    /// 
    /// RETURNS
    ///     True if unmortgage is availible, false if not.
    ///    
    /// </summary>
    public bool UnmortgageAvailible(Player player, Property property)
    {
        // Property is mortgaged, and player can afford to buy it
        if (property.IsMortgaged && property.MortgageValue < player.Cash)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetWinner - returns who won the game.
    ///     
    /// DESCRIPTION
    ///     Determines winner and returns reference to them.
    /// 
    /// RETURNS
    ///     Player reference who won the game (last remaining player with money).
    ///    
    /// </summary>
    public Player GetWinner()
    {
        // Find non-bankrupt player
        foreach (Player player in m_players)
        {
            if (!player.Bankrupt)
            {
                return player;
            }
        }

        // No player found that's not bankrupt...
        throw new Exception("All players bankrup! There is no winner to be found...");
    }

    // ======================================== Private Methods ============================================ //

    /// <summary>
    /// 
    /// NAME
    ///     InitializeSpaces - creates spaces based on space data text file.
    ///     
    /// DESCRIPTION
    ///     Parses a text file with the data for all 40 spaces of the monopoly board. 
    ///     All spaces either are, or inherit from, the base Space class. Almost all 
    ///     spaces have unique functionaity, and therefore a unique type, so spaces
    ///     are dynamically created according to their appropriate type in a switch 
    ///     statement. All space data, and what determines their type, is in a premade
    ///     space data text file in the StreamingAssets folder of this project.
    ///    
    /// </summary>
    void InitializeSpaces()
    {
        // Obtain space data from file
        string spacesFilePath = Path.Combine(Application.streamingAssetsPath, "spaceData.txt");
        string[] lines = File.ReadAllLines(spacesFilePath);

        // Obtain space details from the data
        // Data has several columns, space name, action type, space type, and if relevent,
        // purchase price, morgage price, and land on prices for each number of houses
        int spaceNum = 0;
        foreach (string line in lines)
        {
            // Split the string
            string[] vals = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Obtain the name from first index
            string name = vals[0];

            // Translate second index from string to action
            Actions action = CastActionString(vals[1]);

            // Create the space based on it's specified type in the file
            Space currentSpace = null;
            int purchasePrice;
            switch (vals[2])
            {
                // Generic space (corners)
                case "Jail":
                case "Go":
                case "Free Parking":
                case "Just Visiting":
                    currentSpace = new Space(name, spaceNum, action, 
                        GetSpaceDescription(name));
                    break;

                // Card
                case "Card":
                    currentSpace = new CardSpace(name, spaceNum, action, m_cardController, 
                        GetSpaceDescription(name));
                    break;


                // Tax
                case "Tax":
                    int taxCost = int.Parse(vals[3]);
                    currentSpace = new Tax(name, spaceNum, action, taxCost, 
                        GetSpaceDescription(name));
                    break;

                // Utility
                case "Utility":
                    purchasePrice = int.Parse(vals[3]);
                    currentSpace = new Utility(name, spaceNum, action, purchasePrice, 
                        string.Empty);
                    break;

                // Color property
                case "ColorProperty":

                    // Obtain prices
                    purchasePrice = int.Parse(vals[3]);
                    int houseCost = int.Parse(vals[4]);
                    List<int> landOnPrices = new List<int>();
                    for (int i = 5; i < 11; i++)
                    {
                        landOnPrices.Add(int.Parse(vals[i]));
                    }

                    // Obtain color
                    string color = vals[11];

                    // Create object
                    currentSpace = new ColorProperty(name, spaceNum, action, purchasePrice, 
                        houseCost, landOnPrices, string.Empty, color);
                    break;

                // Railroad
                case "Railroad":

                    // Create object (all railroad prices are the same)
                    currentSpace = new Railroad(name, spaceNum, action, 200, 25, string.Empty);
                    break;

                // Case not found, error
                default:
                    throw new Exception("Could not determine space type, given type: " +
                        vals[2]);
            }

            // Add space to list of spaces and move to next space index
            m_spaces.Add(currentSpace);
            spaceNum++;
        }
    }
    /* void InitializeSpaces() */

    /// <summary>
    /// 
    /// NAME
    ///     InitializePlayers - creates players based on player data xml file.
    ///     
    /// DESCRIPTION
    ///     The start menu where players are created, saves the data to an 
    ///     xml file. This method reads that data to create a list of 
    ///     Player objects.
    ///    
    /// </summary>
    void InitializePlayers()
    {
        // Create player file object
        PlayerFile playerFile = new PlayerFile();

        // Create temp dicts to store players
        Dictionary<string, string> tempPlayers;

        // Obtain players from the file
        tempPlayers = playerFile.ReadPlayersFromFile();

        // Add them to the players list
        int playerNum = 0;
        foreach (string name in tempPlayers.Keys)
        {
            Player currentPlayer = new Player(name, tempPlayers[name], playerNum);
            m_players.Add(currentPlayer);
            playerNum++;
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     InitializePlayers - creates players based on player data xml file.
    ///     
    /// SYNOPSIS
    ///     List<ColorProperty> GetColorSet(ColorProperty a_property)
    ///         a_property      --> property who's color set being returned.
    ///     
    /// DESCRIPTION
    ///     Compiles a list of all the properties who have the same color
    ///     as the passed in property, returns it.
    ///     
    /// RETURNS
    ///     List of color set.
    ///    
    /// </summary>
    List<ColorProperty> GetColorSet(ColorProperty a_property)
    {
        // Get the color
        string color = a_property.Color;
        List<ColorProperty> colorSet = new List<ColorProperty>();
        foreach (Space space in m_spaces)
        {
            // Only check color properties
            if (space is ColorProperty)
            {
                // Cast and check color
                ColorProperty colorProperty = (ColorProperty)space;

                // Add if the same
                if (colorProperty.Color == color)
                {
                    colorSet.Add(colorProperty);
                }
            }
        }
        return colorSet;
    }
    /* List<ColorProperty> GetColorSet(ColorProperty a_property) */

    // Resets the two utilities,
    // flags that mark whether or not player rolled to determine price
    void ResetUtilities()
    {
        Utility electricCompany = (Utility)m_spaces[12];
        Utility waterWorks = (Utility)m_spaces[28];
        electricCompany.DiceRolled = false;
        waterWorks.DiceRolled = false;
    }

    // Sorts players based on their current space, as determined by a dice roll
    static int SortPlayers(Player player1, Player player2)
    {
        if (player1.OrderDeterminingDiceResult < player2.OrderDeterminingDiceResult)
            return 1;
        
        else if (player1.OrderDeterminingDiceResult > player2.OrderDeterminingDiceResult)
            return -1;
        
        else
            return 0;
    }

    /// <summary>
    /// 
    /// NAME
    ///     GetSpaceDescription - returns a description of generic space types.
    ///     
    /// SYNOPSIS
    ///     string GetSpaceDescription(string a_name)
    ///         a_name      --> space to get description of.
    ///     
    /// DESCRIPTION
    ///     For spaces who don't need dynamic descriptions, this method will 
    ///     return a predetermined descrption.
    ///     
    /// RETURNS
    ///     String of the description associated with passed in space name.
    ///     
    /// EXCEPTION
    ///     Throws exception if a name is passed in that has no description.
    ///     
    /// </summary>
    string GetSpaceDescription(string a_name)
    {
        switch (a_name)
        {
            case "Go":
                return "Passing Go gives the player $200!";
            case "Income Tax":
            case "Luxury Tax":
                return "Landing on this space means you owe the bank money...";
            case "Just Visiting":
                return "Space where inmates live, and players can visit them";
            case "Free Parking":
                return "Nothing happens when you land on this space!";
            case "Go to Jail":
                return "Landing on this space means you lose your turn, go to the In Jail space, and have to pay $75 on your next turn to get out...";
            case "Chance":
            case "Community Chest":
                return "Pick up a card that will decide your fate!";
            default:
                throw new Exception("Error determining space description for space: " + a_name);
        }
    }
    /* string GetSpaceDescription(string a_name) */

    /// <summary>
    /// 
    /// NAME
    ///     CastActionString - returns Action associated with string.
    ///     
    /// SYNOPSIS
    ///     Actions CastActionString(string a_str)
    ///         a_str      --> string to cast.
    ///     
    /// DESCRIPTION
    ///     Converts action from string (text file) into actual
    ///     "Actions" enum type.
    ///     
    /// EXCEPTION
    ///     No action found to cast string to.
    ///     
    /// </summary>
    Actions CastActionString(string a_str)
    {
        // Match string with action
        switch (a_str)
        {
            // Actions
            case "LandedOn_UnownedProperty":
                return Actions.LandedOn_UnownedProperty;
            case "LandedOn_ChanceOrCommunityChest":
                return Actions.LandedOn_ChanceOrCommunityChest;
            case "LandedOn_VisitingJail":
                return Actions.LandedOn_VisitingJail;
            case "LandedOn_FreeParking":
                return Actions.EndTurn;
            case "LandedOn_GoToJail":
                return Actions.LandedOn_GoToJail;
            case "LandedOn_Tax":
                return Actions.LandedOn_Tax;
            case "LandedOn_Go":
                return Actions.LandedOn_Go;
            default:
                throw new Exception("No action found to cast to: " + a_str);
        }
    }
    /* Actions CastActionString(string str) */
}
