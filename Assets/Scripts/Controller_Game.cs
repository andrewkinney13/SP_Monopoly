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
    public Controller_Trading m_tradingController;
    public Action_RollDice m_diceRollController;
    public Action_Generic m_genericActionController;
    public Action_TwoChoice m_twoChoiceActionController;

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
    const int ACTION_WINDOW_GENERIC = 0;
    const int ACTION_WINDOW_ROLL_DICE = 1;
    const int ACTION_WINDOW_TWO_CHOICE = 2;

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

        // Close popup windows
        m_popupController.ClosePopupWindow();
        m_spaceDetailsController.CloseDetailsWindow();
        m_playerDetailsController.CloseDetailsWindow();

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
            OrientCamera();

            // Reset scrollbar in the property card view
            m_propertyCardScrollbar.value = 0f;

            // Close the properties and cards window if it's open
            m_propertyManager.ClosePropertyManger();

            // Close the trading menu if it's open
            m_tradingController.CloseTradingMenu();

            // Close details popups
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
        }

        // Erase the details windows if user clicks or mouse over the player panel
        if (Input.GetMouseButtonDown(0))
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

        // Check if player got caught speeding, if so send them to jail
        if (m_board.CurrentPlayer.InJail)
        {
            CreateGenericActionWindow("You got caught speeding! (rolled doubles 3 times)", "Go to Jail", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoToJail);
            return;
        }

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

    // Current player collects a Get out of Jail Free card
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

    // Collecting money from Community Chest or Chance card
    public void Card_CollectMoney(int amount)
    {
        // Add cash
        m_board.CurrentPlayer.Cash += amount;

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Paying money from Community Chest or Chance card
    public void Card_PayMoney(int amount)
    {
        // Subtract cash
        m_board.CurrentPlayer.Cash -= amount;

        // Check bankruptcy 
        if (m_board.CurrentPlayer.Bankrupt)
        {
            // Tell the user
            CreateGenericActionWindow(m_board.GetBankruptMessage(), "Relinquish Property", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoingBankrupt);
            return;
        }


        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Making repairs on property from from Community Chest or Chance card
    public void Card_MakeRepairs(int amount)
    {
        // Subtract cash
        m_board.CurrentPlayer.Cash -= amount;

        // Check bankruptcy 
        if (m_board.CurrentPlayer.Bankrupt)
        {
            // Tell the user
            CreateGenericActionWindow(m_board.GetBankruptMessage(), "Relinquish Property", Color.red);
            m_genericActionController.ActButton.onClick.AddListener(Action_GoingBankrupt);
            return;
        }

        // Mark update
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    /// <summary>
    /// 
    /// NAME
    ///     Action_PickedUpCard - card player picked up is moving them somewhere.
    ///     
    ///         a_location      --> name / type of space being moved to
    ///     
    /// SYNOPSIS
    ///     public void Card_MoveToSpace(string a_location);
    ///         
    ///     
    /// DESCRIPTION
    ///     Current player picks up a community chest or chance card, and
    ///     is being either moved to an explicitly named space, or the 
    ///     nearest space. This method determines what exact space their going to, 
    ///     and moves them there. It also checks for being moved to jail, or passing go.
    ///     
    /// EXCEPTION
    ///     Throws exception if a_location cannot be intepereted as a space to move to.
    /// 
    /// </summary>
    public void Card_MoveToSpace(string a_location)
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
            if (m_board.GetSpace(currentSpace).Name == a_location)
            {
                destinationSpace = currentSpace;
                break;
            }

            // Looking for nearest railroad
            if (m_board.GetSpace(currentSpace) is Railroad && a_location == "Railroad")
            {
                destinationSpace = currentSpace;
                break;
            }

            // Looking for nearest utility
            else if (m_board.GetSpace(currentSpace) is Utility && a_location == "Utility")
            {
                destinationSpace = currentSpace;
                break;
            }

            // Update current space
            currentSpace++;
            if (currentSpace == 40)
                currentSpace = 0;
            
            spacesSearched++;
        }

        // Check that current space is found
        if (destinationSpace == -1)
            throw new Exception("Couldn't find space to move to :(");

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
    /* public void Card_MoveToSpace(string a_location) */

    /// <summary>
    /// 
    /// NAME
    ///     PropertyManager_BuyHouse - player buying a house for a property.
    ///     
    /// SYNOPSIS
    ///     public void PropertyManager_BuyHouse(int a_propertyIndex);
    ///         a_propertyIndex         --> the index on the board of where the 
    ///                                     property is.
    ///     
    /// DESCRIPTION
    ///     Current player is buying a house for a property that they own, this method
    ///     calls the appropriate Board function to do so, and then updates the 
    ///     Unity scene to show the house / hotel purchased.
    /// 
    /// </summary>
    public void PropertyManager_BuyHouse(int a_propertyIndex)
    {
        // Buy the house
        m_board.BuyHouse(a_propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Draw the house/hotel on the board
        int houseNum = m_board.GetPropertyHouses(a_propertyIndex);

        // If hotel, remove all houses first
        if (houseNum == 5)
        {
            for (int i = 0; i < houseNum; i++) 
                FindHouseOrHotelIcon(a_propertyIndex, i + 1).SetActive(false);
        }
        FindHouseOrHotelIcon(a_propertyIndex, houseNum).SetActive(true);

        // Redraw the window
        OnPropertyClick(a_propertyIndex);
    }
    /* public void PropertyManager_BuyHouse(int a_propertyIndex) */

    /// <summary>
    /// 
    /// NAME
    ///     PropertyManager_SellHouse - player selling a house on a property.
    ///     
    /// SYNOPSIS
    ///     public void PropertyManager_SellHouse(int a_propertyIndex);
    ///         a_propertyIndex         --> the index on the board of where the 
    ///                                     property is.
    ///     
    /// DESCRIPTION
    ///     Current player is selling a house on a property that they own, this method
    ///     calls the appropriate Board function to do so, and then updates the 
    ///     Unity scene to remove the house / hotel they sold.
    /// 
    /// </summary>
    public void PropertyManager_SellHouse(int a_propertyIndex)
    {
        // If hotel, add back houses
        int houseNum = m_board.GetPropertyHouses(a_propertyIndex);
        if (houseNum == 5)
        {
            for (int i = 0; i < houseNum; i++)
                FindHouseOrHotelIcon(a_propertyIndex, i + 1).SetActive(true);
        }

        // Remove the hotel icon
        FindHouseOrHotelIcon(a_propertyIndex, houseNum).SetActive(false);
        
        // Sell the house/hotel
        m_board.SellHouse(a_propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(a_propertyIndex);
    }
    /* public void PropertyManager_SellHouse(int a_propertyIndex) */

    // Current player is mortgaging a property 
    public void PropertyManager_MortgageProperty(int a_propertyIndex)
    {
        // Mortgage the property
        m_board.MortgageProperty(a_propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(a_propertyIndex);
    }

    // Current player is buying back a mortgaged property
    public void PropertyManager_UnmortgageProperty(int a_propertyIndex)
    {
        // Buy back the property
        m_board.UnmortgageProperty(a_propertyIndex);

        // Update cash of the player
        UpdatePanelCash();

        // Redraw the window
        OnPropertyClick(a_propertyIndex);
    }

    // Current player has closed the property managing window
    public void PropertyManager_StoppedManaging()
    {
        m_propertyManager.ClosePropertyManger();
    }

    /// <summary>
    /// 
    /// NAME
    ///     PropertyManager_SellHouse - player selling a house on a property.
    ///     
    /// SYNOPSIS
    ///      public void TradeMade(string a_playerName, string a_itemName, 
    ///      int a_cashAmount, bool a_propertyTraded, bool a_cardTraded);
    ///         a_playerName        --> name of the player being traded with. 
    ///        a_itemName          --> the name of the property / card being traded.
    ///        a_cashAmount        --> how much cash is being traded.
    ///        a_propertyTraded    --> whether or not a property is being traded.
    ///        a_cardTraded        --> whether or not a card is being traded. 
    ///     
    /// DESCRIPTION
    ///     Current player just initiated a trade with another player. This method 
    ///     determines what they traded, and makes the trade happen. A player can trade 
    ///     cash, property, or Get out of Jail free cards. 
    /// 
    /// </summary>
    public void TradeMade(string a_playerName, string a_itemName, int a_cashAmount, bool a_propertyTraded, bool a_cardTraded)
    {
        // Find the player being traded with
        Player tradeToPlayer = m_board.GetPlayerByName(a_playerName);

        // If trading a property, trade it
        if (a_propertyTraded)
        {
            // Find the property
            Property tradedProperty = m_board.GetPropertyByName(a_itemName);

            // Change owners
            tradeToPlayer.Properties.Add(tradedProperty);
            m_board.CurrentPlayer.Properties.Remove(tradedProperty);
            tradeToPlayer.Properties.Sort();
            tradedProperty.Owner = tradeToPlayer;
        }

        // If trading a card, trade it
        if (a_cardTraded) 
        {
            if (a_itemName == "Community Chest Jail Card")
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
        if (a_cashAmount > 0)
        {
            // Add cash to trade player's cash
            tradeToPlayer.Cash += a_cashAmount;

            // Subtract from current player's cash
            m_board.CurrentPlayer.Cash -= a_cashAmount;

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
    /* public void TradeMade(string a_playerName, string a_itemName, int a_cashAmount, bool a_propertyTraded, bool a_cardTraded) */


    // ======================================== Private Methods ============================================ //

    // Updates the update made flag
    void UpdateMade() { m_updateMade = true; }

    // Update's players action status
    void ActionMade() { m_board.CurrentPlayer.SpaceActionCompleted = true; }

    // Updates cash display
    void UpdatePanelCash() { m_panelCash.text = "Cash: $" + m_board.CurrentPlayer.Cash.ToString(); }

    /// <summary>
    /// 
    /// NAME
    ///     OnSpaceClick - user clicks on a space of the board.
    ///     
    /// SYNOPSIS
    ///     void OnSpaceClick(int a_spaceIndex);
    ///         a_spaceIndex        --> board index of the space clicked.
    ///     
    /// DESCRIPTION
    ///     User clicked on a space of the board for information about it, 
    ///     this method will create a popup window to display the relevent info.
    /// 
    /// </summary>
    void OnSpaceClick(int a_spaceIndex)
    {
        // Account for extra chance and community chest
        if (a_spaceIndex == 40)
            a_spaceIndex = 2;

        if (a_spaceIndex == 41)
            a_spaceIndex = 8;

        // Get the space info
        string spaceName = m_board.GetSpace(a_spaceIndex).Name;
        string spaceDescription = m_board.GetSpace(a_spaceIndex).Description;

        // Display it in the space details window, where the user clicked
        m_spaceDetailsController.CreateDetailsWindow(spaceName, spaceDescription);
    }

    /// <summary>
    /// 
    /// NAME
    ///     OnPropertyClick - user clicks on a property in their inventory.
    ///     
    /// SYNOPSIS
    ///     void OnPropertyClick(int a_spaceIndex);
    ///         a_spaceIndex        --> board index of the space clicked.
    ///     
    /// DESCRIPTION
    ///     User clicked on a property in their inventory (property view) 
    ///     to manage. This method will compile data about that property, and
    ///     send it to the property manager class which will create a window
    ///     for the player to manage the property.
    /// 
    /// </summary>
    void OnPropertyClick(int a_spaceIndex)
    {
        // Reset the window 
        m_propertyManager.ResetWindow();

        // Obtain the property
        Property property = (Property)m_board.GetSpace(a_spaceIndex);

        // Feed in all parameters if a color property
        if (m_board.GetSpace(a_spaceIndex) is ColorProperty)
        {
            // Cast to inherited type so we can obtain color property specific values
            ColorProperty colorProperty = (ColorProperty)property;

            // Determine if player can purchase and sell houses or hotels
            Player player = m_board.CurrentPlayer;
            bool houseAvailible = m_board.HouseAvailible(player, colorProperty);
            bool hotelAvailible = m_board.HotelAvailible(player, colorProperty);
            bool sellHouseAvailible = m_board.SellHouseAvailible(colorProperty);
            bool sellHotelAvailible = colorProperty.Houses == 5;
            bool mortgageAvailible = m_board.MortgageAvailible(colorProperty);
            bool unmortgageAvailible = m_board.UnmortgageAvailible(player, colorProperty);

            m_propertyManager.CreatePropertyManager(colorProperty.Name, colorProperty.Description, colorProperty.MortgageValue, colorProperty.HouseCost,
                houseAvailible, sellHouseAvailible, hotelAvailible, sellHotelAvailible, mortgageAvailible, unmortgageAvailible, colorProperty.Index);
        }

        // Feed in limited paramaters if a utility or a railroad (no houses, hotels)
        else
        {
            bool unmortgageAvailible = m_board.UnmortgageAvailible(m_board.CurrentPlayer, property);
            m_propertyManager.CreatePropertyManager(property.Name, property.Description, property.MortgageValue, 0, 
                false, false, false, false, !property.IsMortgaged, unmortgageAvailible, property.Index);
        }
    }
    /* void OnPropertyClick(int a_spaceIndex) */

    /// <summary>
    /// 
    /// NAME
    ///     OnPlayerClick - user clicked a player icon.
    ///     
    /// SYNOPSIS
    ///     void OnPlayerClick(int a_spaceIndex);
    ///         a_playerNum        --> index of the player the user clicked on.
    ///     
    /// DESCRIPTION
    ///     User clicked on a player icon of the board for information about
    ///     that player, this method will create a popup window to display the 
    ///     relevent info.
    /// 
    /// </summary>
    void OnPlayerClick(int a_playerNum)
    {
        // Get player reference of the person selected
        Player player = m_board.GetPlayer(a_playerNum);

        // Display detail window with their info
        m_playerDetailsController.CreateDetailsWindow(player.Name, player.Description);

        // Don't display trading menu if selected current player or selected player bankrupt
        if (player == m_board.CurrentPlayer || player.Bankrupt)
            return;

        // Close the property manager if it's open
        m_propertyManager.ClosePropertyManger();

        // Obtain string list of current player's properties and cards they can trade
        List<string> propertiesAndCards = m_board.GetPlayerElligibleTradeStrings(m_board.CurrentPlayer);

        // Open the trading menu
        m_tradingController.CreateTradingMenu(player.Name, GetIconSprite(player.Icon), propertiesAndCards, m_board.CurrentPlayer.Cash);
    }

    /// <summary>
    /// 
    /// NAME
    ///     CreatePlayerPanel - creates the entire right side of the game,
    ///                         that the user interacts with.
    ///     
    /// DESCRIPTION
    ///     Gathers all the information about the the current player: their name, 
    ///     cash, icon, curent action, properties... and fills in the player 
    ///     panel, on the right side of the screen, with all this info.
    ///     Rotates camera according to player's position.
    /// 
    /// </summary>
    void CreatePlayerPanel()
    {
        // Determine action needed this turn
        Board.Actions action = m_board.DetermineAction();

        // Assign basic attributes
        m_panelIcon.sprite = GetIconSprite(m_board.CurrentPlayer.Icon);
        m_panelTitle.text = m_board.CurrentPlayer.Name + "'s Turn";
        m_panelCash.text = "Cash: $" + m_board.CurrentPlayer.Cash;

        // Set the proper action window
        SetActionWindow(action);

        // Display the properties owned by the player in the properties and cards section
        ClearPropertyCardView();
        CreatePropertyCardView();
    }
    /* void CreatePlayerPanel() */

    /// <summary>
    /// 
    /// NAME
    ///     SetActionWindow - sets the proper action window.
    ///     
    /// SYNOPSIS
    ///     void SetActionWindow(Board.Actions a_action);
    ///         a_action    --> what action the current player must do.
    ///     
    /// DESCRIPTION
    ///     Based on what action is passed in, this method will use the data of the board
    ///     and current player to activate the appropriate action window in the
    ///     player panel.
    /// 
    /// </summary>
    void SetActionWindow(Board.Actions a_action)
    {
        // Deactivate any other action windows
        foreach (GameObject actionWindow in m_actionWindows)
            actionWindow.SetActive(false);

        // Assign the proper action window
        switch (a_action)
        {
            // Dice rolling
            case Board.Actions.DetermineOrder:
            case Board.Actions.RollDice:
                m_diceRollController.ResetWindow();
                m_actionWindows[ACTION_WINDOW_ROLL_DICE].SetActive(true);
                break;

            // Landed on a Utility, haven't rolled to determine cost
            case Board.Actions.DetermineUtilityCost:
                m_diceRollController.UtilityCostRoll = true;
                m_diceRollController.ResetWindow();
                m_actionWindows[ACTION_WINDOW_ROLL_DICE].SetActive(true);
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
                        m_twoChoiceActionController.RightButton.interactable = false;
                }

                // Not in jail, rolled doubles
                else if (m_board.CurrentPlayer.RolledDoubles)
                {
                    m_diceRollController.ResetWindow();
                    m_actionWindows[ACTION_WINDOW_ROLL_DICE].SetActive(true);
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
                    m_twoChoiceActionController.LeftButton.interactable = false;

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
                    "Pay: $" + m_board.GetLandedOnOwnedPropertyRent(), Color.red);
                m_genericActionController.ActButton.onClick.AddListener(Action_PayingRent);
                break;

            // Landed on a tax property
            case Board.Actions.LandedOn_Tax:
                CreateGenericActionWindow("You landed on " + m_board.GetSpace(m_board.CurrentPlayer.CurrentSpace).Name,
                    "Pay: $" + m_board.GetLandedOnTaxCost(), Color.red);
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
                throw new Exception("No action window found for this action: " + a_action.ToString());
        }
    }
    /* void SetActionWindow(Board.Actions a_action) */

    /// <summary>
    /// 
    /// NAME
    ///     CreateGenericActionWindow - creates a generic action window.
    ///     
    /// SYNOPSIS
    ///     void CreateGenericActionWindow(string a_title, 
    ///     string a_buttonText, Color a_buttonColor);
    ///         a_title         --> title of the window.
    ///         a_buttonText    --> text of the action button.
    ///         a_buttonColor   --> color of the action button.
    ///     
    ///     
    /// DESCRIPTION
    ///     Player needs to perform an action for which there is only one possible 
    ///     choice for them to make. This method initializes a window and displays it.
    /// 
    /// </summary>
    void CreateGenericActionWindow(string a_title, string a_buttonText, Color a_buttonColor)
    {
        // Set text attributes
        m_genericActionController.Title = a_title;
        m_genericActionController.ActButtonText = a_buttonText;
        m_genericActionController.ActButtonColor = a_buttonColor;

        // Set the window to active 
        m_actionWindows[ACTION_WINDOW_GENERIC].gameObject.SetActive(true);

        // Clear listeners 
        m_genericActionController.ResetListeners();
    }

    /// <summary>
    /// 
    /// NAME
    ///     CreateTwoChoiceActionWindow - creates a two action window.
    ///     
    /// SYNOPSIS
    ///     void CreateTwoChoiceActionWindow(string a_title, string a_leftButtonText, 
    ///     Color a_leftButtonColor, string a_rightButtonText, Color a_rightButtonColor);
    ///         a_title             --> title of the window.
    ///         a_leftButtonText    --> text of the left action button.
    ///         a_leftButtonColor   --> color of the left action button.
    ///         a_rightButtonText   --> text of the right action button.
    ///         a_rightButtonColor  --> color of the right action button.
    ///     
    /// DESCRIPTION
    ///     Player has an action to make with two possible choices they can make,
    ///     this method initializes a window with two buttons to represent 
    ///     each choice for them, and shows it.
    /// 
    /// </summary>
    void CreateTwoChoiceActionWindow(string a_title, string a_leftButtonText, Color a_leftButtonColor, 
        string a_rightButtonText, Color a_rightButtonColor)
    {
        // Clear listeners 
        m_twoChoiceActionController.LeftButton.onClick.RemoveAllListeners();
        m_twoChoiceActionController.RightButton.onClick.RemoveAllListeners();

        // Reactivate buttons
        m_twoChoiceActionController.LeftButton.interactable = true;
        m_twoChoiceActionController.RightButton.interactable = true;

        // Set text
        m_twoChoiceActionController.Title = a_title;
        m_twoChoiceActionController.LeftButtonText = a_leftButtonText;
        m_twoChoiceActionController.RightButtonText = a_rightButtonText;
        m_twoChoiceActionController.LeftButtonColor = a_leftButtonColor;
        m_twoChoiceActionController.RightButtonColor = a_rightButtonColor;

        // Set window active
        m_actionWindows[ACTION_WINDOW_TWO_CHOICE].SetActive(true);
    }
    /* void CreateTwoChoiceActionWindow(string a_title, string a_leftButtonText, Color a_leftButtonColor, 
        string a_rightButtonText, Color a_rightButtonColor) */

    /// <summary>
    /// 
    /// NAME
    ///     ClearPropertyCardView - clears the property window for new player.
    ///     
    /// DESCRIPTION
    ///     This method removes all the images of properties and cards in the
    ///     property manager window. Usually when a new player is up.
    /// 
    /// </summary>
    void ClearPropertyCardView()
    {
        // Obtain all the property views
        RawImage[] propertyViews = m_propertyCardContent.GetComponentsInChildren<RawImage>();

        // Destroy them
        foreach (RawImage propertyView in propertyViews)
            Destroy(propertyView.gameObject);

        // Obtain all the card views
        Image[] cardImages = m_propertyCardContent.GetComponentsInChildren<Image>();

        // Destroy them
        foreach (Image cardImage in cardImages)
            Destroy(cardImage.gameObject);
    }
    /* void ClearPropertyCardView()*/

    /// <summary>
    /// 
    /// NAME
    ///     CreatePropertyCardView - creates the property and card view.
    ///     
    /// DESCRIPTION
    ///     In the player panel, the bottom window labeled "Properties and Cards",
    ///     displays those items. This method is what creates that display.
    ///     It uses render textures and sprites of the properties and cards,
    ///     and also adds button's to the properties for the user to interact with 
    ///     to open the property manager menu. Render textures use camera views of 
    ///     each property, which are live and display the actual current property 
    ///     on the board in real time.
    /// 
    /// </summary>
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
    /* void CreatePropertyCardView() */

    /// <summary>
    /// 
    /// NAME
    ///     OrientCamera - orients camera for player.
    ///     
    /// DESCRIPTION
    ///     According to the current player's position on the board, this method
    ///     will orient the camera so they are right-side up.
    /// 
    /// </summary>
    void OrientCamera()
    {
        int currentSpace = m_board.CurrentPlayer.CurrentSpace;
        if (currentSpace >= 0 && currentSpace <= 9)
            m_cameraController.SetCameraRotation(0);
        else if (currentSpace >= 10 && currentSpace <= 19)
            m_cameraController.SetCameraRotation(270);
        else if (currentSpace >= 20 && currentSpace <= 29)
            m_cameraController.SetCameraRotation(180);
        else
            m_cameraController.SetCameraRotation(90);
    }

    /// <summary>
    /// 
    /// NAME
    ///     InitializePlayerIcons - initializes the player icons.
    ///     
    /// DESCRIPTION
    ///     Utilizes the player track controller class to create 
    ///     lanes which the player icons will follow for the duration of the game.
    ///     This method also figures out which players get which icon, and 
    ///     moves all players to the Go space.
    /// 
    /// </summary>
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
    /* void InitializePlayerIcons() */

    /// <summary>
    /// 
    /// NAME
    ///     InitializePlayerIcons - initializes the player icons.
    ///     
    /// DESCRIPTION
    ///     Iterates through each color property, and removes all 
    ///     houses on it. This is required at the start of the game,
    ///     because the board starts with all houses and hotels enabled.
    /// 
    /// </summary>
    void EraseAllHousesAndHotels()
    {
        // Each space
        foreach (Button spaceButton in m_spaceButtons)
        {
            // Check it's a color property 
            int spaceNum = int.Parse(spaceButton.name);

            // Break if beyond board spaces
            if (spaceNum > 39)
                break;

            if (m_board.GetSpace(spaceNum) is ColorProperty)
            {
                // Deactivate each house
                for (int i = 0; i < 5; i++)
                    FindHouseOrHotelIcon(spaceNum, i + 1).SetActive(false);
            }
        }
    }

    /// <summary>
    /// 
    /// NAME
    ///     FindHouseOrHotelIcon - returns reference to specified 
    ///                             property's house or hotel.
    ///     
    /// DESCRIPTION
    ///     Finds the icon on the board for a specified property's 
    ///     house or hotel. This takes advantage of the naming convention
    ///     applied in the Unity scene.
    /// 
    /// RETURNS
    ///     Reference to icon on board of the specific house searched for.
    /// 
    /// </summary>
    GameObject FindHouseOrHotelIcon(int a_propertyNum, int a_houseNum)
    {
        // Determine icon name based on num
        string houseName = "house" + a_houseNum;
        if (a_houseNum == 5)
            houseName = "hotel";

        // Find the parent transform (property object in the scene)
        Transform propertyTransform = FindSpaceButtonParent(m_spaceButtons[a_propertyNum]);

        // Find the house object 
        Transform houseTransform = FindChildByName(propertyTransform, houseName);
        return houseTransform.gameObject;
    }

    /// <summary>
    /// 
    /// NAME
    ///     SaveEndGameData - saves game data relevent for the final 
    ///                       game scene.
    ///     
    /// DESCRIPTION
    ///     The game has ended, and before displaying the final end 
    ///     game scene, this method will save relevent data of the winning
    ///     player to a text file, for that scene to load and display.
    /// 
    /// </summary>
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
                propertiesList += ", ";

            i++;
        }
        data.Add(propertiesList);

        // Write all the data
        File.WriteAllLines(filePath, data);
    }
    /* void SaveEndGameData() */

    // Returns the property parent object of a given space button
    Transform FindSpaceButtonParent(Button a_spaceButton)
    {
        Transform UITranform = a_spaceButton.transform.parent;
        return UITranform.parent;
    }

    /// <summary>
    /// 
    /// NAME
    ///     FindChildByName - returns a child object by name, given parent.
    ///     
    /// SYNOPSIS
    ///     Transform FindChildByName(Transform a_parent, string a_childName);
    ///         a_parent        --> parent Transform object.
    ///         a_childName     --> name of the child.
    ///     
    /// DESCRIPTION
    ///     Searches through all the children of a specified parent, 
    ///     to find where the specified child name matches the actual
    ///     name of the child object. Returns the child object.
    /// 
    /// RETURNS
    ///     Transform object of child being searched for.
    ///     
    /// EXCEPTION
    ///     Throws exception if the child is not found.
    /// 
    /// </summary>
    Transform FindChildByName(Transform a_parent, string a_childName)
    {
        Transform[] allChildren = a_parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name == a_childName)
                return child;
        }
        throw new Exception("Child not found by name!");
    }

    // Returns icon sprite with the specified name, throws exception if not found
    Sprite GetIconSprite(string a_iconName)
    {
        foreach (Sprite icon in m_icons)
        {
            if (a_iconName == icon.name)
                return icon;
        }
        throw new Exception("Icon not found!");
    }

    /// <summary>
    /// 
    /// NAME
    ///     FindPropertyTexture - returns the render texture associated with 
    ///                           a particular space.
    ///     
    /// SYNOPSIS
    ///     RenderTexture FindPropertyTexture(int a_spaceIndex);
    ///         a_spaceIndex        --> space location on the board.
    ///     
    /// DESCRIPTION
    ///     Searches through all the render textures in the game, 
    ///     finds a match with the space index based on the name
    ///     convention used in Unity.
    /// 
    /// RETURNS
    ///     Render texture of the specified space.
    ///     
    /// EXCEPTION
    ///     Throws exception if the texture is not found.
    /// 
    /// </summary> 
    RenderTexture FindPropertyTexture(int a_spaceIndex)
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
            if (index == a_spaceIndex)
                return texture;
        }

        // No space found, FREAK OUT!
        throw new Exception("No texture found for specified property! Index: " + a_spaceIndex);
    }
    /* RenderTexture FindPropertyTexture(int a_spaceIndex) */
}
