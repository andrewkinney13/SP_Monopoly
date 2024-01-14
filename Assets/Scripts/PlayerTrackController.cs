using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTrackController : MonoBehaviour
{
    // Unity data members
    public List<Button> m_playerIcons;
    public GameObject m_movingPlayerMessageWindow;

    // Private data members
    private List<List<Vector2>> m_playerLanes = new List<List<Vector2>>();
    private float m_iconMovementAnimationDuration = .25f;

    // Set the player moving message to inactive
    private void Start()
    {
        m_movingPlayerMessageWindow.SetActive(false);
    }

    // Sets the player icon buttons with the correct order assigned by GameController
    public void SetIcons(List<Button> icons)
    {
        m_playerIcons = icons;
    }

    // Initializes a list of lanes for each player
    // lane = list of positions a player can go to for the spaces on the board
    public void CreateLanes()
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

    // Moves player icon in an animated fashion to specified location
    public IEnumerator MovePlayer(int playerNum, int initialSpace, int destinationSpace)
    {
        // Check for 0,0 (initialize call)
        if (initialSpace == 0 && destinationSpace == 0) 
        {
            // Get icon of player and position to move player to
            Vector2 destinationPosition = GetIconPosition(playerNum, 0);
            Image playerIcon = m_playerIcons[playerNum].GetComponent<Image>();

            // Assign position to that icon
            playerIcon.rectTransform.anchoredPosition = destinationPosition;

            yield break;
        }

        // Cover the current action window
        m_movingPlayerMessageWindow.SetActive(true);

        // Figure out how far the space needs to travel
        int spaceDifference;
        if (initialSpace <= destinationSpace)
        {
            spaceDifference = destinationSpace - initialSpace;
        }
        else
        {
            spaceDifference = (40 - initialSpace) + destinationSpace; 
        }

        // Move each space to the next
        for (int spacesMoved = 0; spacesMoved < spaceDifference; spacesMoved++) 
        {
            // Set the current and next space numbers
            int currentSpace = initialSpace + spacesMoved;
            if (currentSpace > 39)
            {
                currentSpace -= 40;
            }
            int nextSpace = currentSpace + 1;
            if (nextSpace == 40)
            {
                nextSpace = 0;
            }

            // Get the initial and destination locations of this and next space
            Vector2 initialPosition = GetIconPosition(playerNum, currentSpace);
            Vector2 destinationPosition = GetIconPosition(playerNum, nextSpace);
            Image playerIcon = m_playerIcons[playerNum].GetComponent<Image>();

            // Moves the icon to that position over time
            float elapsedTime = 0f;
            while (elapsedTime < m_iconMovementAnimationDuration)
            {
                float t = elapsedTime / m_iconMovementAnimationDuration;
                playerIcon.rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, destinationPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            playerIcon.rectTransform.anchoredPosition = destinationPosition; // Ensure final position is exact

            // Rotate the icon if needed once moved
            Vector3 currentRotation = playerIcon.rectTransform.eulerAngles;
            if (nextSpace >= 0 && nextSpace < 10)
            {
                currentRotation.z = 0;
            }
            else if (nextSpace >= 10 && nextSpace < 20)
            {
                currentRotation.z = 270;
            }
            else if (nextSpace >= 20 && nextSpace < 30)
            {
                currentRotation.z = 180;
            }
            else
            {
                currentRotation.z = 90;
            }
            playerIcon.rectTransform.eulerAngles = currentRotation;
        }

        // Uncover action window
        m_movingPlayerMessageWindow.SetActive(false);
    }
}
