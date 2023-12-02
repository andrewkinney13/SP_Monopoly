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
    public SpaceDetailsController m_spaceDetailsController;    
    
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
    }

    // Update is called once per frame
    void Update() 
    {
        // Erase the space details window if user clicks
        if (Input.GetMouseButtonDown(0))
        {
            m_spaceDetailsController.CloseSpaceDetailsWindow();
        }
    }


    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
        // Get the space info
        string spaceName = m_board.GetSpace(spaceIndex).Name;
        string spaceDescription = m_board.GetSpace(spaceIndex).Description;

        // Obtain the cursor location
        Vector2 mousePosition = Input.mousePosition;
        Debug.Log(mousePosition);

        // Display it in the space details window, where the user clicked
        m_spaceDetailsController.CreateSpaceDetailsWindow(spaceName, spaceDescription, mousePosition);
    }
}
