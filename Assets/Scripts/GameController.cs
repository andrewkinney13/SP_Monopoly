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
    
    public List<Button> m_spaceButtons;
    public List<Button> m_playerButtons;
    public DetailsPopupController m_spaceDetailsController;
    public DetailsPopupController m_playerDetailsController;
    public PlayerTrackController m_playerTrackController;
    public List<Sprite> m_icons;


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

        // Initialize player buttons
        m_playerTrackController.CreateLanes();
        int i = 0;
        foreach (Player player in m_board.Players())
        {
            // Assign the function
            int playerIndex = i;
            m_playerButtons[i].onClick.AddListener(() => OnPlayerClick(playerIndex));

            // Assign the icon
            m_playerButtons[i].image.sprite = GetIconImage(player.Icon);

            // Move the player to space 0
            StartCoroutine(m_playerTrackController.MovePlayer(i, 0, 0));
            i++;
        }
    }

    // Update is called once per frame
    void Update() 
    {
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

    // When user clicks a player
    void OnPlayerClick(int playerIndex)
    {
        // Get the space info
        string spaceDescription = "Hey this is a player!";

        // Display it in the space details window, where the user clicked
        m_playerDetailsController.CreateDetailsWindow(playerIndex.ToString(), spaceDescription);
    }

    // Returns the image associated with a partular icon
    Sprite GetIconImage(string iconName)
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
