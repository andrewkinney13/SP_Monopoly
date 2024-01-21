using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Action_LO_UnownedProperty : MonoBehaviour
{
    // Unity data members
    public GameController m_gameController;
    public TMP_Text m_title;
    public Button m_yesButton;
    public Button m_noButton;

    // Set the button listeners
    void Start()
    {
        // Buy the property
        m_yesButton.onClick.AddListener(() => m_gameController.Action_BuyingProperty(true));
        m_noButton.onClick.AddListener(() => m_gameController.Action_BuyingProperty(false));
    }

    // Assign the title of the window
    public string Title
    {
        set { m_title.text = value; }
    }
}
