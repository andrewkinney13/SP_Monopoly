using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_PayOrCollect : Action
{
    // Unity data members
    public TMP_Text m_title;
    public Button m_continueButton;
    public TMP_Text m_continueButtonText;

    // Getters and setters
    public string Title
    {
        set { m_title.text = value; }
    }
    public int ContinueButtonAmount
    {
        set 
        {
            if (value  < 0)
            {
                m_continueButtonText.color = Color.red;
                m_continueButtonText.text = "Pay: $" + -1 * value;
            }
            else
            {
                m_continueButtonText.color = Color.green;
                m_continueButtonText.text = "Collect: $" + value;
            }
        }
    }
    
    public Button ContinueButton
    {
        get { return m_continueButton; }
    }

    // Base class abstract implementations
    public override void SendToGame() { }
    public override void ResetWindow() { }
}
