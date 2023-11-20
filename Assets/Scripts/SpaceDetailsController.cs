using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceDetailsController : MonoBehaviour
{
    // Data memebers
    public TMP_Text m_name;
    public TMP_Text m_details;
    public Button m_actionButton;
    public TMP_Text m_actionText;
    

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    // Updates the propery details window within the screen UI
    public void SetSpaceDetailsWindow(string propertyName, string propertyDetails, 
        UnityEngine.Events.UnityAction actionFunction, string actionText)
    {
        m_name.text = propertyName;
        m_details.text = propertyDetails;
        m_actionButton.onClick.AddListener(actionFunction);
        m_actionText.text = actionText;
    }
}

