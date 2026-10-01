
/*
This RPG data streaming assignment was created by Fernando Restituto with
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also,
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Serialization (party <-> text)

/*
    Turns a party into text and back again.
    Knows nothing about files or the hard drive.

    Text layout, one value per line, repeated for each character:
        classID
        health
        mana
        strength
        agility
        wisdom
        equipmentCount
        equipmentID   (repeated equipmentCount times)
*/
static public class PartySerializer
{
    static public string SerializeParty(LinkedList<PartyCharacter> party)
    {
        StringBuilder serializedParty = new StringBuilder();

        foreach (PartyCharacter character in party)
            AppendCharacter(serializedParty, character);

        return serializedParty.ToString();
    }

    static public LinkedList<PartyCharacter> DeserializeParty(string serializedParty)
    {
        LinkedList<PartyCharacter> party = new LinkedList<PartyCharacter>();

        using (StringReader lineReader = new StringReader(serializedParty))
        {
            while (lineReader.Peek() != -1)
                party.AddLast(ReadCharacter(lineReader));
        }

        return party;
    }

    static void AppendCharacter(StringBuilder serializedParty, PartyCharacter character)
    {
        serializedParty.AppendLine(character.classID.ToString());
        serializedParty.AppendLine(character.health.ToString());
        serializedParty.AppendLine(character.mana.ToString());
        serializedParty.AppendLine(character.strength.ToString());
        serializedParty.AppendLine(character.agility.ToString());
        serializedParty.AppendLine(character.wisdom.ToString());

        serializedParty.AppendLine(character.equipment.Count.ToString());
        foreach (int equipmentID in character.equipment)
            serializedParty.AppendLine(equipmentID.ToString());
    }

    static PartyCharacter ReadCharacter(StringReader lineReader)
    {
        PartyCharacter character = new PartyCharacter();

        character.classID = ReadInt(lineReader);
        character.health = ReadInt(lineReader);
        character.mana = ReadInt(lineReader);
        character.strength = ReadInt(lineReader);
        character.agility = ReadInt(lineReader);
        character.wisdom = ReadInt(lineReader);

        int equipmentCount = ReadInt(lineReader);
        for (int equipmentIndex = 0; equipmentIndex < equipmentCount; equipmentIndex++)
            character.equipment.AddLast(ReadInt(lineReader));

        return character;
    }

    static int ReadInt(StringReader lineReader)
    {
        return int.Parse(lineReader.ReadLine());
    }
}

#endregion


#region Hard drive storage (text <-> file)
/*
    Reads and writes text on the hard drive.
    Knows nothing about parties or characters.
*/
static public class TextFileStorage
{
    static public void WriteTextToFile(string filePath, string text)
    {
        using (StreamWriter fileWriter = new StreamWriter(filePath))
        {
            fileWriter.Write(text);
        }
    }

    static public string ReadTextFromFile(string filePath)
    {
        using (StreamReader fileReader = new StreamReader(filePath))
        {
            return fileReader.ReadToEnd();
        }
    }
}

#endregion


#region Assignment Part 1

static public class AssignmentPart1
{
    const string PartyFilePath = "party.txt";

    static public void SavePartyButtonPressed()
    {
        string serializedParty = PartySerializer.SerializeParty(GameContent.partyCharacters);
        TextFileStorage.WriteTextToFile(PartyFilePath, serializedParty);
    }

    static public void LoadPartyButtonPressed()
    {
        if (!File.Exists(PartyFilePath))
            return;

        string serializedParty = TextFileStorage.ReadTextFromFile(PartyFilePath);
        GameContent.partyCharacters = PartySerializer.DeserializeParty(serializedParty);

        GameContent.RefreshUI();
    }

}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.

    The load button has been replaced with a drop down list.
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    const string SavedPartiesFolder = "SavedParties/";
    const string SavedPartyFileExtension = ".txt";

    static List<string> savedPartyNames;
    static string selectedPartyName;

    static public void GameStart()
    {
        savedPartyNames = new List<string>();

        if (!Directory.Exists(SavedPartiesFolder))
            Directory.CreateDirectory(SavedPartiesFolder);

        RefreshSavedPartyNames();
        GameContent.RefreshUI();
    }

    static public List<string> GetListOfPartyNames()
    {
        return savedPartyNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        LoadParty(selectedName);
        selectedPartyName = selectedName;

        GameContent.RefreshUI();
    }

    static public void SavePartyButtonPressed()
    {
        string partyName = GameContent.GetPartyNameFromInput();
        if (string.IsNullOrWhiteSpace(partyName))
            return;

        SaveParty(partyName);
        selectedPartyName = partyName;

        RefreshSavedPartyNames();
        GameContent.RefreshUI();
    }

    static public void DeletePartyButtonPressed()
    {
        if (selectedPartyName != null)
        {
            DeleteParty(selectedPartyName);
            selectedPartyName = null;

            GameContent.partyCharacters.Clear();
            RefreshSavedPartyNames();
        }

        GameContent.RefreshUI();
    }

    static void SaveParty(string partyName)
    {
        string serializedParty = PartySerializer.SerializeParty(GameContent.partyCharacters);
        TextFileStorage.WriteTextToFile(GetPartyFilePath(partyName), serializedParty);
    }

    static void LoadParty(string partyName)
    {
        string partyFilePath = GetPartyFilePath(partyName);
        if (!File.Exists(partyFilePath))
        {
            GameContent.partyCharacters.Clear();
            return;
        }

        string serializedParty = TextFileStorage.ReadTextFromFile(partyFilePath);
        GameContent.partyCharacters = PartySerializer.DeserializeParty(serializedParty);
    }

    static void DeleteParty(string partyName)
    {
        string partyFilePath = GetPartyFilePath(partyName);
        if (File.Exists(partyFilePath))
            File.Delete(partyFilePath);
    }

    static void RefreshSavedPartyNames()
    {
        savedPartyNames.Clear();

        foreach (string partyFilePath in Directory.GetFiles(SavedPartiesFolder, "*" + SavedPartyFileExtension))
            savedPartyNames.Add(Path.GetFileNameWithoutExtension(partyFilePath));
    }

    static string GetPartyFilePath(string partyName)
    {
        return SavedPartiesFolder + partyName + SavedPartyFileExtension;
    }

}

#endregion