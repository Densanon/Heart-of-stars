using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    static string FileData;

    public static void WipeString()
    {
        FileData = "";
    }
    
    public static void SaveResource(ResourceData data, bool last)
    {
        if (!last)
        {
            FileData = FileData + data.DigitizeForSerialization();
        }
        else
        {
            string s = data.DigitizeForSerialization();
            s = s.Remove(s.Length - 1);
            FileData = FileData + s;

        }
    }

    public static void SaveAddress(string address)
    {
        FileData = address;
    }

    public static void SaveFile(string file)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + file;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, FileData);
        stream.Close();

        Debug.Log($"SaveString: {FileData}");
        WipeString();

    }


    public static string LoadFile(string file)
    {
        WipeString();

        string path = Application.persistentDataPath + file;
        if (File.Exists(path))
        {
            //Debug.Log("Found a file, and should be grabbing it.");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string s = formatter.Deserialize(stream) as string;
            Debug.Log($"Here is the file: {s}");
            stream.Close();

            return s;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void SeriouslyDeleteAllSaveFiles()
    {
        string path = Application.persistentDataPath + "/resource_shalom";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("I found the file, and have deleted it?");
        }
    }
}
