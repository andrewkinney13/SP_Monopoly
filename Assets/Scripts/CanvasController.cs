using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles the board objects
public class CanvasController : MonoBehaviour
{
    // Data Members
    public Canvas m_boardCanvas;
    public Canvas m_screenUICanvas;

    private List<Button> m_spaceButtons = new List<Button>();
    private List<GameObject> m_spaceGameObjects = new List<GameObject>();
    private Board m_board;
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board

    // Runs when the script is initialized, using this as a constructor
    void Start()
    {
        // Initialize the buttons and game objects for this class
        InitializeObjects();

        // Initialize the board
        List<Player> players = GetPlayers();
        m_board = new Board(players);
        m_board.InitializeBoard();
    }

    // Update is called once per frame
    void Update() { }

    // Initializes a list of space objects based on data found in Hiararchy window of Unity Project
    private void InitializeObjects()
    {
        // Find the spaces folder
        Transform spacesTransform = m_boardCanvas.transform.Find("Spaces");

        // Find all the buttons that represent spaces
        m_spaceButtons.AddRange(spacesTransform.GetComponentsInChildren<Button>());

        // Sort the buttons by position on the board
        m_spaceButtons.Sort((button1, button2) =>
        {
            int num1 = int.Parse(button1.gameObject.name);
            int num2 = int.Parse(button2.gameObject.name);
            return num1.CompareTo(num2);
        });

        // Assign space button functions
        int index = 0;
        foreach (Button button in m_spaceButtons)
        {
            int buttonIndex = index;
            button.onClick.AddListener(() => OnSpaceClick(buttonIndex));
            index++;
        }

        // Obtain the gameobject associated with each button
        List<string> spaceNames = new List<string>();
        for (int i = 0; i < SPACE_NUM; i++)
        {
            // Obtain the space game object, which is the grandparent folder of it's space button
            Transform grandparentFolder = m_spaceButtons[i].transform.parent.parent;
            GameObject spaceGameObject = GameObject.Find(grandparentFolder.name);

            // Add the gameobject to the list and the name of the space
            m_spaceGameObjects.Add(spaceGameObject);
        }
    }

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
        // Display property info of a space
        UpdatePropertyDetails(spaceIndex);
    }

    
    // Updates the propery details window within the screen UI
    private void UpdatePropertyDetails(int spaceIndex)
    {
        // Find the Property Details textboxes
        Transform[] propertyDetailsFolder = m_screenUICanvas.GetComponentsInChildren<Transform>(true);

        // Set each textbox accordingly
        foreach (Transform child in propertyDetailsFolder) 
        {
            // Set the name text box
            if (child.name == "Name")
            {
                TextMeshProUGUI textBox = child.GetComponent<TextMeshProUGUI>();
                textBox.text = (m_board.GetSpace(spaceIndex).Name).ToUpper();
            }

            // Set the description text box
            if (child.name == "Description")
            {
                TextMeshProUGUI textBox = child.GetComponent<TextMeshProUGUI>();
                textBox.text = m_board.GetSpace(spaceIndex).Description;
            }

            // Set the action button to inactive
            if (child.name == "Action Button")
            {
                Button actionButton = child.GetComponent<Button>();
                // actionButton.interactable = false;

                //Debug.Log("Should Run Once");

                // For when someone lands on the space!
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(m_board.GetSpace(spaceIndex).Action);
                TextMeshProUGUI textBox = actionButton.GetComponentInChildren<TextMeshProUGUI>();
                textBox.text = m_board.GetSpace(spaceIndex).ActionText;

            }
        }
    }
}