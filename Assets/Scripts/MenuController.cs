using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Data Members
    public Canvas m_menuCanvas;
    public Button m_startButton; 

    // Start is called before the first frame update
    void Start()
    {
        m_startButton.onClick.AddListener(StartGame);  
    }

    // Update is called once per frame
    void Update() 
    {
        if (ValidPlayers())
        {
            m_startButton.interactable = true;
        }
        else
        {
            m_startButton.interactable = false;
        }
    }

    // Checks 
    public bool ValidPlayers()
    {
        return false;
    }

    // Starts the game
    private void StartGame()
    {
        // Load game scene
        SceneManager.LoadScene("Board Scene");
    }
}
