using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// CLASS
///     Controller_Game : MonoBehaviour - runs the game.
///     
/// DESCRIPTION
///     This class connects the Unity classes of the game with the
///     Monopoly logic classes; pretty much all of the buttons within
///     the game point to methods in this class, and those methods will
///     call the appropriate Board class methods depending on the state of 
///     the game, what button it was, etc. This class is the most complex
///     because it controlls all the controller classes, and has the sole
///     referene to the Board class. This class also handles the majority of the 
///     player panel functionality.
/// 
/// </summary>
public class Controller_Game : MonoBehaviour
{
    // ======================================== Unity Data Members ========================================= //

    // Popup window
    public Controller_Popup m_popupController;

    // Buttons the user can click to get info about stuff
    public List<Button> m_spaceButtons;
    public List<Button> m_playerButtons;

    // Controller classes
    public Controller_DetailsPopup m_spaceDetailsController;
    public Controller_DetailsPopup m_playerDetailsController;
    public Controller_PlayerTrack m_playerTrackController;
    public Controller_Camera m_cameraController;
    public Action_RollDice m_diceRollController;
    public Action_Generic m_genericActionController;
    public Action_TwoChoice m_twoChoiceActionController;
    public Controller_Trading m_tradingController;

    // Folder of icons a player can have as their game token
    public List<Sprite> m_icons;

    // Player panel components
    public Image m_panelIcon;
    public TMP_Text m_panelTitle;
    public TMP_Text m_panelCash;
    public List<GameObject> m_actionWindows;
    public RectTransform m_propertyCardContent;
    public List<RenderTexture> m_propertyRenderTextures;
    public Scrollbar m_propertyCardScrollbar;
    public PropertyManager m_propertyManager;
    public Sprite m_chanceGetOutOfJailFreeCard;
    public Sprite m_communityChestGetOutOfJailFreeCard;

    // ======================================== Private Data Members ======================================= //
    Board m_board;
    bool m_updateMade = false;

    // ======================================== Start / Update ============================================= //

    /// <summary>
    /// 
    /// NAME
    ///     Start - initializes the game.
    ///     
    /// DESCRIPTION
    ///     Initializes the game by making all houses and hotels hidden, adding listeners to the spaces,
    ///     creating and initializing the Board, and closing the popup window.
    /// 
    /// </summary>
    void Start()
    {
        // Assign space button methods
        foreach (Button button in m_spaceButtons)
            button.onClick.AddListener(() => OnSpaceClick(int.Parse(button.name)));

        // Initialize the board
        m_board = new Board();
        m_board.InitializeBoard();

        // Need to set the panel initially 
        m_updateMade = true;

        // Close popup window
        m_popupController.ClosePopupWindow();

        // Erase all the houses to start
        EraseAllHousesAndHotels();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Update - checks for updates.
    ///     
    /// DESCRIPTION
    ///     This method runs every frame, constantly checks if updates have been made by
    ///     the player. this method will redraw the player panel if an update is made, or 
    ///     switch which player's data is being displayed in the player panel if 
    ///     the turn swithced. Also checks for the user's mouse being in bounds or clicking
    ///     off of a popup, and will close any pop-ups for both those cases.
    /// 
    /// </summary>
    void Update()
    {
        // Check if anything has been updated
        if (m_updateMade)
        {
            // Create the player panel so player can do their turn
            CreatePlayerPanel();

            // Set bool to false now that update has been made
            m_updateMade = false;
        }

        // Update whose turn it is if turn completed
        if (m_board.CurrentPlayer.TurnCompleted)
        {
            // Update the turn in board
            m_board.UpdateTurn();

            // Alert ourselves that an update was made to panel
            m_updateMade = true;

            // Reset the camera 
            m_cameraController.ResetCamera();

            // Reset scrollbar in the property card view
            m_propertyCardScrollbar.value = 0f;

            // Close the properties and cards window if it's open
            m_propertyManager.ClosePropertyManger();

            // Close the trading menu if it's open
            m_tradingController.CloseTradingMenu();
        }

        // Erase the details windows if user clicks or mouse over the player panel
        if (Input.GetMouseButtonDown(0) || !m_cameraController.MouseInBounds())
        {
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
        }
    }

    // ======================================== Public Methods ============================================= //

    // Player ended their turn
    public void Action_EndTurn()
    {
        m_board.CurrentPlayer.TurnCompleted = true;
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_OrderDetermined - determines players' orders for the game.
    /// 
    /// SYNOPSIS
    ///     public void Action_OrderDetermined(int a_diceResult);
    ///         a_diceResult        --> value of player's dice roll.
    /// 
    /// DESCRIPTION
    ///     This method runs will keep track of each players first dice roll, and
    ///     when all have made their first roll, will determine what order to sort the players 
    ///     for the remaineder of the game.
    /// 
    /// </summary>
    public void Action_OrderDetermined(int a_diceResult)
    {
        // Update the players attributes
        m_board.OrderDetermined(a_diceResult);

        // If all players are initialized, we can sort them and finish this action
        if (m_board.AllPlayersInitialized())
        {
            // Update the dice rolling script
            m_diceRollController.OrderDetermined = true;

            // Sort the players 
            m_board.InitializePlayerOrder();

            // Initialize the icons now with proper order 
            InitializePlayerIcons();

            // Show a popup that players have been intialized
            m_popupController.CreatePopupWindow("Order Determined!", m_board.GetOrderDeterminedMessage(), 'G');
        }

        // Mark the current player as having their turn completed
        m_board.CurrentPlayer.TurnCompleted = true;
    }
    /* public void Action_OrderDetermined(int diceResult) */

    /// <summary>
    /// 
    /// NAME
    ///     Action_UtilityCostDetermined - player determined Utility cost.
    /// 
    /// SYNOPSIS
    ///     public void Action_UtilityCostDetermined(int a_diceRoll);
    ///         a_diceResult        --> value of player's dice roll.
    /// 
    /// DESCRIPTION
    ///     Current player landed on an owned Utility space, so they
    ///     had to roll the dice and determine the dice mulitiplier for
    ///     how much the rent will cost. This method marks that action
    ///     as having occured.
    /// 
    /// </summary>
    public void Action_UtilityCostDetermined(int a_diceRoll)
    {
        // Update flags and properties on space and controller
        m_diceRollController.UtilityCostRoll = false;
        m_board.UtilityCostDetermined(a_diceRoll);

        // Mark update made
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_DiceRolled - player rolled the dice.
    /// 
    /// SYNOPSIS
    ///     public void Action_DiceRolled(int a_diceResult, bool a_wereDoubles);
    ///         a_diceResult        --> value of player's dice roll.
    ///         a_wereDoubles       --> matching die faces, or not.
    /// 
    /// DESCRIPTION
    ///     Current player rolled the dice to move along the board. This method
    ///     moves them, checks if they passed go, and marks an update has having occured.
    /// 
    /// </summary>
    public void Action_DiceRolled(int a_diceResult, bool a_wereDoubles)
    {
        // Update board
        int currentSpace = m_board.CurrentPlayer.CurrentSpace;
        m_board.DiceRolled(a_diceResult, a_wereDoubles);
        int destinationSpace = m_board.CurrentPlayer.CurrentSpace;

        // Move the player icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, currentSpace, destinationSpace));

        // Post go message if passing it
        if (currentSpace > destinationSpace) 
        {
            CreateGenericActionWindow("You passed Go!", "Collect $200", Color.green);
            m_genericActionController.ActButton.onClick.AddListener(Action_CollectGo);
            return;
        }

        // Update was made
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_BuyingProperty - player buying property.
    /// 
    /// SYNOPSIS
    ///     public void Action_BuyingProperty(bool a_buying);
    ///         a_buying            --> whether or not the player is buying the property.
    /// 
    /// DESCRIPTION
    ///     Based on whether or not the player is buying property, this
    ///     method will allow them to buy it, and regardless complete their turn.
    /// 
    /// </summary>
    public void Action_BuyingProperty(bool a_buying)
    {
        // Call Board method
        if (a_buying)
            m_board.PurchaseProperty();

        // Completed space action
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_PayingRent - current player pays rent on color property.
    ///     
    /// DESCRIPTION
    ///     The current player landed on a color property they do not own. This method
    ///     is called when they are paying the rent to the owner. Checks for 
    ///     bankruptcy as well. 
    /// 
    /// </summary>
    public void Action_PayingRent()
    {
        // Subtract the cash from player
        m_board.PayRent();

        // Update cash panel
        UpdatePanelCash();

        // Check bankruptcy 
        if (m_board.CurrentPlayer.Bankrupt)
        {
            // Tell the user
            CreateGenericActionWindow(m_board.GetBankruptMessage(), "Relinquish Property", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoingBankrupt);
            return;
        }

        // Completed space action
        m_board.CurrentPlayer.SpaceActionCompleted = true;

        // Update made to game state
        m_updateMade = true;
    }
    /* public void Action_PayingRent() */

    /// <summary>
    /// 
    /// NAME
    ///     Action_PayingTax - current player pays tax.
    ///     
    /// DESCRIPTION
    ///     Current player landed on a tax space, they are 
    ///     paying for it here. Checks for bankruptcy. 
    /// 
    /// </summary>
    public void Action_PayingTax()
    {
        // Subtract cash from player
        m_board.PayTax();

        // Update panel 
        UpdatePanelCash();

        // Check bankruptcy 
        if (m_board.CurrentPlayer.Bankrupt)
        {
            // Tell the user
            CreateGenericActionWindow(m_board.GetBankruptMessage(), "Relinquish Property", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoingBankrupt);
            return;
        }

        // Completed space action
        m_board.CurrentPlayer.SpaceActionCompleted = true;

        // Update made to game state
        m_updateMade = true;
    }
    /* public void Action_PayingTax() */


    /// <summary>
    /// 
    /// NAME
    ///     Action_GoToJail - current player is going to jail.
    ///     
    /// DESCRIPTION
    ///     Current player landed on Go to Jail, or got sent to jail
    ///     by Chance or Community Chest. This method moves them there,
    ///     and updates the board with this info.
    /// 
    /// </summary>
    public void Action_GoToJail()
    {
        // Move the player's icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, m_board.CurrentPlayer.CurrentSpace, 10));

        // Update board
        m_board.GoToJail();

        // Update the dice roller
        m_diceRollController.RolledDoubles = false;

        // Update made
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_GetOutOfJailPay - current player pays to get out of jail.
    ///     
    /// DESCRIPTION
    ///     Current player chose to pay to get out of jail. Update board
    ///     and check for bankruptcy.
    /// 
    /// </summary>
    public void Action_GetOutOfJailPay()
    {
        // Update board
        m_board.GetOutOfJailPay();

        // Check bankruptcy 
        if (m_board.CurrentPlayer.Bankrupt)
        {
            // Tell the user
            CreateGenericActionWindow(m_board.GetBankruptMessage(), "Relinquish Property", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoingBankrupt);
            return;
        }

        // Update made
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_GetOutOfJailWithCard - current player uses card to get out of jail.
    ///     
    /// DESCRIPTION
    ///     Current player had a Get out of Jail Free card, from Community Chest or Chance,
    ///     and chose to use it to get out of jail. 
    /// 
    /// </summary>
    public void Action_GetOutOfJailWithCard()
    {
        // Remove player's jail card
        m_board.GetOutOfJailWithCard();

        // Update made
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_GoingBankrupt - current player ran out of money.
    ///     
    /// DESCRIPTION
    ///     Current player is out of money, this method will mark them 
    ///     as being out of the game, and also check if the game 
    ///     is in a terminal state.
    /// 
    /// </summary>
    public void Action_GoingBankrupt()
    {
        // Update board
        m_board.GoingBankrupt();

        // Check if game over (all but one players bankrupt)
        if (m_board.GameOver())
        {
            // Open end game scene after saving data
            SaveEndGameData();
            SceneManager.LoadScene("End Menu");
        }

        // End their turn
        Action_EndTurn();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_CollectGo - current player passed Go.
    ///     
    /// DESCRIPTION
    ///     The current player passed go, so they collect $200,
    ///     and their turn ends if they are on Go space.
    /// 
    /// </summary>
    public void Action_CollectGo()
    {
        // User collects 200
        m_board.CurrentPlayer.Cash += 200;

        // Update cash
        UpdatePanelCash();

        // Update space action (if on go)
        if (m_board.CurrentPlayer.CurrentSpace == 0)
            m_board.CurrentPlayer.SpaceActionCompleted = true;

        // Update the panel
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_PickedUpCard - current player picked up a community chest or
    ///     chance card.
    ///     
    /// DESCRIPTION
    ///     Current player picks up a community chest or chance card, depending on 
    ///     the card action, an action window is created in the player panel asking
    ///     the player to do something.
    /// 
    /// </summary>
    public void Action_PickedUpCard()
    {
        // Obtain a card for them
        Card card = m_board.PickupCard();

        // Create window according to the card's action and attatch appropriate action method
        switch(card.ActionType)
        {
            case Controller_Card.Actions.collectMoney:
                CreateGenericActionWindow(card.Description, "Collect $" + card.Value, Color.green);
                m_genericActionController.ActButton.onClick.AddListener(() => Card_CollectMoney(card.Value));
                break;
            case Controller_Card.Actions.payMoney:
                CreateGenericActionWindow(card.Description, "Pay $" + card.Value, Color.red);
                m_genericActionController.ActButton.onClick.AddListener(() => Card_PayMoney(card.Value));
                break;
            case Controller_Card.Actions.getJailCard:
                CreateGenericActionWindow(card.Description, "Get Card", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(Card_GetJailCard);
                break;
            case Controller_Card.Actions.move:
                CreateGenericActionWindow(card.Description, "Move", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(() => Card_MoveToSpace(card.Location));
                break;
            case Controller_Card.Actions.makeRepairs:
                int repairCost = m_board.GetRepairCost(card.Value, card.Value2);
                CreateGenericActionWindow(card.Description, "Pay $" + repairCost, Color.red);
                m_genericActionController.ActButton.onClick.AddListener(() => Card_MakeRepairs(repairCost));
                break;

            default:
                throw new Exception("Card action not defined");
        }
    }
    /* public void Action_PickedUpCard() */

    // Getting jail card from card
    public void Card_GetJailCard()
    {
        // Give player jail card
        if (m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace).Name == "Chance")
            m_board.CurrentPlayer.ChanceJailCards++;
        
        else
            m_board.CurrentPlayer.CommunityChestJailCards++;
        

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Collecting money from card
    public void Card_CollectMoney(int amount)
    {
        // Add cash
        m_board.CurrentPlayer.Cash += amount;

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Paying money from card
    public void Card_PayMoney(int amount)
    {
        // Subtract cash
        m_board.CurrentPlayer.Cash -= amount;

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Making repairs from card
    public void Card_MakeRepairs(int amount)
    {
        // Subtract cash
        m_board.CurrentPlayer.Cash -= amount;

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Moving to space from card
    public void Card_MoveToSpace(string location)
    {
        // Find space to move to, start with current space (some cards ask find nearest
        int currentSpace = m_board.CurrentPlayer.CurrentSpace;
        int originSpace = currentSpace;

        // Search spaces after current to Go
        int destinationSpace = -1;
        int spacesSearched = 0;
        while (spacesSearched <= 40)
        {
            // Perfect match 
            if (m_board.GetSpace(currentSpace).Name == location)
            {
                destinationSpace = currentSpace;
                break;
            }

            // Looking for nearest railroad
            if (m_board.GetSpace(currentSpace) is Railroad && location == "Railroad")
            {
                destinationSpace = currentSpace;
                break;
            }

            // Looking for nearest utility
            else if (m_board.GetSpace(currentSpace) is Utility && location == "Utility")
            {
                destinationSpace = currentSpace;
                break;
            }

            // Update current space
            currentSpace++;
            if (currentSpace == 40)
            {
                currentSpace = 0;
            }
            spacesSearched++;
        }

        // Check that current space is found
        if (destinationSpace == -1)
        {
            throw new Exception("Couldn't find space to move to :(");
        }

        // Move the player icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, m_board.CurrentPlayer.CurrentSpace, destinationSpace));

        // Move player on board
        m_board.CurrentPlayer.CurrentSpace = destinationSpace;

        // If going to jail, mark player as in jail
        if (m_board.GetSpace(destinationSpace).Name == "Just Visiting")
        {
            Action_GoToJail();
            return;
        }    

        // If passed go, post message
        else if (m_board.GetSpace(destinationSpace).Name == "Go" || originSpace > currentSpace)
        {
            CreateGenericActionWindow("You passed Go!", "Collect $200", Color.green);
            m_genericActionController.ActButton.onClick.AddListener(Action_CollectGo);
            return;
        }

        // Mark update
        UpdateMade();
    }

    // Property manager button methods
    public void PropertyManager_BuyHouse(int propertyIndex)
    {
        // Buy the house
        m_board.BuyHouse(propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Draw the house/hotel on the board
        int houseNum = m_board.GetPropertyHouses(propertyIndex);

        // If hotel, remove all houses first
        if (houseNum == 5)
        {
            for (int i = 0; i < houseNum; i++) 
            {
                FindHouseOrHotelIcon(propertyIndex, i + 1).SetActive(false);
            }
        }
        FindHouseOrHotelIcon(propertyIndex, houseNum).SetActive(true);

        // Redraw the window
        OnPropertyClick(propertyIndex);
    }
    public void PropertyManager_SellHouse(int propertyIndex)
    {
        // If hotel, add back houses
        int houseNum = m_board.GetPropertyHouses(propertyIndex);
        if (houseNum == 5)
        {

            for (int i = 0; i < houseNum; i++)
            {
                FindHouseOrHotelIcon(propertyIndex, i + 1).SetActive(true);
            }
        }

        // Remove the hotel icon
        FindHouseOrHotelIcon(propertyIndex, houseNum).SetActive(false);
        
        // Sell the house/hotel
        m_board.SellHouse(propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(propertyIndex);
    }
    public void PropertyManager_MortgageProperty(int propertyIndex)
    {
        // Mortgage the property
        m_board.MortgageProperty(propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(propertyIndex);
    }
    public void PropertyManager_UnmortgageProperty(int propertyIndex)
    {
        // Buy back the property
        m_board.UnmortgageProperty(propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(propertyIndex);
    }
    public void PropertyManager_StoppedManaging()
    {
        m_propertyManager.ClosePropertyManger();
    }

    // Player traded with someone
    public void TradeMade(string playerName, string itemName, int cashAmount, bool propertyTraded, bool cardTraded)
    {
        // Find the player being traded with
        Player tradeToPlayer = m_board.GetPlayerByName(playerName);

        // If trading a property, trade it
        if (propertyTraded)
        {
            // Find the property
            Property tradedProperty = m_board.GetPropertyByName(itemName);

            // Change owners
            tradeToPlayer.Properties.Add(tradedProperty);
            m_board.CurrentPlayer.Properties.Remove(tradedProperty);
            tradeToPlayer.Properties.Sort();
            tradedProperty.Owner = tradeToPlayer;
        }

        // If trading a card, trade it
        if (cardTraded) 
        {
            if (itemName == "Community Chest Jail Card")
            {
                tradeToPlayer.CommunityChestJailCards++;
                m_board.CurrentPlayer.CommunityChestJailCards--;
            }
            else
            {
                tradeToPlayer.ChanceJailCards++;
                m_board.CurrentPlayer.ChanceJailCards--;
            }
        }

        // If trading cash, trade it
        if (cashAmount > 0)
        {
            // Add cash to trade player's cash
            tradeToPlayer.Cash += cashAmount;

            // Subtract from current player's cash
            m_board.CurrentPlayer.Cash -= cashAmount;

            // Update the cash in the panel
            UpdatePanelCash();
        }

        // Redraw the property card view
        ClearPropertyCardView();
        CreatePropertyCardView();

        // Redraw the trading window with updates
        List<string> propertiesAndCards = m_board.GetPlayerElligibleTradeStrings(m_board.CurrentPlayer);
        m_tradingController.CreateTradingMenu(tradeToPlayer.Name, GetIconSprite(tradeToPlayer.Icon), propertiesAndCards, m_board.CurrentPlayer.Cash);
    }

    // ======================================== Private Methods ============================================ //

    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
        // Account for extra chance and community chest
        if (spaceIndex == 40)
        {
            spaceIndex = 2;
        }
        if (spaceIndex == 41)
        {
            spaceIndex = 8;
        }

        // Get the space info
        string spaceName = m_board.GetSpace(spaceIndex).Name;
        string spaceDescription = m_board.GetSpace(spaceIndex).Description;

        // Display it in the space details window, where the user clicked
        m_spaceDetailsController.CreateDetailsWindow(spaceName, spaceDescription);
    }

    // Player clicks on property they own in the properties and cards panel
    void OnPropertyClick(int spaceIndex)
    {
        // Reset the window 
        m_propertyManager.ResetWindow();

        // Obtain the property
        Property property = (Property)m_board.GetSpace(spaceIndex);

        // Feed in all parameters if a color property
        if (m_board.GetSpace(spaceIndex) is ColorProperty)
        {
            // Cast to inherited type so we can obtain color property specific values
            ColorProperty colorProperty = (ColorProperty)property;

            // Determine if player can purchase and sell houses or hotels
            Player player = m_board.CurrentPlayer;
            bool houseAvailible = m_board.HouseAvailible(player, colorProperty);
            bool hotelAvailible = m_board.HotelAvailible(player, colorProperty);
            bool sellHouseAvailible = m_board.SellHouseAvailible(colorProperty);
            bool sellHotelAvailible = colorProperty.Houses == 5;
            bool unmortgageAvailible = m_board.UnmortgageAvailible(player, colorProperty);

            m_propertyManager.CreatePropertyManager(colorProperty.Name, colorProperty.Description, colorProperty.MortgageValue, colorProperty.HouseCost,
                houseAvailible, sellHouseAvailible, hotelAvailible, sellHotelAvailible, !colorProperty.IsMortgaged, unmortgageAvailible, colorProperty.Index);
        }

        // Feed in limited paramaters if a utility or a railroad (no houses, hotels)
        else
        {
            bool unmortgageAvailible = m_board.UnmortgageAvailible(m_board.CurrentPlayer, property);
            m_propertyManager.CreatePropertyManager(property.Name, property.Description, property.MortgageValue, 0, false, false, false, false, property.IsMortgaged, unmortgageAvailible, 0);
        }
    }

    // When user clicks a player
    void OnPlayerClick(int playerNum)
    {
        // Get player reference of the person selected
        Player player = m_board.GetPlayer(playerNum);

        // Display detail window with their info
        m_playerDetailsController.CreateDetailsWindow(player.Name, player.Description);

        // Don't display trading menu if selected current player
        if (player == m_board.CurrentPlayer)
        {
            return;
        }

        // Obtain string list of current player's properties and cards they can trade
        List<string> propertiesAndCards = m_board.GetPlayerElligibleTradeStrings(m_board.CurrentPlayer);

        // Open the trading menu
        m_tradingController.CreateTradingMenu(player.Name, GetIconSprite(player.Icon), propertiesAndCards, m_board.CurrentPlayer.Cash);
    }

    // Updates the update made flag
    void UpdateMade()
    {
        // Mark update made
        m_updateMade = true;
    }

    // Update's players action status
    void ActionMade()
    {
        m_board.CurrentPlayer.SpaceActionCompleted = true;
    }

    // Updates cash display
    void UpdatePanelCash()
    {
        m_panelCash.text = "Cash: $" + m_board.CurrentPlayer.Cash.ToString();
    }

    // Creates the player panel for the player to do actions during their turn
    void CreatePlayerPanel()
    {
        // Determine action needed this turn
        Board.Actions action = m_board.DetermineAction();

        // Assign basic attributes
        m_panelIcon.sprite = GetIconSprite(m_board.CurrentPlayer.Icon);
        m_panelTitle.text = m_board.CurrentPlayer.Name + "'s Turn";
        m_panelCash.text = "Cash: $" + m_board.CurrentPlayer.Cash;

        // Deactivate any other action windows
        foreach (GameObject actionWindow in m_actionWindows)
        {
            actionWindow.SetActive(false);
        }

        // Set the proper action window
        switch (action)
        {
            // Dice rolling
            case Board.Actions.DetermineOrder:
            case Board.Actions.RollDice:
                m_actionWindows[1].SetActive(true);
                break;

            // Landed on a Utility, haven't rolled to determine cost
            case Board.Actions.DetermineUtilityCost:
                m_diceRollController.UtilityCostRoll = true;
                m_diceRollController.ResetWindow();
                m_actionWindows[1].SetActive(true);
                break;

            // Landed on go to jail
            case Board.Actions.LandedOn_GoToJail:
                CreateGenericActionWindow("You're going to jail, sorry!", "Move", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(Action_GoToJail);
                break;

            // Landed on visiting jail
            case Board.Actions.LandedOn_VisitingJail:

                // In jail
                if (m_board.CurrentPlayer.InJail)
                {
                    CreateTwoChoiceActionWindow("You must pay to be released from Jail...", "Pay $75", Color.red, "Use Jail Card", Color.black);
                    m_twoChoiceActionController.LeftButton.onClick.AddListener(Action_GetOutOfJailPay);
                    m_twoChoiceActionController.RightButton.onClick.AddListener(Action_GetOutOfJailWithCard);

                    // Check if they have a card
                    if (m_board.CurrentPlayer.CommunityChestJailCards == 0 && m_board.CurrentPlayer.ChanceJailCards == 0)
                    {
                        m_twoChoiceActionController.RightButton.interactable = false;
                    }
                }

                // Not in jail, rolled doubles
                else if (m_board.CurrentPlayer.RolledDoubles)
                {
                    m_actionWindows[1].SetActive(true);

                }

                // End their turn
                else
                {
                    CreateGenericActionWindow("No actions left to complete", "End Turn", Color.black);
                    m_genericActionController.ActButton.onClick.AddListener(Action_EndTurn);
                }
                break;

            // Landed on an unowned property 
            case Board.Actions.LandedOn_UnownedProperty:
                CreateTwoChoiceActionWindow(m_board.GetLandedOnUnownedPropertyTitle(), "Yes", Color.green, "No", Color.red);

                // Disable first button if cannot afford
                Property property = (Property)m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace);
                if (m_board.CurrentPlayer.Cash < property.PurchasePrice)
                {
                    m_twoChoiceActionController.LeftButton.interactable = false;
                }

                m_twoChoiceActionController.LeftButton.onClick.AddListener(() => Action_BuyingProperty(true));
                m_twoChoiceActionController.RightButton.onClick.AddListener(() => Action_BuyingProperty(false));
                m_actionWindows[2].SetActive(true);
                break;

            // Landed on a mortgaged property
            case Board.Actions.LandedOn_MortgagedProperty:
                Debug.Log("HERE");
                Property mortgagedProperty = (Property)m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace);
                CreateGenericActionWindow("You landed on " + mortgagedProperty.Name + ", owned by " +
                     mortgagedProperty.Owner.Name + ". But, because it is mortgaged you don't need to pay rent!",
                     "Continue", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(ActionMade);
                m_genericActionController.ActButton.onClick.AddListener(UpdateMade);
                break;

            // Landed on a jailed person's property
            case Board.Actions.LandedOn_JailedOwnerProperty:
                Property jailedProperty = (Property)m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace);
                CreateGenericActionWindow("You landed on " + jailedProperty.Name + ", owned by " +
                   jailedProperty.Owner.Name + ". But, because they are in jail you don't need to pay rent!",
                   "Continue", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(ActionMade);
                m_genericActionController.ActButton.onClick.AddListener(UpdateMade);
                break;

            // Landed on an owned property
            case Board.Actions.LandedOn_OwnedColorProperty:
            case Board.Actions.LandedOn_OwnedRailroad:
            case Board.Actions.LandedOn_OwnedUtility:
                CreateGenericActionWindow(m_board.GetLandedOnOwnedPropertyTitle(), 
                    "Pay: " + (-1 * m_board.GetLandedOnOwnedPropertyRent()), Color.red);
                m_genericActionController.ActButton.onClick.AddListener(Action_PayingRent);
                break;

            // Landed on a tax property
            case Board.Actions.LandedOn_Tax:
                CreateGenericActionWindow("You landed on " + m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace).Name,
                    "Pay: $" + (-1 * m_board.GetLandedOnTaxCost()), Color.red);
                m_genericActionController.ActButton.onClick.AddListener(Action_PayingTax);
                break;

            // Landed on a card property
            case Board.Actions.LandedOn_ChanceOrCommunityChest:
                CreateGenericActionWindow("You landed on " + m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace).Name, 
                    "Pickup Card", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(Action_PickedUpCard);
                break;

            // Ending turn 
            case Board.Actions.EndTurn:
                CreateGenericActionWindow("No Actions Left to Complete", "End Turn", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(Action_EndTurn);
                break;

            // Error if hitting this default case
            default:
                CreateGenericActionWindow("No Actions Left to Complete", "End Turn", Color.black);
                m_genericActionController.ActButton.onClick.AddListener(Action_EndTurn);
                Debug.Log("Determine action did not find action for this move...");
                break;
        }

        // Display the properties owned by the player in the properties and cards section
        ClearPropertyCardView();
        CreatePropertyCardView();
    }

    // Creates a generic action window 
    void CreateGenericActionWindow(string title, string buttonText, Color buttonColor)
    {
        // Set text attributes
        m_genericActionController.Title = title;
        m_genericActionController.ActButtonText = buttonText;
        m_genericActionController.ActButtonColor = buttonColor;

        // Set the window to active 
        m_actionWindows[0].gameObject.SetActive(true);

        // Clear listeners 
        m_genericActionController.ResetListeners();
    }

    // Creates a two choice action window
    void CreateTwoChoiceActionWindow(string title, string leftButtonText, Color leftButtonColor, string rightButtonText, Color rightButtonColor)
    {
        // Clear listeners 
        m_twoChoiceActionController.LeftButton.onClick.RemoveAllListeners();
        m_twoChoiceActionController.RightButton.onClick.RemoveAllListeners();

        // Reactivate buttons
        m_twoChoiceActionController.LeftButton.interactable = true;
        m_twoChoiceActionController.RightButton.interactable = true;

        // Set text
        m_twoChoiceActionController.Title = title;
        m_twoChoiceActionController.LeftButtonText = leftButtonText;
        m_twoChoiceActionController.RightButtonText = rightButtonText;
        m_twoChoiceActionController.LeftButtonColor = leftButtonColor;
        m_twoChoiceActionController.RightButtonColor = rightButtonColor;

        // Set window active
        m_actionWindows[2].SetActive(true);
    }

    // Clears all the property and card view contents and resets the size
    void ClearPropertyCardView()
    {
        // Obtain all the property views
        RawImage[] propertyViews = m_propertyCardContent.GetComponentsInChildren<RawImage>();

        // Destroy them
        foreach (RawImage propertyView in propertyViews)
        {
            Destroy(propertyView.gameObject);
        }

        // Obtain all the card views
        Image[] cardImages = m_propertyCardContent.GetComponentsInChildren<Image>();

        // Destroy them
        foreach (Image cardImage in cardImages)
        {
            Destroy(cardImage.gameObject);
        }
    }

    // Adds a new property to the properties and cards section
    void CreatePropertyCardView()
    {
        // Set the sizes
        float propertyWidth = 224f;
        float cardWidth = propertyWidth * 2;
        float propertyHeight = 300f;
        float startX = -3860f;

        // Loop through owned properties
        int propertyNum = 0;
        foreach (Space property in m_board.CurrentPlayer.Properties)
        {
            // Create object with viewer as child of properties content
            GameObject newPropertyImage = new GameObject("Property");
            newPropertyImage.transform.SetParent(m_propertyCardContent.transform);
            RawImage newViewer = newPropertyImage.AddComponent<RawImage>();
            newViewer.transform.localScale = new Vector2(1, 1);

            // Set size and position
            RectTransform newViewerRect = newPropertyImage.GetComponent<RectTransform>();
            newViewerRect.sizeDelta = new Vector2(propertyWidth, propertyHeight);
            float xOffset = startX + 10 + propertyWidth * propertyNum + 20 * propertyNum;
            newViewerRect.anchoredPosition = new Vector2(xOffset, 0);

            // Assign property render texture to the viewer
            newViewer.texture = FindPropertyTexture(property.Index);

            // Add button and listener for the space
            Button viewerButton = newViewer.AddComponent<Button>();
            viewerButton.onClick.AddListener(() => OnPropertyClick(property.Index));

            // Increment property
            propertyNum++;
        }

        // For each card player owns, add it 
        int numCards = m_board.CurrentPlayer.CommunityChestJailCards + m_board.CurrentPlayer.ChanceJailCards;
        int chanceCardsPrinted = 0;
        int communityChanceCardsPrinted = 0;
        for (int i = 0; i < numCards; i++)
        {
            // Create object with viewer as child of properties content
            GameObject newCardImage = new GameObject("Card");
            newCardImage.transform.SetParent(m_propertyCardContent.transform);
            Image newViewer = newCardImage.AddComponent<Image>();
            newViewer.transform.localScale = new Vector2(2f, 1);

            // Set size and position
            RectTransform newViewerRect = newCardImage.GetComponent<RectTransform>();
            newViewerRect.sizeDelta = new Vector2(propertyWidth, propertyHeight);
            float xOffset = startX + 125 + propertyWidth * propertyNum + cardWidth * i + 20 * propertyNum + i * 20;
            newViewerRect.anchoredPosition = new Vector2(xOffset, 0);

            // Print chance first
            if (chanceCardsPrinted < m_board.CurrentPlayer.ChanceJailCards)
            {
                newViewer.sprite = m_chanceGetOutOfJailFreeCard;
                chanceCardsPrinted++;
            }
            // Print community chest second
            else
            {
                newViewer.sprite = m_communityChestGetOutOfJailFreeCard;
                communityChanceCardsPrinted++;
            }
        }
    }

    // Initializes the player lanes and icons 
    void InitializePlayerIcons()
    {
        m_playerTrackController.CreateLanes();
        m_playerTrackController.SetIcons(m_playerButtons);
        for (int playerNum = 0; playerNum < m_board.PlayerCount; playerNum++)
        {
            // Create local player num
            int localPlayerNum = playerNum;

            // Add the onClick method
            m_playerButtons[playerNum].onClick.AddListener(() => OnPlayerClick(localPlayerNum));

            // Assign the icon
            m_playerButtons[playerNum].image.sprite = GetIconSprite(m_board.GetPlayerIconName(playerNum));

            // Move the player to space 0
            StartCoroutine(m_playerTrackController.MovePlayer(playerNum, 0, 0));
        }
    }
    
    // Deactivates all houses (for game start)
    void EraseAllHousesAndHotels()
    {
        // Each space
        foreach (Button spaceButton in m_spaceButtons)
        {
            // Check it's a color property 
            int spaceNum = int.Parse(spaceButton.name);
            try
            {
                // Try casting to color property
                ColorProperty property = (ColorProperty)m_board.GetSpace(spaceNum);
            }
            catch
            {
                // Ignore if doesn't work
                continue;
            }

            // Each house
            for (int i = 0; i < 5; i++)
            {
                // Deactivate the icon
                FindHouseOrHotelIcon(spaceNum, i + 1).SetActive(false);
            }
        }
    }

    // Returns house/hotel icon given the property num and house num
    GameObject FindHouseOrHotelIcon(int propertyNum, int houseNum)
    {
        // Determine icon name based on num
        string houseName = "house" + houseNum;
        if (houseNum == 5)
        {
            houseName = "hotel";
        }

        // Find the parent transform (property object in the scene)
        Transform propertyTransform = FindSpaceButtonParent(m_spaceButtons[propertyNum]);

        // Find the house object 
        Transform houseTransform = FindChildByName(propertyTransform, houseName);
        return houseTransform.gameObject;
    }

    // Saves the game data for use in the end menu
    void SaveEndGameData()
    {
        // Create textfile
        string filePath = Application.streamingAssetsPath + "endGameData.txt";

        // Create write string
        List<string> data = new List<string>();

        // Find winning player
        Player winner = m_board.GetWinner();

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

    // Returns the property parent object of a given space button
    Transform FindSpaceButtonParent(Button spaceButton)
    {
        // Find the house game object by traversing the hierarchy structure, starting w/ the space button
        Transform UITranform = spaceButton.transform.parent;
        return UITranform.parent;
    }

    // Returns the appropriate child from a parent
    Transform FindChildByName(Transform parent, string childName)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == childName)
                return child;
        }
        throw new System.Exception("Child not found by name!");
    }

    // Returns the image associated with a partular icon
    Sprite GetIconSprite(string iconName)
    {
        foreach (Sprite icon in m_icons)
        {
            if (iconName == icon.name)
            {
                return icon;
            }
        }
        return null;
    }

    // Finds a particular render texture for a given property 
    RenderTexture FindPropertyTexture(int spaceIndex)
    {
        // Use the name of the render texture to find the correct texture for a space index
        foreach (RenderTexture texture in m_propertyRenderTextures)
        {
            // Obtain the name without 'RT'
            string name = texture.name;
            name = name.Substring(0, name.Length - 2);

            // Cast to int
            int index = -1;
            int.TryParse(name, out index);

            // Return if its the matching texture
            if (index == spaceIndex)
            {
                return texture;
            }
        }

        // No space found, FREAK OUT!
        throw new Exception("No texture found for specified property! Index: " + spaceIndex);
    }
}
