using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_LO_UnownedProperty : Action
{
    // Unity data members
    public TMP_Text m_title;
    public Button m_yesButton;
    public Button m_noButton;

    // Set the button listeners
    void Start()
    {
        // Buy the property
        m_yesButton.onClick.AddListener(() => m_gameController.Action_BuyingProperty(true));
        m_noButton.onClick.AddListener(() => m_gameController.Action_BuyingProperty(false));

        // Reset the window
    }

    // Assign the title of the window
    public string Title
    {
        set { m_title.text = value; }
    }

    // Base class abstract implementations
    public override void SendToGame() { }    
    public override void ResetWindow() { }
}
