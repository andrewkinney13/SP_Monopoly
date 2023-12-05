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
        DiceRoll = 0,
        UnownedProperty = 1,
        OwnedColorProperty = 2,
        OwnedUtility = 3,
        OwnedRailroad = 4,
        ChanceOrCommunityChest = 5,
        VisitingJail = 6,
        FreeParking = 7,
        GoToJail = 8,
        Tax = 9,
        EndTurn = 10
    }

    private bool m_turnCompleted = false;
    private string m_currentName;
    private float m_currentCash;
    private Actions m_currentAction;

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

    public void AssignActionWindow(Actions action)
    {
        switch (action)
        {
            case Actions.EndTurn:
            {
                // Assign the window from list 
                GameObject currentActionWindow = m_actionWindows[0];    // index will eventually match enum
                                                                        // ^ this line will be outside of switch  statement

                // Assign title
                TMP_Text title = GameObject.Find("End Turn Title").GetComponent<TMP_Text>();
                title.text = "Turn over...";

                // Assign end button function
                Button endTurn = GameObject.Find("End Turn Button").GetComponent<Button>();
                endTurn.onClick.AddListener(EndTurn);
                currentActionWindow.gameObject.SetActive(true);
                break;
            }
        }
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
