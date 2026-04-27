using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string DirectoryPath;
    private static bool Initialised;


    public static void Init()
    {
        DirectoryPath = $"{Application.dataPath}/Saves/";

        if(!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);

        Initialised = true;
    }

    public static void Save(object DataToSave, string FileName = "Save")
    {
        if(!Initialised)
        {
            Init();
        }

        string JSONSave = JsonUtility.ToJson(DataToSave);

        if(File.Exists($"{DirectoryPath} {FileName}.json"))
        {
            File.Delete($"{DirectoryPath} {FileName}.json");
        }

        StreamWriter saveFileWriter = new StreamWriter($"{DirectoryPath} {FileName}.json");

        saveFileWriter.WriteLine(JSONSave);
        saveFileWriter.Close();
    }

    public static void Load<T>(out T LoadedData, string FileName = "Save")
    {
        if(!Initialised)
        {
            Init();
        } 

        StreamReader saveFileReader = new StreamReader($"{DirectoryPath} {FileName}.json");
        string JSONSave = saveFileReader.ReadLine();
        LoadedData = JsonUtility.FromJson<T>(JSONSave);  
    }
}
