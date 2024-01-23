
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Class to handle the board and basically the entire game's C# logic at a high level
public class Board
{
    // Data members
    private List<Space> m_spaces = new List<Space>();
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board
    private List<Player> m_players = new List<Player>();
    private int m_turnNum;
    private CardController m_cardController = new CardController();

    // Potential actions a player might have to make
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

    // Constructor
    public Board()
    {

    }

    // Creates the spaces 
    public void InitializeBoard()
    {
        // Initialize the players and spaces
        InitializeSpaces();
        InitializePlayers();
    }

    void TestBuy(int playerNum, int propertyNum)
    {
        m_turnNum = playerNum;
        CurrentPlayer.CurrentSpace = propertyNum;
        PropertyPurchase();
    }

    // Obtains spaces from text file
    public void InitializeSpaces()
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
                // Generic space
                case "Jail":
                case "Go":
                case "Free Parking":
                case "Just Visiting":
                    currentSpace = new Space(name, spaceNum, action, GetSpaceDescription(name));
                    break;

                // Card
                case "Card":
                    currentSpace = new CardSpace(name, spaceNum, action, m_cardController, GetSpaceDescription(name));
                    break;


                // Tax
                case "Tax":
                    int taxCost = int.Parse(vals[3]);
                    currentSpace = new Tax (name, spaceNum, action, taxCost, GetSpaceDescription(name));
                    break;

                // Utility
                case "Utility":
                    purchasePrice = int.Parse(vals[3]);
                    currentSpace = new Utility(name, spaceNum, action, purchasePrice, 4, string.Empty);
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
                    currentSpace = new ColorProperty(name, spaceNum, action, purchasePrice, houseCost, landOnPrices, string.Empty, color);
                    break;

                // Railroad
                case "Railroad":

                    // Create object (all railroad prices are the same)
                    currentSpace = new Railroad(name, spaceNum, action, 200, 25, string.Empty);
                    break;


                // Case not found, error
                default:
                    currentSpace = new Space(name, spaceNum, action, "Unimplemented inherited class");
                    // ^ Change this to throw new Exception("Space data error at space: " + spaceNum);
                    break;
            }

            // Add space to list of spaces and move to next space index
            m_spaces.Add(currentSpace);
            spaceNum++;
        }
    }

    // Obtains the players from PlayerFile class
    public void InitializePlayers()
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

    // Returns space object at a particular index
    public Space GetSpace(int index)
    {
        if (index < 0 && index >= SPACE_NUM)
        {
            throw new ArgumentException("Space index out of range");
        }
        return m_spaces[index];
    }

    // Returns player object at indexty
    public Player GetPlayer(int index)
    {
        if (index < 0 || index > m_players.Count - 1)
        {
            throw new ArgumentException("Player index out of range");
        }

        return m_players[index];
    }

    // Returns how many houses are on a space
    public int GetPropertyHouses(int propertyIndex)
    {
        try
        {
            ColorProperty property = (ColorProperty)m_spaces[propertyIndex];
            return property.Houses;
        }
        catch
        {
            throw new Exception("Trying to find number of houses of a property which cannot " +
                "have houses on it! (non- ColorProperty type");
        }
    }

    // Returns the current player
    public Player CurrentPlayer
    {
        get { return m_players[m_turnNum]; }
    }

    // Returns the amount of players in the game
    public int PlayerCount
    {
        get { return m_players.Count; }
    }

    // Throws an exception if index out of bounds
    public void CheckPlayerNum(int playerNum)
    {
        if ((playerNum < 0) || playerNum > PlayerCount)
        {
            throw new Exception("Player num out of bounds!");
        }
    }

    // Returns player's icon name
    public string GetPlayerIconName(int playerNum)
    {
        // Check player num 
        CheckPlayerNum(playerNum);

        // Return the icon name
        return m_players[playerNum].Icon;
    }

    // Changes utility flag that the player is currently on, indicates they rolled dice to determine cost
    public void UtilityCostDetermined(int diceRoll)
    {
        Utility utility = (Utility)m_spaces[CurrentPlayer.CurrentSpace];
        utility.DiceRolled = true;
        utility.CurrentDiceRoll = diceRoll;
    }

    // Determines what action the current player must make depending on the state of their turn
    public Actions DetermineAction()
    {
        // Roll dice to determine turn
        if (!CurrentPlayer.TurnInitialized)
        {
            return Actions.DetermineOrder;
        }

        // Roll dice to move
        if (!CurrentPlayer.RolledDice && !CurrentPlayer.InJail)
        {
            return Actions.RollDice;
        }

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
                {
                    return Actions.EndTurn;
                }
            }

            // Otherwise do the space activity, if it's an end turn activity (free parking, just visiting) and 
            // player rolled doubles, don't end turn
            Actions spaceAction = m_spaces[CurrentPlayer.CurrentSpace].ActionType;
            if (!(spaceAction == Actions.EndTurn && CurrentPlayer.RolledDoubles))
            {
                return m_spaces[CurrentPlayer.CurrentSpace].ActionType;
            }
        }

        // If they rolled doubles, let them roll again and have another turn (unless in jail)
        if (CurrentPlayer.RolledDoubles && !CurrentPlayer.InJail)
        {
            CurrentPlayer.SpaceActionCompleted = false;
            return Actions.RollDice;
        }

        // Nothing to do, end turn
        else
        {
            return Actions.EndTurn;
        }
    }


    // Updates whose turn it is
    public void UpdateTurn()
    {
        // Reset the current player's attributes
        ResetCurrentPlayer();

        // Reset utility bools 
        ResetUtilities();

        // Update whose turn is is (don't use a bankrupt player)
        while (true)
        {
            if (m_turnNum < m_players.Count - 1)
            {
                m_turnNum++;
            }
            else
            {
                m_turnNum = 0;
            }

            if (!CurrentPlayer.Bankrupt)
            {
                break;
            }
        }
    }

    // Resets the two utilities, flags that mark whether or not player rolled to determine price
    private void ResetUtilities()
    {
        Utility electricCompany = (Utility)m_spaces[12];
        Utility waterWorks = (Utility)m_spaces[12];
        electricCompany.DiceRolled = false;
        waterWorks.DiceRolled = false;
    }

    // Resets the attributes of a player at the end of their turn
    public void ResetCurrentPlayer()
    {
        CurrentPlayer.RolledDice = false;
        CurrentPlayer.RolledDoubles = false;
        CurrentPlayer.DiceRollResult = 0;
        CurrentPlayer.TurnCompleted = false;
        CurrentPlayer.SpaceActionCompleted = false;
    }

    // A player's order was determined 
    public void OrderDetermined(int diceResult)
    {
        // Update the players attributes
        CurrentPlayer.OrderDeterminingDiceResult = diceResult;
        CurrentPlayer.TurnInitialized = true;
    }

    // Returns whether or not all the players were initialized
    public bool AllPlayersInitialized()
    {
        // Check each player to see if they were not initialized
        foreach (Player player in m_players)
        {
            if (!player.TurnInitialized)
            {
                return false;
            }
        }

        // All players initialized
        return true;
    }

    // Reorders the players based on what space they're on, resets thier current space
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

    // Sorts players based on their current space, as determined by a dice roll
    static int SortPlayers(Player player1, Player player2)
    {
        if (player1.OrderDeterminingDiceResult < player2.OrderDeterminingDiceResult)
        {
            return 1;
        }
        else if (player1.OrderDeterminingDiceResult > player2.OrderDeterminingDiceResult)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    // Checks if all players but one are bankrupt
    public bool GameOver()
    {
        // Count amount of bankrupt players
        int bankruptPlayers = 0;
        foreach (Player player in m_players)
        {
            if (player.Bankrupt)
            {
                bankruptPlayers++;
            }
        }

        // Check if all but one are bankrupt
        if (bankruptPlayers == m_players.Count - 1)
        {
            return true;
        }
        return false;
    }

    // Returns a message about the player order
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

        // Return the message
        return message;
    }

    // Returns an appropriate title for landing on unowned property
    public string GetLandedOnUnownedPropertyTitle()
    {
        Property purchaseProperty = (Property)m_spaces[CurrentPlayer.CurrentSpace];
        return "Would you like to buy " + purchaseProperty.Name + " for $" + purchaseProperty.PurchasePrice + "?";
    }

    // Returns a title for landing on an owned property
    public string GetLandedOnOwnedPropertyTitle()
    {
        // Cast to property 
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];

        // Obtain basic attribs
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
                {
                    retString += " but, color set is owned (rent doubled)";
                }
            }
            else if (colorProperty.Houses >= 1 && colorProperty.Houses <= 4)
            {
                retString += colorProperty.Houses + " house(s)";
            }
            else
            {
                retString += "1 hotel";
            }
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
            {
                retString += " owns both utilities";
            }
            else
            {
                retString += " owns only one utility";
            }
            return retString;
        }
        else
        {
            throw new Exception("Attempting to get landed on title for an owned property, " +
                "from a non-owned property type space...");
        }
    }

    // Player just went bankrupt, return message based on conditions
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

    // Returns the cost for landing on an owned property 
    public int GetLandedOnOwnedPropertyRent()
    {
        // Cast to property type and use derived method
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];
        return property.RentPrice;
    }

    // Returns a title for landing on a tax space
    public string GetLandedOnTaxTitle()
    {
        // Cast to tax
        Tax taxSpace = (Tax)m_spaces[CurrentPlayer.CurrentSpace];
        return "You landed on " + taxSpace.Name;
    }

    // Returns the tax amount for current tax property
    public int GetLandedOnTaxCost()
    {
        // Cast to tax
        Tax taxSpace = (Tax)m_spaces[CurrentPlayer.CurrentSpace];
        return taxSpace.TaxCost;
    }

    // Player rolled the dice
    public void DiceRolled(int diceResult, bool wereDoubles)
    {
        // Update the current space of player
        int destinationSpace = diceResult + CurrentPlayer.CurrentSpace;
        if (destinationSpace >= 40)
        {
            destinationSpace -= 40;
        }
        CurrentPlayer.CurrentSpace = destinationSpace;

        // Update current players "rolled doubles"
        CurrentPlayer.RolledDoubles = wereDoubles;

        // Update the players rolled dice boolean
        CurrentPlayer.RolledDice = true;
    }

    // Player picking up card
    public Card PickupCard()
    {
        // Return card for the player
        CardSpace cardSpace = (CardSpace)m_spaces[CurrentPlayer.CurrentSpace];
        return cardSpace.TakeCard();
    }

    // Returns the repair cost for current player based on card's values
    public int GetRepairCost(int houseCost, int hotelCost)
    {
        // Go through every property owned by current player
        int sumCost = 0;
        foreach(Property property in CurrentPlayer.Properties)
        {
            // Skip any railroads or utilities
            if (!(property is ColorProperty))
                continue;

            // Cast to color property
            ColorProperty colorProperty = (ColorProperty)property;

            // Add to sum for houses or hotels
            if (colorProperty.Houses == 5)
            {
                sumCost += hotelCost;
            }
            else
            {
                sumCost += houseCost * colorProperty.Houses;
            }
        }
        return sumCost;
    }

    // Player went bankrupt
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

    // Player going to jail
    public void GoToJail()
    {
        // Mark them as in jail and move them to just visiting
        CurrentPlayer.InJail = true;
        CurrentPlayer.CurrentSpace = 10;
        CurrentPlayer.SpaceActionCompleted = true;
    }

    // Player pays to get out of jail
    public void GetOutOfJailPay()
    {
        // Remove jail cost and unmark them as in jail
        CurrentPlayer.Cash -= 75;
        CurrentPlayer.InJail = false;
    }

    // Player uses card to get out of jail
    public void GetOutOfJailWithCard()
    {
        // Remove jail card and unmark them as in jail
        if (CurrentPlayer.CommunityChestJailCards >= 0)
        {
            CurrentPlayer.CommunityChestJailCards--;
        }
        else
        {
            CurrentPlayer.ChanceJailCards--;
        }
        CurrentPlayer.InJail = false;
    }

    // Current player purchases the current space they're on
    public void PropertyPurchase()
    {
        // Obtain the property
        Property currentSpace = (Property)m_spaces[CurrentPlayer.CurrentSpace];

        // Subtract cash from player
        CurrentPlayer.Cash -= currentSpace.PurchasePrice;

        // Add property to player
        CurrentPlayer.Properties.Add(currentSpace);

        // Sort the properties
        CurrentPlayer.Properties.Sort();

        // Mark space as purchased and add owner to space
        currentSpace.Owner = CurrentPlayer;
        currentSpace.IsPurchased = true;
    }


    // Player is paying rent on a property they landed om
    public void PayRent()
    {
        // Cast space
        Property property = (Property)m_spaces[CurrentPlayer.CurrentSpace];

        // Take cash from player who landed and give it to owner
        CurrentPlayer.Cash -= property.RentPrice;
        property.Owner.Cash += property.RentPrice;
    }

    // Player is paying tax from a tax space they landed on
    public void PayTax()
    {
        // Cast space
        Tax taxSpace = (Tax)m_spaces[CurrentPlayer.CurrentSpace];

        // Take cash 
        CurrentPlayer.Cash -= taxSpace.TaxCost;
    }

    // Player is buying a house
    public void BuyHouse(int propertyIndex)
    {
        // Obtain property
        ColorProperty property = (ColorProperty)GetSpace(propertyIndex);

        // Update it's properties
        CurrentPlayer.Cash -= property.HouseCost;
        property.Houses++;
    }

    // Player is selling a house, houses worth half their buy value
    public void SellHouse(int propertyIndex)
    {
        // Obtain property
        ColorProperty property = (ColorProperty)GetSpace(propertyIndex);

        // Update it's properties
        CurrentPlayer.Cash += property.HouseCost / 2;
        property.Houses--;
    }

    // Player is mortgaging a property
    public void MortgageProperty(int propertyIndex)
    {
        // Obtain property
        Property property = (Property)GetSpace(propertyIndex);

        // Add mortgage value to player's cash
        CurrentPlayer.Cash += property.MortgageValue;

        // Mark it as mortgaged
        property.IsMortgaged = true;
    }

    // Player is unmortgaging their property
    public void UnmortgageProperty(int propertyIndex)
    {
        // Obtain the property 
        Property property = (Property)GetSpace(propertyIndex);

        // Add mortgage value to player's cash
        CurrentPlayer.Cash -= property.MortgageValue;

        // Mark it as not mortgaged
        property.IsMortgaged = false;
    }

    // Determines whether or not a player can buy a house on a given property
    public bool HouseAvailible(Player player, ColorProperty property)
    {
        // Total number of houses full, property is mortgaged, not enough cash, or full color set unowned
        if (property.Houses >= 4 || property.HouseCost > player.Cash || property.IsMortgaged || !property.ColorSetOwned)
        {
            return false;
        }

        // Total house number would exceed other house minimum by > 1
        int houseMin = 4;
        List<ColorProperty> colorSet = GetColorSet(property);

        // Find smallest num of houses
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses < houseMin)
            {
                houseMin = colorProperty.Houses;
            }
        }

        // Check that new house total would not exeed min by 1
        if (property.Houses + 1 - houseMin > 1)
        {
            return false;
        }

        return true;
    }

    public bool SellHouseAvailible(Player player, ColorProperty property)
    {
        // Non-zero num of houses, not a hotel
        if (property.Houses <= 0 || property.Houses >= 5)
        {
            return false;
        }

        // Total house number would be lower than the max by > 1
        int houseMax = 0;
        List<ColorProperty> colorSet = GetColorSet(property);

        // Find smallest num of houses
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses > houseMax)
            {
                houseMax = colorProperty.Houses;
            }
        }

        // Check that new house total would not exeed min by 1
        if (houseMax - (property.Houses - 1) > 1)
        {
            return false;
        }

        return true;
    }

    // Determines whether or not a player can buy a hotel on a given property
    public bool HotelAvailible(Player player, ColorProperty property)
    {
        // House value is not 4, property is mortgaged, or not enough cash
        if (property.Houses != 4 || property.HouseCost > player.Cash || property.IsMortgaged)
        {
            return false;
        }

        // Color set all has at least 4 houses
        List<ColorProperty> colorSet = GetColorSet(property);
        foreach (ColorProperty colorProperty in colorSet)
        {
            if (colorProperty.Houses < 4)
            {
                return false;
            }
        }

        return true;
    }

    // Determines whether or not a player can unmortgage their property
    public bool UnmortgageAvailible(Player player, Property property)
    {
        // Property is mortgaged, and player can afford to buy it
        if (property.IsMortgaged && property.MortgageValue < player.Cash)
        {
            return true;
        }
        return false;
    }

    // Returns the full color set of a given property
    private List<ColorProperty> GetColorSet(ColorProperty property)
    {
        // Get the color
        string color = property.Color;
        List<ColorProperty> colorSet = new List<ColorProperty>();
        foreach (Space space in m_spaces)
        {
            // Only check color properties
            if (space is  ColorProperty)
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

    // Casts a string version of an action into the enum type
    private Actions CastActionString(string str)
    {
        // Match string with action
        switch (str)
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
                return Actions.ERROR;
        }
    }

    // Returns a description for a space depending on it's action type
    private string GetSpaceDescription(string name)
    {
        switch (name)
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
                throw new Exception ("Error determining space description for space: " + name);
        }
    }

    // Returns player with specified name
    public Player FindPlayerByName(string name)
    {
        // Return player 
        foreach (Player player in m_players)
        {
            if (player.Name == name)
            {
                return player;
            }
        }

        // Player not found, freak out
        throw new Exception("Player: " + name + " not found searching by name...");
    }

    // Returns property with specified name
    public Property FindPropertyByName(string name)
    {
        // Return property 
        foreach (Space space in m_spaces)
        {
            // Only check property spaces
            if (space is Property)
            {
                Property property = (Property)space;
                if (property.Name == name) 
                {
                    return property;
                }
            }
        }

        // Property never found
        throw new Exception("Property: " + name + " not found searching by name...");
    }

    // Returns a string list of player's properties' names
    public List<string> GetPlayerPropertiesAndCardsStrings(Player player)
    {
        // Iterate over all owned properties
        List<string> items = new List<string>();
        foreach (Property property in player.Properties)
        {
            items.Add(property.Name);
        }

        // Iterate over cards
        for (int i = 0; i < player.CommunityChestJailCards; i++)
        {
            items.Add("Community Chest Jail Card");
        }
        for (int i = 0; i < player.ChanceJailCards; i++)
        {
            items.Add("Chance Jail Card");
        }

        // Return the list
        return items;
    }

    // Saves game data to text file for end game scene to read
    public void SaveEndGameData()
    {
        // Create textfile
        string filePath = Application.streamingAssetsPath + "endGameData.txt";

        // Create write string
        List<string> data = new List<string>();

        // Find winning player
        Player winner= null;
        foreach(Player player in m_players)
        {
            if (!player.Bankrupt)
            {
                winner = player;
                break;
            }    
        }

        // Add their name
        data.Add(winner.Name);

        // Add their icon
        data.Add(winner.Icon);

        // Add their cash
        data.Add(winner.Cash.ToString());

        //Add their properties
        string propertiesList = "";
        int i = 0;
        foreach (Property property in winner.Properties)
        {
            propertiesList += property.Name;
            if (i != winner.Properties.Count - 1)
            {
                propertiesList += ", ";
            }
            i++;
        }
        data.Add(propertiesList);

        // Write all the data
        File.WriteAllLines(filePath, data);
    }
}
