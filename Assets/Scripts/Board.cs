using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles the board objects
public class Board : MonoBehaviour
{
    // Data Members
    public Canvas m_boardCanvas;
    public Canvas m_screenUICanvas;
    public Camera m_propertyCamera;

    private List<Space> m_spaces = new List<Space>();
    private int SPACE_NUM = 40; // There are always 40 spaces on the board

    // Runs when the script is initialized, using this as a constructor
    void Start()
    {
        // Initialize the spaces
        InitializeSpaces();
    }

    // Update is called once per frame
    void Update() { }

    // When user clicks a space
    void OnSpaceClick(int spaceIndex)
    {
        // Display property info of a space
        UpdatePropertyDetails(spaceIndex);
    }

    // Initializes a list of space objects based on data found in Hiararchy window of Unity Project
    private void InitializeSpaces()
    {
        // Find the spaces folder
        Transform spacesTransform = m_boardCanvas.transform.Find("Spaces");

        // Find all the buttons that represent spaces
        List<Button> spaceButtons = new List<Button>();
        spaceButtons.AddRange(spacesTransform.GetComponentsInChildren<Button>());

        // Sort the buttons by position on the board
        spaceButtons.Sort((button1, button2) =>
        {
            int num1 = int.Parse(button1.gameObject.name);
            int num2 = int.Parse(button2.gameObject.name);
            return num1.CompareTo(num2);
        });

        // Assign space button functions
        int index = 0;
        foreach (Button button in spaceButtons)
        {
            int buttonIndex = index;
            button.onClick.AddListener(() => OnSpaceClick(buttonIndex));
            index++;
        }

        // Obtain the gameobject associated with each space and it's name
        List<GameObject> spaceGameObjects = new List<GameObject>();
        List<string> spaceNames = new List<string>();
        for (int i = 0; i < SPACE_NUM; i++)
        {
            // Obtain the space game object, which is the grandparent folder of it's space button
            Transform grandparentFolder = spaceButtons[i].transform.parent.parent;
            GameObject spaceGameObject = GameObject.Find(grandparentFolder.name);

            // Add the gameobject to the list and the name of the space
            spaceGameObjects.Add(spaceGameObject);
            spaceNames.Add(spaceGameObject.name);
        }

        // Create all the spaces and add them into the list
        for (int i = 0; i < SPACE_NUM; i++)
        {
            // Create space with all the attributes obtained in this function
            Space space = new Space(spaceGameObjects[i], spaceButtons[i], spaceNames[i], i);

            // Add the space to the list
            m_spaces.Add(space);
        }
    }


    // Updates the propery details window within the screen UI
    private void UpdatePropertyDetails(int spaceIndex)
    {
        Debug.Log(spaceIndex);

        // Move the property camera to the appropriate position 
        Transform space = m_spaces[spaceIndex].GameObject.transform;
        m_propertyCamera.transform.position = space.position;

        // WHAT TO FIX! The z direction is set to 0 and needs to not be set to this
        // only assign the x and y from the space position:)
    }

}
