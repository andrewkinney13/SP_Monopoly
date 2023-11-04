using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceButtonManager : MonoBehaviour
{
    public List<Button> spaceButtons = new List<Button>();

    // Initialize all the space buttons
    void Start()
    {
        // Assign the OnClick function
        int index = 0;
        foreach (Button button in spaceButtons) 
        {
            int buttonIndex = index;   // locally scoped variable, for function assignment
            button.onClick.AddListener(() => OnClick(buttonIndex));
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick(int spaceIndex)
    {
        Debug.Log("Hey! User clicked the..." + spaceIndex + " space!");
    }
}
