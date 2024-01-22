using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Handles the board objects
public class GameController : MonoBehaviour
{
    // Data Members
    private Board m_board;     
    private bool m_updateMade = false;

    // Popup window
    public PopupController m_popupController;

    // Buttons the user can click to get info about stuff
    public List<Button> m_spaceButtons;
    public List<Button> m_playerButtons;

    // Controller classes
    public DetailsPopupController m_spaceDetailsController;
    public DetailsPopupController m_playerDetailsController;
    public PlayerTrackController m_playerTrackController;
    public CameraController m_cameraController;
    
    public Action_RollDice m_diceRollController;
    public Action_LO_UnownedProperty m_LO_unownedPropertyController;

    public Action_Generic m_genericActionController;
    // public Action_TwoChoice m_twoChoiceActionController;

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

    // Runs when the script is initialized, using this as a constructor
    void Start()
    {
        // Assign space button functions
        foreach (Button button in m_spaceButtons)
        {
            button.onClick.AddListener(() => OnSpaceClick(int.Parse(button.name)));
        }

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

    // Update is called once per frame
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
        }

        // Erase the space details window if user clicks
        if (Input.GetMouseButtonDown(0))
        {
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
        }

    }

    // Updates cash display
    void UpdatePanelCash()
    {
        m_panelCash.text = "Cash: $" + m_board.CurrentPlayer.Cash.ToString();
    }
    

    // Getters and setters
    public void UpdateMade()
    {
        // Mark update made
        m_updateMade = true;
    }


    // Creates the player panel for the player to do actions during their turn
    private void CreatePlayerPanel()
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
                if (m_board.CurrentPlayer.InJail)
                {
                    CreateGenericActionWindow("You must pay to be released from Jail...", "Pay $75", Color.red);
                    m_genericActionController.ActButton.onClick.AddListener(Action_GetOutOfJail);
                }
                else if (m_board.CurrentPlayer.RolledDoubles)
                {
                    m_actionWindows[1].SetActive(true);
                    
                }
                else
                {
                    CreateGenericActionWindow("No actions left to complete", "End Turn", Color.black);
                    m_genericActionController.ActButton.onClick.AddListener(Action_EndTurn);
                }
                break;

            // Landed on an onowned property 
            case Board.Actions.LandedOn_UnownedProperty:
                m_LO_unownedPropertyController.Title = m_board.GetLandedOnUnownedPropertyTitle();
                m_actionWindows[2].SetActive(true);
                break;

            // Landed on an owned property
            case Board.Actions.LandedOn_OwnedColorProperty:
            case Board.Actions.LandedOn_OwnedRailroad:
            case Board.Actions.LandedOn_OwnedUtility:
                CreateGenericActionWindow(m_board.GetLandedOnOwnedPropertyTitle(), "Pay: " + (-1 * m_board.GetLandedOnOwnedPropertyRent()), Color.red);
                m_genericActionController.ActButton.onClick.AddListener(Action_PayingRent);
                break;

            // Landed on a tax property
            case Board.Actions.LandedOn_Tax:
                CreateGenericActionWindow(m_board.GetLandedOnTaxTitle(), "Pay: $" + (-1 * m_board.GetLandedOnTaxCost()), Color.red);
                m_genericActionController.ActButton.onClick.AddListener(Action_PayingTax);
                break;

            // Ending turn / default (error)
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
    private void CreateGenericActionWindow(string title, string buttonText, Color buttonColor)
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

    // Clears all the property and card view contents and resets the size
    private void ClearPropertyCardView()
    {
        // Obtain all the property views
        RawImage[] propertyViews = m_propertyCardContent.GetComponentsInChildren<RawImage>();

        // Destroy them
        foreach (RawImage propertyView in propertyViews) 
        {
            Destroy(propertyView.gameObject);
        }
    }

    // Adds a new property to the properties and cards section
    private void CreatePropertyCardView()
    {
        // Set the sizes
        float propertyWidth = 224f;
        float propertyHeight = 300f;
        float startX = -2590f;

        // Loop through owned properties
        int propertyNum = 0;
        foreach (Space property in  m_board.CurrentPlayer.Properties) 
        {
            // Create object with viewer as child of properties content
            GameObject newPropertyImage = new GameObject("RawImage");
            newPropertyImage.transform.SetParent(m_propertyCardContent.transform);
            RawImage newViewer = newPropertyImage.AddComponent<RawImage>();
            newViewer.transform.localScale = new Vector2(1,1);

            // Set size and position
            RectTransform newViewerRect = newPropertyImage.GetComponent<RectTransform>();
            newViewerRect.sizeDelta = new Vector2(propertyWidth, propertyHeight);
            float xOffset = startX + 20 + propertyWidth * propertyNum + 20 * propertyNum;
            newViewerRect.anchoredPosition = new Vector2(xOffset, 0);

            // Assign property render texture to the viewer
            newViewer.texture = FindPropertyTexture(property.Index);

            // Add button and listener for the space
            Button viewerButton = newViewer.AddComponent<Button>();
            viewerButton.onClick.AddListener(() => OnPropertyClick(property.Index)); 
            
            // Increment property
            propertyNum++;
        }
    }

    // Player ended their turn
    public void Action_EndTurn()
    {
        // End the current players turn
        m_board.CurrentPlayer.TurnCompleted = true;
    }

    // Player rolled dice to determine their action
    public void Action_OrderDetermined(int diceResult)
    {
        // Update the players attributes
        m_board.OrderDetermined(diceResult);

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

    // Player rolled dice to determine the cost of landing on a utility
    public void Action_UtilityCostDetermined(int diceRoll)
    {
        // Update flags and properties on space and controller
        m_diceRollController.UtilityCostRoll = false;
        m_board.UtilityCostDetermined(diceRoll);

        // Mark update made
        UpdateMade();
    }

    // Player rolled the dice
    public void Action_DiceRolled(int diceResult, bool wereDoubles)
    {
        // Update board
        int currentSpace = m_board.CurrentPlayer.CurrentSpace;
        m_board.DiceRolled(diceResult, wereDoubles);
        int destinationSpace = m_board.CurrentPlayer.CurrentSpace;

        // Move the player icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, currentSpace, destinationSpace));

        // Post go message if passing it, not on way to jail though!
        if (currentSpace > destinationSpace) 
        {
            CreateGenericActionWindow("You passed Go!", "Collect $200", Color.green);
            m_genericActionController.ActButton.onClick.AddListener(Action_CollectGo);
            return;
        }

        // Update was made
        UpdateMade();
    }

    // Player buying property
    public void Action_BuyingProperty(bool buying)
    {
        // Call Board function
        if (buying)
        {
            m_board.PropertyPurchase();
        }

        // Completed space action
        m_board.CurrentPlayer.SpaceActionCompleted = true;
        UpdateMade();
    }

    // Player paying rent for a color property
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

    // Player paying tax
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

    // Player going to jail
    public void Action_GoToJail()
    {
        // Move the player's icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, m_board.CurrentPlayer.CurrentSpace, 10));

        // Update board
        m_board.GoToJail();

        // Update made
        UpdateMade();
    }

    // Player getting out of jail
    public void Action_GetOutOfJail()
    {
        // Update board
        m_board.GetOutOfJail();

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

    // Player going bankrupt
    public void Action_GoingBankrupt()
    {
        // Update board
        m_board.GoingBankrupt();

        // Check if game over (all but one players bankrupt)
        if (m_board.GameOver())
        {
            // Open end game scene after saving data
            m_board.SaveEndGameData();
            SceneManager.LoadScene("End Game");
        }

        // End their turn
        Action_EndTurn();
    }

    // PLayer collecting $200
    public void Action_CollectGo()
    {
        // User collects 200
        m_board.CurrentPlayer.Cash += 200;

        // Update cash
        UpdatePanelCash();

        // Completed space action
        m_board.CurrentPlayer.SpaceActionCompleted = true;

        // Update the panel
        UpdateMade();
    }

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
        if (m_board.GetSpace(spaceIndex).ActionType == Board.Actions.LandedOn_OwnedColorProperty)
        {
            // Cast to inherited type so we can obtain color property specific values
            ColorProperty colorProperty = (ColorProperty)property;

            // Determine if player can purchase and sell houses or hotels
            Player player = m_board.CurrentPlayer;
            bool houseAvailible = m_board.HouseAvailible(player, colorProperty);
            bool hotelAvailible = m_board.HotelAvailible(player, colorProperty);
            bool sellHouseAvailible = m_board.SellHouseAvailible(player, colorProperty);
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

    // Property manager button functions
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


    // When user clicks a player
    void OnPlayerClick(int playerNum)
    {
        // Display it in the space details window, where the user clicked
        m_playerDetailsController.CreateDetailsWindow(m_board.GetPlayerName(playerNum), m_board.GetPlayerDescription(playerNum));
    }

    // Initializes the player lanes and icons 
    public void InitializePlayerIcons()
    {
        m_playerTrackController.CreateLanes();
        m_playerTrackController.SetIcons(m_playerButtons);
        for (int playerNum = 0; playerNum < m_board.PlayerCount; playerNum++) 
        {
            // Create local player num
            int localPlayerNum = playerNum;

            // Add the onClick function
            m_playerButtons[playerNum].onClick.AddListener(() => OnPlayerClick(localPlayerNum));

            // Assign the icon
            m_playerButtons[playerNum].image.sprite = GetIconSprite(m_board.GetPlayerIconName(playerNum));

            // Move the player to space 0
            StartCoroutine(m_playerTrackController.MovePlayer(playerNum, 0, 0));
        }
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
    private RenderTexture FindPropertyTexture(int spaceIndex)
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
        throw new System.Exception("No texture found for specified property! Index: " + spaceIndex);
    }

    // Deactivates all houses (for game start)
    private void EraseAllHousesAndHotels()
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
    private GameObject FindHouseOrHotelIcon(int propertyNum, int houseNum)
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

    // Returns the property parent object of a given space button
    private Transform FindSpaceButtonParent(Button spaceButton)
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
}


