using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles the board objects
public class GameController : MonoBehaviour
{
    // Data Members
    private Board m_board;
    private int m_turnNum;    
    private bool m_turnCompleted = false;       
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

    // Folder of icons a player can have as their game token
    public List<Sprite> m_icons;
    
    // Player panel components
    public Image m_panelIcon;
    public TMP_Text m_panelTitle;
    public TMP_Text m_panelCash;
    public List<GameObject> m_actionWindows;
    public RectTransform m_propertiesContent;

    // Possible actions a player may have to take during their turn
    public enum Actions
    {
        EndTurn = 0,
        DetermineOrder = 1,
        RollDice = 2,
        UnownedProperty = 3,
        OwnedColorProperty = 4,
        OwnedUtility = 5,
        OwnedRailroad = 6,
        ChanceOrCommunityChest = 7,
        VisitingJail = 8,
        FreeParking = 9,
        GoToJail = 10,
        Tax = 11
    }

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

        // Set the turn for first person in list (for now)
        m_turnNum = 0;

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
            // Obtain current player data
            string name = m_board.Players[m_turnNum].Name;
            float cash = m_board.Players[m_turnNum].Cash;

            // Determine action needed this turn
            Actions action = DetermineAction();

            // Obtain arguements to pass to panel creator
            Sprite icon = GetIconSprite(m_board.Players[m_turnNum].Icon);

            // Create the player panel so player can do their turn
            CreatePlayerPanel(icon, name, cash, action);

            // Set bool to false now that update has been made
            m_updateMade = false;
        }

        // Update whose turn it is if turn completed
        if (m_turnCompleted)
        {
            // Update whose turn is is
            if (m_turnNum < m_board.Players.Count - 1)
            {
                m_turnNum++;
            }
            else
            {
                m_turnNum = 0;
            }
            m_turnCompleted = false;
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
    public bool TurnCompleted
    {
        get { return m_turnCompleted; }
        set { m_turnCompleted = value; }
    }
    public bool UpdateMade
    {
        get { return m_updateMade; }
        set { m_updateMade = value; }
    }
    public Player CurrentPlayer
    {
        get { return m_board.Players[m_turnNum]; }
    }


    // Creates the player panel for the player to do actions during their turn
    public void CreatePlayerPanel(Sprite icon, string name, float cash, Actions action)
    {
        // Assign basic attributes
        m_panelIcon.sprite = icon;
        m_panelTitle.text = name + "'s Turn";
        m_panelCash.text = "Cash: $" + cash;

        // Deactivate any other action windows
        foreach (GameObject actionWindow in m_actionWindows)
        {
            actionWindow.SetActive(false);
        }

        // Get the proper window
        GameObject currentActionWindow = m_actionWindows[(int)action];

        // Set the window as active
        currentActionWindow.gameObject.SetActive(true);
    }

    
    // Returns what action needs to be taken by the current player
    public Actions DetermineAction()
    {
        // Check to see if player needs their turn initialized
        if (!m_board.Players[m_turnNum].TurnInitialized)
        {
            return Actions.DetermineOrder;
        }

        /*// 
        if (!m_board.Players[m_turnNum].RolledDice)
        {
            return Actions.RollDice;
        }*/

        // Check if the turn order has been determined
        return Actions.EndTurn;
    }

    // Called by determine order action
    public void Action_OrderDetermined(int diceResult)
    {
        // Mark the player has having their turn initialized
        CurrentPlayer.CurrentSpace = diceResult;
        CurrentPlayer.TurnInitialized = true;

        // If this was the last player, all players were initialized
        if (CurrentPlayer.PlayerNum == m_board.Players.Count - 1)
        {
            // Sort the players 
            m_board.Players.Sort(SortPlayers);

            // Iterate through the players to update their properties and create output message
            string popUpMessage = "";
            int playerNum = 0;
            foreach (Player player in m_board.Players)
            {
                // Reinit player num w/ new order
                player.PlayerNum = playerNum;
                

                // Append output for popup
                popUpMessage += (playerNum + 1) + ": " + player.Name + ", rolled " + player.CurrentSpace + "\n";

                // Reset their space to Go
                player.CurrentSpace = 0;

                // Append what player we're on
                playerNum++;
            }

            // Initialize the icons now with proper order 
            InitializePlayerIcons();

            // Show a popup that players have been intialized
            m_popupController.CreatePopupWindow("Order Determined!", popUpMessage);
        }

        // Mark update was made
        m_updateMade = true;
    }

    // Sorts players based on their current space, as determined by a dice roll
    static int SortPlayers(Player player1, Player player2)
    {
        if (player1.CurrentSpace < player2.CurrentSpace) 
        {
            return 1;
        }
        else if (player1.CurrentSpace > player2.CurrentSpace)
        {
            return -1;
        }
        else 
        { 
            return 0; 
        }
    }

    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
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
        m_playerDetailsController.CreateDetailsWindow(m_board.Players[playerNum].Name, m_board.Players[playerNum].Description);
    }

    // Initializes the player lanes and icons 
    public void InitializePlayerIcons()
    {
        m_playerTrackController.CreateLanes();
        m_playerTrackController.SetIcons(m_playerButtons);
        foreach (Player player in m_board.Players)
        {
            m_playerButtons[player.PlayerNum].onClick.AddListener(() => OnPlayerClick(player.PlayerNum));

            // Assign the icon
            m_playerButtons[player.PlayerNum].image.sprite = GetIconSprite(player.Icon);

            // Move the player to space 0
            StartCoroutine(m_playerTrackController.MovePlayer(player.PlayerNum, 39, 0));
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


