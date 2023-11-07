using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Data Members
    public List<Button> m_spaceButtons = new List<Button>();
    public List<Space> m_spaceObjects = new List<Space>();

    private int SPACE_NUM = 40; // There are always 40 spaces on the board


    // Runs when the script is initialized, using this as a constructor
    void Start()
    {
        // Obtain the space buttons from Unity
        GameObject canvasObject = transform.parent.Find("Canvas").gameObject;
        Transform spacesTransform = canvasObject.transform.Find("Spaces");
        m_spaceButtons.AddRange(spacesTransform.GetComponentsInChildren<Button>());

        // Sort the space buttons
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

        // Initalize all the space objects
        for (int i = 0; i < SPACE_NUM; i++)
        {
            Space currentSpace = new Space(GetSpaceName(i), i);
            m_spaceObjects.Add(currentSpace);

            Debug.Log(currentSpace.Description);
        }
    }

    // Update is called once per frame
    void Update() { }

    void OnSpaceClick(int spaceIndex)
    {
        Debug.Log("Hey! User clicked the " + spaceIndex + " space!");
    }

    // Returns the name of the space, based on the name of the grandparent folder
    // that the button is stored in 
    private string GetSpaceName(int spaceIndex) 
    {
        Transform grandparentFolder = m_spaceButtons[spaceIndex].transform.parent.parent;
        return grandparentFolder.gameObject.name;
    }


    /*
    private string GetSpaceName(int index)
    {
        switch (index)
        {
            case 0:
                return "Go";
            case 1:
                return "Mediteranean Avenue";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return ""
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";
            case :
                return "";

            default:
                return "OUT OF INDEX RANGE";
        }   
    
    }
    */
}
