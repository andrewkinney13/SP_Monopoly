using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Data Members
    public List<Image> m_playerIcons;
    public Button m_movePlayers;

    private List<List<Vector2>> m_playerLanes = new List<List<Vector2>>();
    private int m_spaceIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Create the player lanes
        CreateLanes();

        m_movePlayers.onClick.AddListener(MovePlayers);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Initializes a list of lanes for each player
    // lane = list of positions a player can go to for the spaces on the board
    private void CreateLanes()
    {
        // Create offsets for each player (so no direct overlap)
        Vector2[] offsets = new Vector2[]
        {
            new Vector2(-.2f,.45f),
            new Vector2(.2f,.45f),
            new Vector2(-.2f,0f),
            new Vector2(.2f,0f),
            new Vector2(-.2f,-.4f),
            new Vector2(.2f,-.4f)
        };

        // Runs for every player
        for (int playerNum = 0; playerNum < 6; playerNum++) 
        { 
            // Create new lane
            List<Vector2> currentLane = new List<Vector2>();

            // Set vectors for every space
            for (int spaceNum = 0; spaceNum < 40; spaceNum++)
            {
                // Declare variables
                float spaceX, spaceY;

                // First lane
                if (spaceNum >= 0 && spaceNum < 10)
                {
                    // Assign x and y
                    spaceX = GetHorizontalPositon(spaceNum) + offsets[playerNum].x;
                    spaceY = -5.2f + offsets[playerNum].y;
                    if (spaceNum % 10 == 0)
                        spaceX += offsets[playerNum].x / 2;
                }

                // Second lane
                else if (spaceNum >= 10 && spaceNum < 20)
                {
                    // Assign x and y
                    spaceY = -1 * (GetHorizontalPositon(spaceNum - 10) + offsets[playerNum].x);
                    spaceX = -5.25f + offsets[playerNum].y;
                    if (spaceNum % 10 == 0)
                        spaceY -= offsets[playerNum].x / 2;
                }

                // Third lane
                else if (spaceNum >= 20 && spaceNum < 30)
                {
                    // Assign x and y
                    spaceX = -1 * (GetHorizontalPositon(spaceNum - 20) + offsets[playerNum].x);
                    spaceY = -1 * (-5.25f + offsets[playerNum].y);
                    if (spaceNum % 10 == 0)
                        spaceX -= offsets[playerNum].x / 2;
                }

                // Fourth lane
                else
                {
                    // Assign x and y
                    spaceY = GetHorizontalPositon(spaceNum - 30) + offsets[playerNum].x;
                    spaceX = -1* (-5.25f + offsets[playerNum].y);
                    if (spaceNum % 10 == 0)
                        spaceY += offsets[playerNum].x / 2;
                }

                // Assign the vector to the current lane at this space
                currentLane.Add(new Vector2(spaceX, spaceY));
            }

            // Add this lane for the current player
            m_playerLanes.Add(currentLane);
        }
    }

    // Obtains the horizontal position of an icon depending on what space it's on
    private float GetHorizontalPositon(int location)
    {
        switch (location) 
        {
            case 0:
                return 5.25f;
            case 1:
                return 4f;
            case 2:
                return 3f;
            case 3:
                return 2f;
            case 4:
                return 1f;
            case 5:
                return 0f;
            case 6:
                return -1f;
            case 7:
                return -2f;
            case 8:
                return -3f;
            case 9:
                return -4f;
            case 10:
                return -5.25f;
        }
        throw new ArgumentException("Space index out of range...");
    }

    // Obtains the location that a player should be at,
    // given their player index and space position
    public Vector2 GetIconPosition(int playerNum, int spaceNum)
    {
        // Get this players lane
        List<Vector2> lane = m_playerLanes[playerNum];
        
        // Return the location at that space index
        return lane[spaceNum];
    }

    private void MovePlayers()
    {
        for (int playerNum = 0; playerNum < 6; playerNum++)
        {
            // Assign the position
            RectTransform imageTransform = m_playerIcons[playerNum].rectTransform;
            imageTransform.anchoredPosition = GetIconPosition(playerNum, m_spaceIndex);

            // Assign the rotation
            Vector3 currentRotation = imageTransform.eulerAngles;
            if (m_spaceIndex >= 0 && m_spaceIndex < 10)
            {
                  currentRotation.z = 0;
            }
            else if (m_spaceIndex >= 10 && m_spaceIndex < 20)
            {
                currentRotation.z = 270;
            }
            else if (m_spaceIndex >= 20 && m_spaceIndex < 30)
            {
                currentRotation.z = 180;
            }
            else
            {
                currentRotation.z = 90;
            }
            imageTransform.eulerAngles = currentRotation;
        }
        m_spaceIndex++;
    }
}
