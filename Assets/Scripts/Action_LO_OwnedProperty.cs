using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_LO_OwnedProperty : Action
{
    // Unity data members
    public TMP_Text m_title;
    public Button m_continueButton;
    public TMP_Text m_continueButtonText;

    // Start
    private void Start()
    {
        m_continueButton.onClick.AddListener(m_gameController.Action_PayingRent);
    }

    // Getters and setters
    public string Title
    {
        set { m_title.text = value; }
    }
    public int ContinueButtonText
    {
        set { m_continueButtonText.text = "Pay: $" + value; }
    }

    // Base class abstract implementations
    public override void SendToGame() { }
    public override void ResetWindow() { }
}
