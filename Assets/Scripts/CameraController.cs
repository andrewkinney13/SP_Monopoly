
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Class to control camera controlled by the user
// Handles zooming and moving around while looking at the board
public class CameraController : MonoBehaviour
{
    // Unity data members
    public Button m_leftRotate;
    public Button m_rightRotate;
    public GameObject m_cameraPivot;

    // Private Data Members
    private Camera m_camera;
    private Vector3 m_lastMousePosition;
    private float m_minSize = 50.0f;
    private float m_maxSize = 325.0f;
    private bool m_zoomEnabled;

    private void Start()
    {
        // Initialize camera w/ proper size
        m_camera = Camera.main;
        m_camera.orthographicSize = m_maxSize - 25f;

        // Assign the rotate function buttons
        m_leftRotate.onClick.AddListener(() => RotateCamera(-90));
        m_rightRotate.onClick.AddListener(() => RotateCamera(90));

        // Enable zooming
        m_zoomEnabled = true;
    }

    // Runs every frame
    private void Update()
    {
        // Zooms if the player scrolled
        Zoom();

        // Moves camera if user clicks + holds rmb
        Move();
    }

    // Rotate camera 
    private void RotateCamera(int angle)
    {
        // Rotate camera
        m_cameraPivot.transform.Rotate(0f, 0f, angle);
    }

    // So other classes with scroll bars can disable zoom
    public void SetZooming(bool zoomState)
    {
        m_zoomEnabled = zoomState;
    }

    // Resets camera's position and orientation 
    public void ResetCamera()
    {
        // Reorient the camera pivot
        int currentRotation = (int)m_cameraPivot.transform.eulerAngles.z;
        RotateCamera(-1 * (currentRotation));

        // Reset size and position within the pivot
        m_camera.orthographicSize = m_maxSize - 25f;
        m_camera.transform.position = new Vector3(-1000f, 0f, -1f);
    }

    // If the user scrolls this frame, adjust the screen accordingly
    private void Zoom()
    {
        // Do nothing if zooming disabled
        if (!m_zoomEnabled) 
        { 
           return;
        }

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

            // Adjust delta based on the rotation of the screen
            int currentRotation = (int)m_camera.transform.eulerAngles.z;
            switch (currentRotation)
            {
                // 90 left, flip x and y, flip x sign
                case 270:
                    float y = delta.y;
                    delta.y = delta.x;
                    delta.x = -y;
                    
                    break;

                // 180 flip, board is upside down, flip x and y signs
                case 180:
                    delta.x = -delta.x;
                    delta.y = -delta.y;
                    break;

                // 90 right, flip x and y, flip y sign
                case 90:
                    float x = delta.x;
                    delta.x = delta.y;
                    delta.y = -x;
                    break;
            }

            // Move camera based on the difference
            transform.Translate(delta);

            // Reassign the initial mouse position
            m_lastMousePosition = currentMousePosition;
        }
    }
}
