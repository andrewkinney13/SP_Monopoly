using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml;

public class MenuController : MonoBehaviour
{
    // Data Members
    public Canvas m_menuCanvas;
    public Button m_startButton;
    public Button m_enterPlayerButton;
    public TMP_InputField m_nameInputField;
    public List<Button> m_iconButtons = new List<Button>();

    private XmlWriter m_writer;
    private int m_playerCount;
    private string m_selectedIcon;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize button functions
        m_startButton.onClick.AddListener(StartGame);  
        m_enterPlayerButton.onClick.AddListener(CreatePlayer);
        InitializePlayerFile();
    }

    // Creates "players.xml" file to save player data 
    private void InitializePlayerFile()
    {
        // Create document w/ indentation
        XmlWriterSettings settings = new XmlWriterSettings()
        {
            Indent = true
        };
        m_writer = XmlWriter.Create("players.xml", settings);

        // Root element
        m_writer.WriteStartDocument();
        m_writer.WriteStartElement("Players");
    }

    // Closes the "players.xml" file
    private void ClosePlayerFile()
    {
        m_writer.WriteEndElement(); 
        m_writer.WriteEndDocument();
        m_writer.Flush();
        m_writer.Close();
    }


    // Update is called once per frame
    void Update() 
    {
        if (EnoughPlayers())
        {
            m_startButton.interactable = true;
        }
        else
        {
            m_startButton.interactable = false;
        }
    }

    // Checks if enough players made for a game
    private bool EnoughPlayers()
    {
        return true;
    }

    // Starts the game
    private void StartGame()
    {
        // Close the Xml Player File
        ClosePlayerFile();

        // Load game scene
        SceneManager.LoadScene("Board Scene");

    }

    // Creates player
    private void CreatePlayer()
    {
        // Obtain the name

    }

    // 
}
