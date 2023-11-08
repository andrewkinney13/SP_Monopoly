using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class to control camera controlled by the user
// Handles zooming and moving around while looking at the board
public class CameraController : MonoBehaviour
{
    // Data Members
    private Camera m_camera;
    private Vector3 m_lastMousePosition;
    private float m_minSize = 100.0f;
    private float m_maxSize = 620.0f;

    private void Start()
    {
        // Initialize camera w/ proper size
        m_camera = Camera.main;
        m_camera.orthographicSize = m_maxSize;
    }

    // Runs every frame
    private void Update()
    {
        // Zooms if the player scrolled
        Zoom();

        // Moves camera if user clicks + holds rmb
        Move();
    }

    // If the user scrolls this frame, adjust the screen accordingly
    private void Zoom()
    {
        // Get the scroll direction
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Obtain new size based on the scroll direction
        float newSize = Camera.main.orthographicSize - (scroll * 2) * m_minSize;

        // Assign size if within the max and min range
        newSize = Mathf.Clamp(newSize, m_minSize, m_maxSize);

        // Obtain cursor position
        Vector3 cursorPosition = m_camera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate difference between the cursor position and where the camera was
        Vector3 offset = cursorPosition - m_camera.transform.position;

        // Apply the offset to zoom towards the cursor position
        m_camera.transform.position += offset * (1.0f - newSize / m_camera.orthographicSize);
        m_camera.orthographicSize = newSize;
    }

    // Moves the camera according to cursor position
    public void Move()
    {
        // If user clicks rmb, assign initial mouse position
        if (Input.GetMouseButtonDown(1))
        {
            m_lastMousePosition = Input.mousePosition;
        }

        // Holding rmb, move screen
        if (Input.GetMouseButton(1))
        {
            // Assign current position
            Vector3 currentMousePosition = Input.mousePosition;

            // Obtain the difference betweem where camera was and now is, based on cursor location
            Vector3 delta = Camera.main.ScreenToWorldPoint(m_lastMousePosition) - Camera.main.ScreenToWorldPoint(currentMousePosition);
           
            // Move camera based on the difference
            transform.Translate(delta);

            // Reassign the initial mouse position
            m_lastMousePosition = currentMousePosition;
        }
    }
}
