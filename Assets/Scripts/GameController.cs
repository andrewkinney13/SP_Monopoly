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
    public Canvas m_boardCanvas;
    public Canvas m_screenUICanvas;
    public List<Button> m_spaceButtons;
    public SpaceDetailsController m_spaceDetailsWindow;
    private Board m_board;
    
    // Runs when the script is initialized, using this as a constructor
    void Start()
    {

        // Assign space button functions
        foreach (Button button in m_spaceButtons)
        {
            button.onClick.AddListener(() => OnSpaceClick(int.Parse(button.name)));
        }
        
        // Initialize the board
        List<Player> players = GetPlayers();
        m_board = new Board();
        m_board.InitializeBoard();

    }

    // Update is called once per frame
    void Update() { }

    // Obtains the players
    public List<Player> GetPlayers()
    {
        List<Player> players = new List<Player>();
        Player andrew = new Player("Andrew");
        players.Add(andrew);
        Player kwas = new Player("Kwas");
        players.Add(kwas);
        Player max = new Player("Max");
        players.Add(max);
        Player bmac = new Player("Bmac");
        players.Add(bmac);
        return players;
    }

    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
        // Get the space info
        string spaceName = m_board.GetSpace(spaceIndex).Name;
        string spaceDescription = m_board.GetSpace(spaceIndex).Description;
        string spaceActionText = m_board.GetSpace(spaceIndex).ActionText;

        // Display it in the space details window
        m_spaceDetailsWindow.SetSpaceDetailsWindow(spaceName, spaceDescription, 
            m_board.GetSpace(spaceIndex).Action, spaceActionText);
    }
}
