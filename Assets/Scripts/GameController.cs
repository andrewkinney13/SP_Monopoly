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
    private bool m_panelUpdateNeeded = false;

    // Popup window
    public PopupController m_popupController;

    // Buttons the user can click to get info about stuff
    public List<Button> m_spaceButtons;
    public List<Button> m_playerButtons;

    // Controller classes for buttons above
    public DetailsPopupController m_spaceDetailsController;
    public DetailsPopupController m_playerDetailsController;

    // Controller class for player icon movememnt
    public PlayerTrackController m_playerTrackController;

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
        m_panelUpdateNeeded = true;

        // Close popup window
        m_popupController.ClosePopupWindow();
    }

    // Update is called once per frame
    void Update()
    {
        // If attributes of the player change, redraw the player panel
        if (m_panelUpdateNeeded)
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
            m_panelUpdateNeeded = false;
        }

        // Check to see if the turn num
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
            m_panelUpdateNeeded = true;
        }

        // Erase the space details window if user clicks
        if (Input.GetMouseButtonDown(0))
        {
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
        }
    }

    // Creates the player panel for the player to do actions during their turn
    public void CreatePlayerPanel(Sprite icon, string name, float cash, Actions action)
    {
        // Assign basic attributes
        m_panelIcon.sprite = icon;
        m_panelTitle.text = name + "'s Turn";
        m_panelCash.text = "Cash: $" + cash;

        // Assign action window
        AssignActionWindow(action);
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
        // Get the space info
        string spaceDescription = "Hey this is a player!";

        // Display it in the space details window, where the user clicked
        m_playerDetailsController.CreateDetailsWindow(playerNum.ToString(), spaceDescription);
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
            StartCoroutine(m_playerTrackController.MovePlayer(player.PlayerNum, 0, 0));
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

    // Returns what action needs to be taken by the current player
    public Actions DetermineAction()
    {
        // Check to see if player needs their turn initialized
        if (!m_board.Players[m_turnNum].TurnInitialized)
        {
            return Actions.DetermineOrder;
        }

        // Check if the turn order has been determined
        return Actions.EndTurn;
    }

    // Assigns the game action window according to what action the user needs to complete
    public void AssignActionWindow(Actions action)
    {
        // Deactivate any other action windows
        foreach (GameObject actionWindow in m_actionWindows)
        {
            actionWindow.SetActive(false);
        }

        // Get the window
        GameObject currentActionWindow = m_actionWindows[(int)action];

        // Set the window as active
        currentActionWindow.gameObject.SetActive(true);

        // Assign attributes of window depending on action
        switch (action)
        {
            case Actions.DetermineOrder:
            
                // Obtain the dice buttons
                Button dice = GameObject.Find("Dice Button").GetComponent<Button>();
                Image die1 = GameObject.Find("Die 1").GetComponent<Image>();
                Image die2 = GameObject.Find("Die 2").GetComponent<Image>();

                // Assign determine order function
                dice.onClick.AddListener(() => DetermineOrder(die1, die2));

                break;
            
            case Actions.EndTurn:
            
                // Assign end button function
                Button endTurn = GameObject.Find("End Turn Button").GetComponent<Button>();
                endTurn.onClick.AddListener(EndTurn);
                break;
        }
    }

    // All players roll dice, players go in order of the result of their dice roll
    void DetermineOrder(Image die1, Image die2)
    {
        // They roll the dice
        m_board.Players[m_turnNum].CurrentSpace = Random.Range(2, 12);

        // Update their turn initialization status
        m_board.Players[m_turnNum].TurnInitialized = true;

        // If all players have rolled to determine their order, we can now set the actual turn orders
        if (m_turnNum == m_board.Players.Count - 1)
        {
            // Sort players
            m_board.Players.Sort(SortPlayers);

            // Create a pop up to tell the users that the order of players has been determined
            m_popupController.CreatePopupWindow("Test", "Test");
        }

        m_panelUpdateNeeded = true;
    }

    // Sorts players based on their current space, as determined by a dice roll
    static int SortPlayers(Player player1, Player player2)
    {
        if (player1.CurrentSpace < player2.CurrentSpace) 
        {
            return -1;
        }
        else if (player1.CurrentSpace > player2.CurrentSpace)
        {
            return 1;
        }
        else 
        { 
            return 0; 
        }
    }

    void EndTurn()
    {
        m_turnCompleted = true;
    }

}


