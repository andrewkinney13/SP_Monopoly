using TMPro;
using UnityEngine;

/// <summary>
/// 
/// CLASS
///     Controller_DetailsPopup : Monobehaviour - controls detail popup windows.
///     
/// DESCRIPTION
///     This class handles the creation and deletion of pop-up windows that 
///     hover over the user's cursor.
/// 
/// </summary>
public class Controller_DetailsPopup : MonoBehaviour
{
    // ======================================== Unity Data Members ========================================= //
    public TMP_Text m_name;
    public TMP_Text m_details;
    public GameObject m_window;

    // ======================================== Private Data Members ======================================= //
    bool m_isActive;

    // ======================================== Start / Update ============================================= //
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

    // ======================================== Public Methods ============================================= //

    /// <summary>
    /// 
    /// NAME
    ///     CreateDetailsWindow - creates details window.
    ///     
    /// SYNOPSIS
    ///     public void CreateDetailsWindow(string a_title, string a_text);
    ///         a_title         --> header of the window.
    ///         a_text          --> what goes in the textbox.
    ///     
    /// DESCRIPTION
    ///     Creates window with passed in title and text, makes it active, and 
    ///     updates the flag that says it's active.
    /// 
    /// </summary>
    public void CreateDetailsWindow(string a_title, string a_text)
    {
        // Set up the name and details
        m_name.text = a_title;
        m_details.text = a_text;

        // Update flag
        m_isActive = true;

        // Enable the window
        m_window.SetActive(true);
    }

    // Closes the window and marks flag
    public void CloseDetailsWindow()
    {
        m_window.SetActive(false);
        m_isActive = false;
    }
}

