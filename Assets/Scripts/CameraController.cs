using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class to control camera controlled by the user
// Handles zooming and moving around while looking at the board
public class CameraController : MonoBehaviour
{
    // Data Members
    private Vector3 m_lastMousePosition; 
    private Camera m_camera;
    public float m_zoomSpeed = 1.0f;
    public float m_minZoom = 200.0f;
    public float m_maxZoom = 630.0f;
    public float m_cameraSpeed = 50.0f;

    private void Start()
    {
        // Initialize camera w/ proper size
        m_camera = Camera.main;
        m_camera.orthographicSize = m_maxZoom;
    }

    // Runs every frame
    private void Update()
    {
        // Scrolls if the player scrolled
        CheckScroll();

        // User clicked rmb
        if (Input.GetMouseButtonDown(1))
        {
            m_lastMousePosition = Input.mousePosition;
        }

        // User holding rmb, move the camera
        else if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - m_lastMousePosition;
            m_lastMousePosition = Input.mousePosition;
            transform.Translate(-mouseDelta * Time.deltaTime * m_cameraSpeed);
        }
    }

    // If the user scrolls this frame, adjust the screen accordingly
    private void CheckScroll()
    {
        // Get the scroll direction
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Obtain new size based on the scroll direction
        float newSize = Camera.main.orthographicSize - scroll * m_minZoom;

        // Assign size if within the max and min
        newSize = Mathf.Clamp(newSize, m_minZoom, m_maxZoom);

        // Obtain cursor position
        Vector3 cursorPosition = m_camera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate difference between the cursor position and where the camera was
        Vector3 offset = cursorPosition - m_camera.transform.position;

        // Apply the offset to zoom towards the cursor position
        m_camera.transform.position += offset * (1.0f - newSize / m_camera.orthographicSize);
        m_camera.orthographicSize = newSize;
    }
}
