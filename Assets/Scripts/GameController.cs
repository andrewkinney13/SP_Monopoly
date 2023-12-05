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
    private int m_turnNum;    // whose turn it is

    public List<Button> m_spaceButtons;
    public List<Button> m_playerButtons;
    public List<Sprite> m_icons;

    public DetailsPopupController m_spaceDetailsController;
    public DetailsPopupController m_playerDetailsController;
    public PlayerTrackController m_playerTrackController;
    public PlayerPanelController m_playerPanelController;

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
    }

    // Update is called once per frame
    void Update()
    {
        // Obtain current player data
        string name = m_board.Players[m_turnNum].Name;
        float cash = m_board.Players[m_turnNum].Cash;
        PlayerPanelController.Actions action = PlayerPanelController.Actions.EndTurn;

        // If attributes of the player change, redraw the player panel
        if (m_playerPanelController.NeedsUpdate(name, cash, action))
        {
            // Obtain arguements to pass to panel creator
            Sprite icon = GetIconSprite(m_board.Players[m_turnNum].Icon);

            // Create the player panel so player can do their turn
            m_playerPanelController.CreatePlayerPanel(icon, name, cash, action);
        }

        // Check to see if the turn num
        if (m_playerPanelController.TurnEnded)
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
            m_playerPanelController.TurnEnded = false;
        }

        // Erase the space details window if user clicks
        if (Input.GetMouseButtonDown(0))
        {
            m_spaceDetailsController.CloseDetailsWindow();
            m_playerDetailsController.CloseDetailsWindow();
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

    // When user clicks a player
    void OnPlayerClick(int playerNum)
    {
        // Get the space info
        string spaceDescription = "Hey this is a player!";

        // Display it in the space details window, where the user clicked
        m_playerDetailsController.CreateDetailsWindow(playerNum.ToString(), spaceDescription);
    }

}
