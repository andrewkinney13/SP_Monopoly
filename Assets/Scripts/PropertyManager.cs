using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// CLASS
///     PropertyManager : MonoBehaviour - controls property managing menu.
///     
/// DESCRIPTION
///     This class controls the menu of the player panel which contains 
///     options for developing color properties, and mortgaging all
///     properties. 
/// 
/// </summary>
public class PropertyManager : MonoBehaviour
{
    // ======================================== Unity Data Members ========================================= //
    public TMP_Text m_title;
    public Button m_buyHouseOrHotelButton;
    public TMP_Text m_buyHouseOrHotelButtonText;
    public Button m_sellHouseOrHotelButton;
    public TMP_Text m_sellHouseOrHotelButtonText;
    public Button m_mortgageButton;
    public TMP_Text m_mortgageButtonText;
    public Button m_backButton;
    public TMP_Text m_description;
    public GameObject m_window;
    public Scrollbar m_scrollbar;

    // ======================================== Private Data Members ======================================= //
    public Controller_Game m_gameController;

    // ======================================== Start / Update ============================================= //
    void Start()
    {
        m_window.SetActive(false);
        m_backButton.onClick.AddListener(ClosePropertyManger);
    }

    // ======================================== Public Methods ============================================= //

    /// <summary>
    /// 
    /// NAME
    ///     CreatePropertyManager - creates property manager menu.
    ///     
    /// SYNOPSIS
    ///     public void CreatePropertyManager(string a_propertyName, string a_propertyDescription, 
    ///         int a_mortgageValue, int a_houseCost, bool a_buyHouseAvailible, bool a_sellHouseAvailible,
    ///         bool a_buyHotelAvailible, bool a_sellHotelAvailible, bool a_mortgageAvailible,
    ///         bool a_unmortgageAvailible, int a_propertyIndex)
    ///             a_propertyName          --> name of the property being managed.
    ///             a_propertyDescription   --> description of the property.
    ///             a_mortgageValue         --> property's mortgage value.
    ///             a_houseCost             --> how much a house costs on this property.
    ///             a_buyHouseAvailible     --> whether or not player can buy a house.
    ///             a_sellHouseAvailible    --> whether or not player can sell a house.
    ///             a_buyHotelAvailible     --> whether or not player can buy a house.
    ///             a_sellHotelAvailible    --> whether or not player can sell a hotel.
    ///             a_mortgageAvailible     --> whether or not player can mortage the property.
    ///             a_unmortgageAvailible   --> whether or not player can buy the property back.
    ///             a_propertyIndex         --> location of property on the board.
    /// 
    /// DESCRIPTION
    ///     This method takes in many paramaters, which are from the game indicating
    ///     what the player is and isn't allowed to do in this menu. Based on this data
    ///     they are able to modify their property in many ways: buying or selling houses, 
    ///     mortgage or buy back a property. There are many factors that would affect 
    ///     whether or not a player can modify their properties... the game controller
    ///     handles these.
    /// 
    /// </summary>
    public void CreatePropertyManager(string a_propertyName, string a_propertyDescription, 
        int a_mortgageValue, int a_houseCost, bool a_buyHouseAvailible, bool a_sellHouseAvailible, 
        bool a_buyHotelAvailible, bool a_sellHotelAvailible, bool a_mortgageAvailible,
        bool a_unmortgageAvailible, int a_propertyIndex)
    {
        // Add all listeners
        m_buyHouseOrHotelButton.onClick.AddListener(() => m_gameController.PropertyManager_BuyHouse(a_propertyIndex));
        m_sellHouseOrHotelButton.onClick.AddListener(() => m_gameController.PropertyManager_SellHouse(a_propertyIndex));

        // Reset scrollbar
        m_scrollbar.value = 0f;

        // Set text of titles
        m_title.text = a_propertyName;
        m_description.text = a_propertyDescription;

        // Buying 
        m_buyHouseOrHotelButton.interactable = true;
        if (a_buyHouseAvailible)
            m_buyHouseOrHotelButtonText.text = "Buy\nHouse\n($-" + a_houseCost + ")";

        else if (a_buyHotelAvailible)
            m_buyHouseOrHotelButtonText.text = "Buy\nHotel\n($-" + a_houseCost + ")";

        else
        {
            m_buyHouseOrHotelButtonText.text = "Buying Unavailible";
            m_buyHouseOrHotelButton.interactable = false;
        }

        // Selling
        m_sellHouseOrHotelButton.interactable = true;   
        if (a_sellHouseAvailible)
            m_sellHouseOrHotelButtonText.text = "Sell\nHouse\n($+" + a_houseCost / 2 + ")";
        else if (a_sellHotelAvailible)
            m_sellHouseOrHotelButtonText.text = "Sell\nHotel\n($+" + a_houseCost / 2+ ")";

        else
        {
            m_sellHouseOrHotelButtonText.text = "Selling unavailible";
            m_sellHouseOrHotelButton.interactable = false;
        }

        // Mortgage
        if (a_mortgageAvailible)
        {
            m_mortgageButtonText.text = "Mortgage\n(+$" + a_mortgageValue + ")";
            m_mortgageButton.onClick.AddListener(() => m_gameController.PropertyManager_MortgageProperty(a_propertyIndex));
            m_mortgageButton.interactable = true;
        }
        else if (a_unmortgageAvailible)
        {
            m_mortgageButtonText.text = "Buy back\n(-$" + a_mortgageValue + ")";
            m_mortgageButton.onClick.AddListener(() => m_gameController.PropertyManager_UnmortgageProperty(a_propertyIndex));
            m_mortgageButton.interactable = true;
        }
        else
        {
            m_mortgageButtonText.text = "Mortgaging Unavailible";
            m_mortgageButton.interactable = false;
        }

        // Activate the window
        m_window.SetActive(true);
    }

    // Closes property window
    public void ClosePropertyManger() { m_window.SetActive(false); }

    // Resets button listenerns on all the buttons
    public void ResetWindow()
    {
        m_buyHouseOrHotelButton.onClick.RemoveAllListeners();
        m_sellHouseOrHotelButton.onClick.RemoveAllListeners();
        m_mortgageButton.onClick.RemoveAllListeners();
    }
}
