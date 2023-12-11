using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Unity data members
    public Canvas m_menuCanvas;
    public Button m_startButton;
    public Button m_enterPlayerButton;
    public TMP_InputField m_nameInputField;
    public List<Button> m_iconButtons;
    public PopupController m_popupContoller;

    // Private data members
    private int m_playerCount = 0;
    private string m_selectedIcon;
    private Color m_selectedColor;
    private PlayerFile m_playerFile = new PlayerFile();

    // Start is called before the first frame update
    void Start()
    {
        // Initialize button functions
        m_startButton.onClick.AddListener(StartGame);  
        m_enterPlayerButton.onClick.AddListener(CreatePlayer);
        foreach (Button iconButton in m_iconButtons)
        {
            iconButton.onClick.AddListener(() => SelectedIcon(iconButton));
        }

        // Assign selected color
        ColorUtility.TryParseHtmlString("#FDCC00", out m_selectedColor);

        // Initialize the file conroller
        m_playerFile.CreatePlayerFile();

        // Hide the popup window
        m_popupContoller.ClosePopupWindow();
    }

    // Update is called once per frame
    void Update()
    {
        // Game can start w/ 2 players  // NEEDS TO CHANGE LATER BACK TO >= 2!!!
        if (m_playerCount >= 2)
        {
            m_startButton.interactable = true;
        }
        else
        {
            m_startButton.interactable = false;
        }

        // Max of 6 players
        if (m_playerCount < 6)
        {
            m_enterPlayerButton.interactable = true;
        }
        else
        {
            m_enterPlayerButton.interactable = false;
        }
    }

    // User selected an icon 
    private void SelectedIcon(Button selectedIconButton)
    {
        // Clear all button colors, highlight selected button
        foreach (Button iconButton in m_iconButtons)
        {
            Image buttonImage = iconButton.GetComponent<Image>();
            if (iconButton == selectedIconButton)
            {
                buttonImage.color = m_selectedColor;
            }
            else
            {
                buttonImage.color = Color.white;
            }
        }

        // Set the selected icon string
        m_selectedIcon = selectedIconButton.name;
    }

    // Starts the game
    private void StartGame()
    {
        // Close the Xml Player File
        m_playerFile.ClosePlayerFile();

        // Load game scene
        SceneManager.LoadScene("Board Scene");

    }

    // Creates player
    private void CreatePlayer()
    {
        // Obtain the name
        string name = m_nameInputField.text;
        if (string.IsNullOrEmpty(name))
        {
            m_popupContoller.CreatePopupWindow("Error!", 
                "Name must not be empty...", 'E');
            return;
        }

        // Obtain the icon
        if (string.IsNullOrEmpty(m_selectedIcon))
        {
            m_popupContoller.CreatePopupWindow("Error!",
                "Please select an icon...", 'E');
            return;
        }

        // Save the player to file
        m_playerFile.WritePlayerToFile(name, m_selectedIcon);

        // Remove the icon from list of icons
        foreach (Button iconButton in m_iconButtons)
        {
            // Hide the button whose name matches
            if (m_selectedIcon == iconButton.name)
            {
                iconButton.gameObject.SetActive(false);
            }
        }

        // Clear the name textbox
        m_nameInputField.text = "";

        // Reset the icon to null
        m_selectedIcon = string.Empty;

        // Show popup confirming the player was added
        m_popupContoller.CreatePopupWindow("Success",
            "Player " + (m_playerCount + 1) + " added successfuly", 'G');

        // Increment the number of players
        m_playerCount++;
    }
}
