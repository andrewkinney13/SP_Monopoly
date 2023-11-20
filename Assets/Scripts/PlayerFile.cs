using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class PlayerFile
{
    // Data members
    private XmlWriter m_writer;
    private string m_fileName = "Players.xml";


    // Constructor
    public PlayerFile()
    {
        
    }

    // Creates "players.xml" file to save player data 
    public void CreatePlayerFile()
    {
        // Create document w/ indentation
        XmlWriterSettings settings = new XmlWriterSettings()
        {
            Indent = true
        };

        string filePath = Path.Combine(Application.streamingAssetsPath, m_fileName);
        m_writer = XmlWriter.Create(m_fileName, settings);

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
}


