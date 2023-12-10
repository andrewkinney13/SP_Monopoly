using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Action_DetermineOrder : Action
{
    // Unity data members
    public Image m_die1;
    public Image m_die2;
    public Button m_diceButton;
    public Button m_continueButton;
    public TMP_Text m_continueText;

    // Private Data Members
    private int m_diceResult;

    // Start is called before the first frame update
    void Start()
    {
        m_diceButton.onClick.AddListener(DetermineOrder);
        m_continueButton.onClick.AddListener(SendToGame);
        m_continueButton.enabled = false;
        m_continueText.color = Color.red;
    }

    // All players roll dice, players go in order of the result of their dice roll
    void DetermineOrder()
    {
        // They roll the dice
        StartCoroutine(RollDice(m_die1, m_die2));
        
        // Obtain the die values 
        // use what the names of the dice faces are for this

        // Tell game controller
    }

    IEnumerator RollDice(Image die1, Image die2)
    {
        m_diceButton.enabled = false;

        // Shows a new die face for every frame
        float elapsedTime = 0f;
        while (elapsedTime < .25f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_diceResult = Random.Range(2, 12);
        m_continueText.color = Color.green;
        m_continueButton.enabled = true;
    }

    public override void SendToGame()
    {
        m_continueButton.enabled = false;
        m_diceButton.enabled = true;

        m_gameController.Action_OrderDetermined(m_diceResult);
    }
}
