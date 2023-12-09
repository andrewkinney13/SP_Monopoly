using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelController : MonoBehaviour
{
    // Data members
    public Image m_icon;
    public TMP_Text m_title;
    public TMP_Text m_cash;
    public List<GameObject> m_actionWindows;
    public RectTransform m_propertiesContent;

    public enum Actions
    {
        EndTurn = 0,
        DetermineOrder = 1,
        RollDice = 2,
        UnownedProperty = 3,
        OwnedColorProperty = 4,
        OwnedUtility = 5,
        OwnedRailroad = 6,
        ChanceOrCommunityChest = 7,
        VisitingJail = 8,
        FreeParking = 9,
        GoToJail = 10,
        Tax = 11
    }

    private bool m_turnCompleted = false;
    private string m_currentName;
    private float m_currentCash;
    private Actions m_currentAction;
    private int m_diceResult;

    // Creates the player panel for the player to do actions during their turn
    public void CreatePlayerPanel(Sprite icon, string name, float cash, Actions action)
    {
        // Assign current values for checking later
        m_currentName = name;
        m_currentCash = cash;
        m_currentAction = action;

        // Assign basic attributes
        m_icon.sprite = icon;
        m_title.text = name + "'s Turn";
        m_cash.text = "Cash: $" + cash;

        // Assign action window
        AssignActionWindow(action);
    }

    // Assigns the game action window according to what action the user needs to complete
    public void AssignActionWindow(Actions action)
    {
        // Deactivate any other action windows
        foreach (GameObject actionWindow in m_actionWindows)
        {
            actionWindow.SetActive(false);  
        }

        // Get the window
        GameObject currentActionWindow = m_actionWindows[(int)action];

        // Set the window as active
        currentActionWindow.gameObject.SetActive(true);

        Debug.Log((int)action);
        
        // Assign attributes of window depending on action
        switch (action)
        {
            case Actions.DetermineOrder:
            case Actions.RollDice:
            {
                    // Obtain the dice buttons
                    Button die1 = GameObject.Find("Die 1").GetComponent<Button>();
                    Button die2 = GameObject.Find("Die 2").GetComponent<Button>();

                    // Assign determine order function
                    die1.onClick.AddListener(() => DiceRoll(die1, die2));
                    die2.onClick.AddListener(() => DiceRoll(die1, die2));

                break;
            }
            case Actions.EndTurn:
            { 
                    // Assign end button function
                    Button endTurn = GameObject.Find("End Turn Button").GetComponent<Button>();
                    endTurn.onClick.AddListener(EndTurn);
                break;
            }
        }
    }

    // Returns result of a dice roll 
    // Source of icons: https://game-icons.net/1x1/delapouite/dice-six-faces-three.html 
    public void DiceRoll(Button die1, Button die2)
    {
        DiceResult = Random.Range(2, 12);
    }

    // End turn
    public void EndTurn()
    {
        TurnEnded = true;
    }

    // Getters and setters
    public bool TurnEnded
    {
        get { return m_turnCompleted; }
        set { m_turnCompleted = value; }
    }
    public int DiceResult
    {
        get { return m_diceResult;  }
        set { m_diceResult = value;  }
    }

    // Checks the new and old parameters of the panel, if any changes, update is needed
    public bool NeedsUpdate(string newName, float newCash, Actions newAction)
    {
        if (m_currentName != newName || m_currentCash != newCash || m_currentAction != newAction)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
