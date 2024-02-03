/// <summary>
/// 
/// CLASS
///     CardSpace : Space - inherited space class for Community Chest and Chance spaces.
///     
/// DESCRIPTION
///     Controls functionality of Community Chest and Chance spaces on the board, specifically
///     allows for the calling of a method which picks up a card from the Contrller_Card
///     class, which manages the decks of cards in the game.
/// 
/// </summary>
public class CardSpace : Space
{
    // ======================================== Private Data Members ======================================= //
    Controller_Card m_cardController;

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     CardSpace - constructor for Community Chest and Chance spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public CardSpace(string a_name, int a_index, Board.Actions a_action, Controller_Card a_cardController, 
    ///     string a_description) : base(a_name, a_index, a_action, a_description)
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_cardController    --> Class to manage card decks.
    ///         a_description       --> Description of the space.
    ///         
    ///     
    /// DESCRIPTION
    ///     Assigns reference to the ControllerCard class object, everything else is 
    ///     initialized with the base class (Space) constructor.
    /// 
    /// </summary>
    public CardSpace(string a_name, int a_index, Board.Actions a_action, Controller_Card a_cardController, 
        string a_description) : base(a_name, a_index, a_action, a_description)
    {
        m_cardController = a_cardController;
    }

    // ======================================== Public Methods ============================================= //

    // Returns card from appropriate deck from the cardController,
    // depending on what space type the player is on.
    public Card TakeCard()
    {
        // Return appropriate card from respective deck
        if (Name == "Chance")
            return m_cardController.TakeChanceCard();
        
        else
            return m_cardController.TakeCommunityChestCard();
    }
}
