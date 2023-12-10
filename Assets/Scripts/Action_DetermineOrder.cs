using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Tilemaps;


public class Action_DetermineOrder : Action
{
    // Unity data members
    public Image m_die1;
    public Image m_die2;
    public Button m_diceButton;
    public Button m_continueButton;
    public TMP_Text m_continueText;
    public List<Sprite> m_diceSprites;

    // Private Data Members
    private int m_diceResult;

    // Start is called before the first frame update
    void Start()
    {
        m_diceButton.onClick.AddListener(DetermineOrder);
        m_continueButton.onClick.AddListener(SendToGame);

        // Set the window
        ResetWindow();
    }

    // All players roll dice, players go in order of the result of their dice roll
    void DetermineOrder()
    {
        // They roll the dice
        StartCoroutine(RollDice(m_die1, m_die2));
    }

    IEnumerator RollDice(Image die1, Image die2)
    {
        m_diceButton.interactable = false;

        // Shows a new die face for every frame
        float elapsedTime = 0f;
        int die1Val = 0;
        int die2Val = 0;
        while (elapsedTime < .5f)
        {
            // Set the dice face to a random image
            die1Val = Random.Range(1, 6);
            die2Val = Random.Range(1, 6);
            die1.sprite = m_diceSprites[die1Val];
            die2.sprite = m_diceSprites[die2Val];

            // Update the time passes
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_diceResult = die1Val + die2Val;
        m_continueText.color = Color.green;
        m_continueButton.interactable = true;
    }

    public override void ResetWindow()
    {
        m_continueButton.interactable = false;
        m_continueText.color = Color.red;
        m_diceButton.interactable = true;
        m_die1.sprite = m_diceSprites[0];
        m_die2.sprite = m_diceSprites[0];
    }

    public override void SendToGame()
    {
        // Reset the window
        ResetWindow();

        // Alert game controller that action occured
        m_gameController.Action_OrderDetermined(m_diceResult);
    }
}
