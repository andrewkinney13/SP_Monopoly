using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Action_EndTurn : Action
{
    // Unity data members
    public Button m_endTurnButton;

    // Start is called before the first frame update
    void Start()
    {
        m_endTurnButton.onClick.AddListener(SendToGame);        
    }

    // Tells game controller that the turn is over
    public override void SendToGame()
    {
        m_gameController.TurnCompleted = true;
    }

}
