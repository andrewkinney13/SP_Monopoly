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
    public GameObject m_window; 

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    // Creates a propery details window within the screen UI, where the user clicked the propery
    public void CreateSpaceDetailsWindow(string propertyName, string propertyDetails, Vector2 cursorLocation)
    {
        // Set up the name and details
        m_name.text = propertyName;
        m_details.text = propertyDetails;
        
        // Assign location (corner of cursor)
        RectTransform windowTransform = m_window.GetComponent<RectTransform>();
        cursorLocation.y += windowTransform.rect.height / 2;
        windowTransform.position = cursorLocation;

        // Enable the window
        m_window.SetActive(true);
    }

    // Closes the window 
    public void CloseSpaceDetailsWindow()
    {
        m_window.SetActive(false);
    }
}

