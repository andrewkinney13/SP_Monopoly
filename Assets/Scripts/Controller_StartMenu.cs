using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// CLASS
///     Controller_StartMenu : MonoBehaviour - runs start menu.
///     
/// DESCRIPTION
///     This class controls the start menu when the game opens, asking
///     players to enter in their information before starting the game.
/// 
/// </summary>
public class Controller_StartMenu : MonoBehaviour
{
    // ======================================== Unity Data Members ========================================= //
    public Canvas m_menuCanvas;
    public Button m_startButton;
    public Button m_enterPlayerButton;
    public TMP_InputField m_nameInputField;
    public List<Button> m_iconButtons;
    public Controller_Popup m_popupContoller;

    // ======================================== Private Data Members ======================================= //
    int m_playerCount = 0;
    string m_selectedIcon;
    Color m_selectedColor;
    PlayerFile m_playerFile = new PlayerFile();

    // ======================================== Start / Update ============================================= //
    void Start()
    {
        // Initialize button methods
        m_startButton.onClick.AddListener(StartGame);  
        m_enterPlayerButton.onClick.AddListener(CreatePlayer);
        foreach (Button iconButton in m_iconButtons)
            iconButton.onClick.AddListener(() => SelectedIcon(iconButton));

        // Assign selected color
        ColorUtility.TryParseHtmlString("#FDCC00", out m_selectedColor);

        // Initialize the file conroller
        m_playerFile.CreatePlayerFile();

        // Hide the popup window
        m_popupContoller.ClosePopupWindow();
    }
    void Update()
    {
        // Game can start w/ 2 players 
        if (m_playerCount >= 2)
            m_startButton.interactable = true;
        else
            m_startButton.interactable = false;

        // Max of 6 players
        if (m_playerCount < 6)
            m_enterPlayerButton.interactable = true;
        else
            m_enterPlayerButton.interactable = false;
    }

    // ======================================== Private Methods ========================================== //

    /// <summary>
    /// 
    /// NAME
    ///     SelectedIcon - user selected an icon.
    ///     
    /// SYNOPSIS
    ///     void SelectedIcon(Button a_selectedIconButton);
    ///         a_selectedIconButton    --> icon button user selected.
    /// 
    /// DESCRIPTION
    ///     When the user clicks on an icon, this method will save that
    ///     selection and highlight that icon.
    /// 
    /// </summary>
    void SelectedIcon(Button a_selectedIconButton)
    {
        // Clear all button colors, highlight selected button
        foreach (Button iconButton in m_iconButtons)
        {
            Image buttonImage = iconButton.GetComponent<Image>();
            if (iconButton == a_selectedIconButton)
                buttonImage.color = m_selectedColor;
            else
                buttonImage.color = Color.white;
        }

        // Set the selected icon string
        m_selectedIcon = a_selectedIconButton.name;
    }

    /// <summary>
    /// 
    /// NAME
    ///     CreatePlayer - saves a player's data to file.
    /// 
    /// DESCRIPTION
    ///     Checks all the data the user has entered for validity. 
    ///     Ensuring everything is valid, this method will then 
    ///     save all the player data to a player XML file,
    ///     to be read later by the game.
    /// 
    /// </summary>
    void CreatePlayer()
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
                iconButton.gameObject.SetActive(false);
        }

        // Clear the name textbox
        m_nameInputField.text = "";

        // Reset the icon to null
        m_selectedIcon = string.Empty;

        // Show popup confirming the player was added, not at limit yet
        if (m_playerCount != 5)
        {
            m_popupContoller.CreatePopupWindow("Success",
            "Player " + (m_playerCount + 1) + " added successfuly", 'G');
        }
        // Players full popup
        else
        {
            m_popupContoller.CreatePopupWindow("Success",
                "Player " + (m_playerCount + 1) + " added successfuly\nMaximum players created!", 'G');
        }
        
        // Increment the number of players
        m_playerCount++;
    }
    /* void CreatePlayer() */

    // Starts the game
    void StartGame()
    {
        // Close the Xml Player File
        m_playerFile.ClosePlayerFile();

        // Load game scene
        SceneManager.LoadScene("Game");
    }
}
