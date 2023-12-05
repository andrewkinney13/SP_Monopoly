using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPopupController : MonoBehaviour
{
    // Data memebers
    public TMP_Text m_name;
    public TMP_Text m_details;
    public GameObject m_window; 
    
    private bool m_isActive;

    // Update is called once per frame
    void Update()
    {
        // Renders the window above the cursor if it's active
        if (m_isActive)
        {
            // Get cursor location
            Vector2 mousePosition = Input.mousePosition;

            // Render the space details window
            RectTransform windowTransform = m_window.GetComponent<RectTransform>();
            mousePosition.y += windowTransform.rect.height / 2;
            windowTransform.position = mousePosition;
        }
    }

    // Creates a propery details window within the screen UI
    public void CreateDetailsWindow(string propertyName, string propertyDetails)
    {
        // Set up the name and details
        m_name.text = propertyName;
        m_details.text = propertyDetails;

        // Set the window as active
        m_isActive = true;

        // Enable the window
        m_window.SetActive(true);
    }

    // Closes the window 
    public void CloseDetailsWindow()
    {
        m_window.SetActive(false);
        m_isActive = false;
    }

}

