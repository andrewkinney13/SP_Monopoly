using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_Generic : MonoBehaviour
{
    // Unity data members
    public TMP_Text m_title;
    public Button m_actButton;
    public TMP_Text m_actButtonText;

    // Getters and setters
    public string Title
    {
        set { m_title.text = value; }
    }
    public string ActButtonText
    {
        set 
        {
            m_actButtonText.text = value;
        }
    }
    public Button ActButton
    {
        get { return m_actButton; }
    }

    public Color ActButtonColor
    {
        set 
        {
            // Make bold if an actual color
            if (value != Color.black)
            {
                m_actButtonText.fontStyle = FontStyles.Bold;
            }
            else
            {
                m_actButtonText.fontStyle = FontStyles.Normal;
            }
            m_actButtonText.color = value; 
        }
    }

    // Resets the button (removes listeners)
    public void ResetListeners()
    {
        m_actButton.onClick.RemoveAllListeners();
    }

}
