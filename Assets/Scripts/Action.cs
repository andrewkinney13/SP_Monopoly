using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    // Unity data members
    public GameController m_gameController;

    // Pure virtual functions to implement
    public abstract void SendToGame();
}
