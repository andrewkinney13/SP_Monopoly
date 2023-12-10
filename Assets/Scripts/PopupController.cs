using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    // Unity Data Members
    public GameObject m_popupWindow;
    public Button m_popupCloseButton;
    public TMP_Text m_title;
    public TMP_Text m_message;
    public CameraController m_cameraController;

    public void Start()
    {
        m_popupCloseButton.onClick.AddListener(ClosePopupWindow);
    }

    // Closes the popup window
    public void ClosePopupWindow()
    {
        // Enable zooming for camera if a controller class is defined
        try
        {
            m_cameraController.SetZooming(true);
        }
        catch { }

        // Disable the window
        m_popupWindow.SetActive(false);
    }

    // Creates a popup window
    public void CreatePopupWindow(string title, string message, char type = 'N')
    {
        // Set name and message
        m_title.text = title;
        m_message.text = message;

        // Set the color depending on type
        if (type == 'E')
        {
            m_title.color = Color.red;
        }
        else if (type == 'G')
        {
            m_title.color = Color.green;
        }
        else
        {
            m_title.color = Color.white;
        }

        // Disable zooming if a camera controller is defined
        try
        {
            m_cameraController.SetZooming(false);
        }
        catch { }
        

        // Set the popup window as active
        m_popupWindow.SetActive(true);
    }
}
