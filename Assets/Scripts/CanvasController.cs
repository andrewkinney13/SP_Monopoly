using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles the board objects
public class CanvasController : MonoBehaviour
{
    // Data Members
    public Canvas m_boardCanvas;
    public Canvas m_screenUICanvas;
    public Camera m_propertyCamera;

    private List<Button> m_spaceButtons = new List<Button>();
    private List<GameObject> m_spaceGameObjects = new List<GameObject>();   
    private Board m_board = new Board();
    private const int SPACE_NUM = 40; // There are always 40 spaces on the board

    // Runs when the script is initialized, using this as a constructor
    void Start()
    {
        // Initialize the buttons and game objects for this class
        InitializeObjects();

        // Initialize the board
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

    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
        // Display property info of a space
        UpdatePropertyDetails(spaceIndex);
    }


    // Updates the propery details window within the screen UI
    private void UpdatePropertyDetails(int spaceIndex)
    {
        // Move the property camera to the appropriate position 
        Vector3 spacePosition = m_spaceGameObjects[spaceIndex].transform.position;
        spacePosition.z = -1;  // Don't set the z to the objects z
        m_propertyCamera.transform.position = spacePosition;

        // SOMETHING TO ADD:
        // MAKE THE ROTATION ALWAYS CORRECT, SET IT BASED ON INDEX VALUE!
    }

}
