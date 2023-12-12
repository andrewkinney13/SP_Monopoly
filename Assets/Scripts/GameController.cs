using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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

    // Folder of icons a player can have as their game token
    public List<Sprite> m_icons;
    
    // Player panel components
    public Image m_panelIcon;
    public TMP_Text m_panelTitle;
    public TMP_Text m_panelCash;
    public List<GameObject> m_actionWindows;
    public RectTransform m_propertiesContent;

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
        }

        // Erase the space details window if user clicks
        if (Input.GetMouseButtonDown(0))
        {
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
        }
    }

    // Getters and setters
    public bool UpdateMade
    {
        get { return m_updateMade; }
        set { m_updateMade = value; }
    }


    // Creates the player panel for the player to do actions during their turn
    public void CreatePlayerPanel()
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
        GameObject currentActionWindow;
        switch (action)
        {
            case Board.Actions.DetermineOrder:
            case Board.Actions.RollDice:
                currentActionWindow = m_actionWindows[1];
                break;
            case Board.Actions.EndTurn:
                currentActionWindow = m_actionWindows[0];
                break;
            default:
                throw new System.Exception("No window set for action...");
        }

        // Set the window as active
        currentActionWindow.gameObject.SetActive(true);
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

    // Player rolled the dice
    public void Action_DiceRolled(int diceResult, bool wereDoubles)
    {
        // Update board
        int currentSpace = m_board.CurrentPlayer.CurrentSpace;
        m_board.DiceRolled(diceResult, wereDoubles);
        int destinationSpace = m_board.CurrentPlayer.CurrentSpace;

        Debug.Log(m_board.CurrentPlayer.Name);
        Debug.Log(m_board.CurrentPlayer.PlayerNum);

        // Move the player icon
        StartCoroutine(m_playerTrackController.MovePlayer(m_board.CurrentPlayer.PlayerNum, currentSpace, destinationSpace));

        // Update was made
        UpdateMade = true;
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
            StartCoroutine(m_playerTrackController.MovePlayer(playerNum, 39, 0));
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
}


