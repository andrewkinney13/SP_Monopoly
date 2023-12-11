using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerFile
{
    // Data members
    private XmlWriter m_writer;
    private XmlReader m_reader;
    private string m_fileName = "Players.xml";

    // Creates "players.xml" file to save player data 
    public void CreatePlayerFile()
    {
        // Create document w/ indentation
        XmlWriterSettings settings = new XmlWriterSettings()
        {
            Indent = true
        };

        string filePath = Path.Combine(Application.streamingAssetsPath, m_fileName);
        m_writer = XmlWriter.Create(filePath, settings);

        // Root element
        m_writer.WriteStartDocument();
        m_writer.WriteStartElement("Players");
    }

    // Closes the "players.xml" file
    public void ClosePlayerFile()
    {
        m_writer.WriteEndElement();
        m_writer.WriteEndDocument();
        m_writer.Flush();
        m_writer.Close();
    }

    // Writes a player to the xml file
    public void WritePlayerToFile(string name, string icon)
    {
        m_writer.WriteStartElement("Player");
        m_writer.WriteElementString("Name", name);
        m_writer.WriteElementString("Icon", icon);
        m_writer.WriteEndElement();
    }

    // Reads a player from the xml file
    public Dictionary<string, string> ReadPlayersFromFile()
    {
        // Obtain file path THIS NEEDS TO BE CHANGED LATER AFTER TESTING DONE!
        string filePath = Path.Combine(Application.streamingAssetsPath, "Players.xml");

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

        // Return the dictionary
        return players;
    }
}


