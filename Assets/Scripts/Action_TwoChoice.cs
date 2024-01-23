using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_TwoChoice : MonoBehaviour
{
    // Unity data members
    public GameController m_gameController;
    public TMP_Text m_title;
    public Button m_leftButton;
    public Button m_rightButton;
    public TMP_Text m_leftButtonText;
    public TMP_Text m_rightButtonText;

    // Set the button listeners
    void Start()
    { }

    // Getters
    public Button LeftButton
    {
        get { return m_leftButton; }
    }
    public Button RightButton
    {
        get { return m_rightButton; }
    }
    public string LeftButtonText
    {
        set { m_leftButtonText.text = value; }
    }
    public string RightButtonText
    {
        set { m_rightButtonText.text = value; }
    }
    public Color LeftButtonColor
    {
        set
        {
            // Make bold if an actual color
            if (value != Color.black)
            {
                m_leftButtonText.fontStyle = FontStyles.Bold;
            }
            else
            {
                m_leftButtonText.fontStyle = FontStyles.Normal;
            }
            m_leftButtonText.color = value;
        }
    }
    public Color RightButtonColor
    {
        set
        {
            // Make bold if an actual color
            if (value != Color.black)
            {
                m_rightButtonText.fontStyle = FontStyles.Bold;
            }
            else
            {
                m_rightButtonText.fontStyle = FontStyles.Normal;
            }
            m_rightButtonText.color = value;
        }
    }


    // Assign the title of the window
    public string Title
    {
        set { m_title.text = value; }
    }
}
