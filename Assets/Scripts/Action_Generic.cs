
// Header library
// ============================== Unity Data Members =============================== //
// ============================== Private Data Members ============================= //
// ============================== Protected Data Members =========================== //
// ============================== Public Data Members ============================== //
// ============================== Constructor ====================================== //
// ============================== Start / Update =================================== //
// ============================== Virtual Methods ================================== //
// ============================== Override Methods ================================= //
// ============================== Properties ======================================= //
// ============================== Public Methods =================================== //
// ============================== Private Methods ================================== //

using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// CLASS
///     Action_Generic
/// 
/// SYSNOPSIS
///     Action_Generic : MonoBehavior - accessor for a generic (single choice) action window's 
///     properties in the player panel.
///     
/// DESCRIPTION
///     Controls a window in the player panel, under "Current Player Action".
///     This window is used when a player encounters a simple action for which there is 
///     only one action they are capable of doing before progressing in their turn.
/// 
/// </summary>
public class Action_Generic : MonoBehaviour
{
    // ============================== Unity Data Members =============================== //
    public TMP_Text m_title;
    public Button m_actButton;
    public TMP_Text m_actButtonText;

    // ============================== Properties ======================================= //

    // Set the title text
    public string Title
    {
        set { m_title.text = value; }
    }

    // Set the action button's text
    public string ActButtonText
    {
        set 
        {
            m_actButtonText.text = value;
        }
    }

    // Returns a reference to the act button (for adding listeners)
    public Button ActButton
    {
        get { return m_actButton; }
    }

    // Sets the color of the act button's text, and font
    // according to the color
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

    // ============================== Public Methods =================================== //

    // Resets the act button (removes listeners)
    public void ResetListeners()
    {
        m_actButton.onClick.RemoveAllListeners();
    }
}
