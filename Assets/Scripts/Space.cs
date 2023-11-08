using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class each space on the board
public class Space
{
    // Data Members
    private GameObject m_gameObject;
    private Button m_button;
    private string m_name;
    private int m_index;
    private string m_description;

    // Constructor
    public Space(GameObject gameObject, Button button, string name, int index)
    {
        GameObject = gameObject;
        Button = button;
        Name = name;
        Index = index;
        Description = Name + ", at index: " + Index;
    }

    // Getters and Setters
    public GameObject GameObject
    {
        get { return m_gameObject; }
        set { m_gameObject = value; }
    }
    public Button Button
    {
        get { return m_button; }
        set { m_button = value; }
    }
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }
    public int Index
    {
        get { return m_index; }
        set { m_index = value; }
    }
    public string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }
}
