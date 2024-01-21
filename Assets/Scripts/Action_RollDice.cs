using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Action_RollDice : Action
{
    // Unity data members
    public Image m_die1;
    public Image m_die2;
    public Button m_diceButton;
    public Button m_continueButton;
    public TMP_Text m_continueText;
    public List<Sprite> m_diceSprites;
    public TMP_Text m_title;

    // Private data members
    private bool m_orderDetermined;
    private int m_diceResult;
    private bool m_wereDoubles;
    private bool m_utilityCostRoll;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize listeners
        m_diceButton.onClick.AddListener(StartRoll);
        m_continueButton.onClick.AddListener(SendToGame);
        m_orderDetermined = false;
        m_utilityCostRoll = false;

        // Set the window
        ResetWindow();
    }

    // Getters and setters
    public bool OrderDetermined
    {
        get { return m_orderDetermined; }
        set { m_orderDetermined = value; }
    }
    public bool UtilityCostRoll
    {
        get { return m_utilityCostRoll; }
        set { m_utilityCostRoll = value; }
    }

    // All players roll dice, players go in order of the result of their dice roll
    void StartRoll()
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
        while (elapsedTime < .01f) // .5f is good ======== TESTING =============
        {
            // Set the dice face to a random image
            die1Val = Random.Range(1, 7);
            die2Val = Random.Range(1, 7);
            die1.sprite = m_diceSprites[die1Val];
            die2.sprite = m_diceSprites[die2Val];

            // Update the time passes
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assign dice result
        //m_diceResult = die1Val + die2Val;

        // ================= TESTING ======================
        m_diceResult = 4;

        // Assign if doubles or not
        m_wereDoubles = false;
        if (die1Val == die2Val)
        {
            m_wereDoubles = true;
        }

        // Update the window
        m_title.text = "You Rolled a " + m_diceResult + "!";
        m_continueText.color = Color.green;
        m_continueButton.interactable = true;
    }
    public override void ResetWindow()
    {
        // Set title accordingly 
        if (!OrderDetermined)
        {
            m_title.text = "Roll Dice to Determine Order...";
        }
        else if (UtilityCostRoll)
        {
            m_title.text = "Roll Dice to Determine Utility Rent Price...";
        }
        else
        {
            m_title.text = "Roll Dice";
        }

        m_continueButton.interactable = false;
        m_continueText.color = Color.red;
        m_diceButton.interactable = true;
        m_die1.sprite = m_diceSprites[0];
        m_die2.sprite = m_diceSprites[0];
    }

    public override void SendToGame()
    {
        // Alert game controller that action occured
        if (!m_orderDetermined)
        {
            m_gameController.Action_OrderDetermined(m_diceResult);

        }
        else if (UtilityCostRoll)
        {
            m_gameController.Action_UtilityCostDetermined(m_diceResult);
        }
        else
        {
            m_gameController.Action_DiceRolled(m_diceResult, m_wereDoubles);
        }

        // Reset the window
        ResetWindow();
    }
}
