using System;

/// <summary>
/// 
/// CLASS
///     Space : IComparable<Space> - base class for all board spaces.
///     
/// DESCRIPTION
///     Every space on the board inherits from this class, so 
///     all data and methods within this class apply to all spaces.
/// 
/// </summary>
public class Space : IComparable<Space>
{
    // ======================================== Private Data Members ======================================= //
    protected string m_name;
    protected int m_index;
    protected Board.Actions m_actionType;
    protected string m_description;

    // ======================================== Constructor ================================================ //

    /// <summary>
    /// 
    /// NAME
    ///     Space - constructor for spaces on the board.
    /// 
    /// SYSNOPSIS
    ///     public Space(string a_name, int a_index, Board.Actions a_action, string a_description)
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     This constructor constructs new spaces on the board, and assigns all common 
    ///     data spaces have in the parameters.
    /// 
    /// </summary>
    public Space(string a_name, int a_index, Board.Actions a_action, string a_description)
    {
        Name = a_name;
        Index = a_index;
        ActionType = a_action;
        Description = a_description;
    }

    // ======================================== Virtual Methods ============================================ //

    // Returns the description of the space
    public virtual string Description
    {
        get { return m_description; }
        set { m_description = value; }
    }

    // Whether or not this space is mortgaged.
    public virtual bool IsMortgaged
    {
        get { return false; }
        set { throw new System.Exception("Attempting to mortgage a non-property type!"); }
    }

    // What action a player must do, landing on this property
    public virtual Board.Actions ActionType
    {
        get { return m_actionType; }
        set { m_actionType = value; }
    }

    // ======================================== Properties ================================================= //
    
    // Name of the property
    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    // Property's location on the board
    public int Index
    {
        get { return m_index; }
        set { m_index = value; }
    }

    // ======================================== Public Methods ============================================= //

    /// <summary>
    /// 
    /// NAME
    ///     CompareTo - sorting method definition.
    /// 
    /// SYSNOPSIS
    ///     public Space(string a_name, int a_index, Board.Actions a_action, string a_description)
    ///         a_name              --> Name of the space.
    ///         a_index             --> Index on board of the space.
    ///         a_action            --> The action associated with landing on this space.
    ///         a_description       --> Description of this space.
    ///     
    /// DESCRIPTION
    ///     Sorting method for sorting spaces... this is an arbitrary sorting system
    ///     used for the properties and cards window, so that there is some kind of
    ///     organization there, and properties aren't displayed purely in the order
    ///     that players buy them in. Order of properties is as follows:
    ///         1. Color properties (in order of apperance on board, backwards)
    ///         2. Railroads 
    ///         3. Utilities 
    /// 
    /// </summary>
    public int CompareTo(Space other)
    {
        // This is of type color property 
        if (this is ColorProperty)
        {
            // Both color properties, sort by index
            if (other is ColorProperty)
            {
                if (Index > other.Index) 
                    return -1;
                else
                    return 1;
            }
            // Other space isn't color property
            return -1;
        }
        // This is a railroad
        if (this is Railroad)
        {
            // Other is a color property
            if (other is ColorProperty)
                return 1;

            // Both railroads
            if (other is Railroad) 
            { 
                if (Index > other.Index)
                    return -1;
                return 1;
            }

            // Other is a utility
            return -1;
        }
       
        // Both utilities
        if (other is Utility)
        {
            if (Index > other.Index)
                return -1;
            return 1;
        }

        // Other isn't a utility
        return 1;
    }
    /* public int CompareTo(Space other) */
}
