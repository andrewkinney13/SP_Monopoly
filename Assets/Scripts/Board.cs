using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Buttons
    public List<Button> spaceButtons = new List<Button>();
    //public Button diceButton;

    // Game objects
    

    // Initialize all the space buttons
    void Start()
    {
        // Space Button
        int index = 0;
        foreach (Button button in spaceButtons) 
        {
            int buttonIndex = index;   // locally scoped variable, for function assignment
            button.onClick.AddListener(() => OnSpaceClick(buttonIndex));
            index++;
        }

        // Dice Button
        //diceButton.onClick.AddListener(OnDiceClick);

    }

    // Update is called once per frame
    void Update() { }

   
    void OnSpaceClick(int spaceIndex)
    {
        Debug.Log("Hey! User clicked the..." + spaceIndex + " space!");
    }

    // 
    /*
    void OnDiceClick()
    {

    }    */
}
