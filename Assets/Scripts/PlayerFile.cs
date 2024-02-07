using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

/// <summary>
/// 
/// CLASS
///     PlayerFile - class for saving and reading player data.
///     
/// DESCRIPTION
///     This class controls saving and writing data to and from
///     a file for player data, inbetween scene transitions.
///     Uses XML file format.
/// 
/// </summary>
public class PlayerFile
{
    // ======================================== Private Data Members ======================================= //
    XmlWriter m_writer;
    XmlReader m_reader;
    string m_fileName = "playerData.xml";

    // ======================================== Public Methods ============================================= //


    /// <summary>
    /// 
    /// CLASS
    ///     CreatePlayerFile - creates the file with player data.
    ///     
    /// DESCRIPTION
    ///     This creates the player XML file, and initialies it.
    /// 
    /// </summary>
    public void CreatePlayerFile()
    {
        // Create document w/ indentation
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        string filePath = Path.Combine(Application.streamingAssetsPath, m_fileName);
        m_writer = XmlWriter.Create(filePath, settings);

        // Root element
        m_writer.WriteStartDocument();
        m_writer.WriteStartElement("Players");
    }

    /// <summary>
    /// 
    /// CLASS
    ///     ClosePlayerFile - closes the file with player data.
    ///     
    /// DESCRIPTION
    ///     This method closes the player file.
    /// 
    /// </summary>
    public void ClosePlayerFile()
    {
        m_writer.WriteEndElement();
        m_writer.WriteEndDocument();
        m_writer.Flush();
        m_writer.Close();
    }

    /// <summary>
    /// 
    /// CLASS
    ///     ClosePlayerFile - closes the file with player data.
    ///     
    /// SYNOPSIS
    ///     public void WritePlayerToFile(string a_name, string a_icon);
    ///         a_name      --> name of the player.
    ///         a_icon      --> player's icon name.
    ///     
    /// DESCRIPTION
    ///     This method is used to add player data to the file.
    /// 
    /// </summary>
    public void WritePlayerToFile(string a_name, string a_icon)
    {
        m_writer.WriteStartElement("Player");
        m_writer.WriteElementString("Name", a_name);
        m_writer.WriteElementString("Icon", a_icon);
        m_writer.WriteEndElement();
    }

    /// <summary>
    /// 
    /// CLASS
    ///     ReadPlayersFromFile - reads player file.
    ///     
    /// DESCRIPTION
    ///     This method is used to read player data from the file,
    ///     returns all of it as a dictionary of strings.
    ///     
    /// RETURNS
    ///     Dictionary of player data.
    ///     PlayerName : IconName.
    /// 
    /// </summary>
    public Dictionary<string, string> ReadPlayersFromFile()
    {
        // Obtain file path 
        string filePath = Path.Combine(Application.streamingAssetsPath, m_fileName);


        // Setup the players dict
        Dictionary<string, string> players = new Dictionary<string, string>();

        // Setup the xmlreader object 
        using (m_reader = XmlReader.Create(filePath))
        {
            string name = string.Empty;
            string icon = string.Empty;

            // Read all the player data 
            while (m_reader.Read())
            {
                // We're reading in a player
                if (m_reader.NodeType == XmlNodeType.Element)
                {
                    if (m_reader.Name == "Name")
                    {
                        m_reader.Read(); // Move to the text inside <Name>
                        name = m_reader.Value; // Get the value
                    }
                    else if (m_reader.Name == "Icon")
                    {
                        m_reader.Read(); // Move to the text inside <Icon>
                        icon = m_reader.Value; // Get the value
                    }
                }

                // Done reading in players
                else if (m_reader.NodeType == XmlNodeType.EndElement && m_reader.Name == "Player")
                {
                    players[name] = icon;
                    name = string.Empty;
                    icon = string.Empty;
                }
            }
        }
        return players;
    }
    /* public Dictionary<string, string> ReadPlayersFromFile() */
}


