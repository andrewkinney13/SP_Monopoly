using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_YesNo : MonoBehaviour
{
    // Unity data members
    public GameController m_gameController;
    public TMP_Text m_title;
    public Button m_yesButton;
    public Button m_noButton;

    // Set the button listeners
    void Start()
    { }

    // Getters
    public Button YesButton
    {
        get { return m_yesButton; }
    }
    public Button NoButton
    {
        get { return m_noButton; }
    }

    // Assign the title of the window
    public string Title
    {
        set { m_title.text = value; }
    }
}
