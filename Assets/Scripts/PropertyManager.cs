using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertyManager : MonoBehaviour
{
    // Data memebrs
    public Controller_Game m_gameController;

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


    // ======================================== Start / Update ============================================= //
    void Start()
    {
        // Set window to be inactive to start
        m_window.SetActive(false);
        m_backButton.onClick.AddListener(ClosePropertyManger);
    }

    // ======================================== Public Methods ============================================= //
    public void CreatePropertyManager(string propertyName, string propertyDescription, int mortgageValue, int houseCost, 
        bool buyHouseAvailible, bool sellHouseAvailible, bool buyHotelAvailible, bool sellHotelAvailible, 
        bool mortgageAvailible, bool unmortgageAvailible, int propertyIndex)
    {
        // Add all listeners
        m_buyHouseOrHotelButton.onClick.AddListener(() => m_gameController.PropertyManager_BuyHouse(propertyIndex));
        m_sellHouseOrHotelButton.onClick.AddListener(() => m_gameController.PropertyManager_SellHouse(propertyIndex));

        // Reset scrollbar
        m_scrollbar.value = 0f;

        // Set text of titles
        m_title.text = propertyName;
        m_description.text = propertyDescription;

        // Buying 
        m_buyHouseOrHotelButton.interactable = true;
        if (buyHouseAvailible)
        {
            m_buyHouseOrHotelButtonText.text = "Buy\nHouse\n($-" + houseCost + ")";
        }
        else if (buyHotelAvailible)
        {
            m_buyHouseOrHotelButtonText.text = "Buy\nHotel\n($-" + houseCost + ")";
        }
        else
        {
            m_buyHouseOrHotelButtonText.text = "Buying Unavailible";
            m_buyHouseOrHotelButton.interactable = false;
        }

        // Selling
        m_sellHouseOrHotelButton.interactable = true;   
        if (sellHouseAvailible)
        {
            m_sellHouseOrHotelButtonText.text = "Sell\nHouse\n($+" + houseCost / 2 + ")";
        }
        else if (sellHotelAvailible)
        {
            m_sellHouseOrHotelButtonText.text = "Sell\nHotel\n($+" + houseCost / 2+ ")";
        }
        else
        {
            m_sellHouseOrHotelButtonText.text = "Selling unavailible";
            m_sellHouseOrHotelButton.interactable = false;
        }

        // Mortgage
        if (mortgageAvailible)
        {
            m_mortgageButtonText.text = "Mortgage\n(+$" + mortgageValue + ")";
            m_mortgageButton.onClick.AddListener(() => m_gameController.PropertyManager_MortgageProperty(propertyIndex));
            m_mortgageButton.interactable = true;
        }
        else if (unmortgageAvailible)
        {
            m_mortgageButtonText.text = "Buy back\n(-$" + mortgageValue + ")";
            m_mortgageButton.onClick.AddListener(() => m_gameController.PropertyManager_UnmortgageProperty(propertyIndex));
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
    public void ClosePropertyManger()
    {
        // Hide window
        m_window.SetActive(false);
    }

    // Resets button listenerts
    public void ResetWindow()
    {
        // Reset all listeners
        m_buyHouseOrHotelButton.onClick.RemoveAllListeners();
        m_sellHouseOrHotelButton.onClick.RemoveAllListeners();
        m_mortgageButton.onClick.RemoveAllListeners();
    }
}
