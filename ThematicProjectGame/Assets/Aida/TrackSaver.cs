using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TrackSaver : MonoBehaviour
{
    [System.Serializable]
    public class SaveableTracksInScene
    {
        public SaveableTrack[] saveableTracks;
    }

    [System.Serializable]
    public class SaveableTrack
    {
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public int ID;
    }

    
    private string trackName = "NewTrack";
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject[] layout;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetSaves();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Save();
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            GetSaves();
        }

    }

    public void Save()
    {
        TrackID[] objectsInScene = FindObjectsByType<TrackID>(FindObjectsSortMode.None);

        SaveableTracksInScene objectData = new SaveableTracksInScene
        {
            saveableTracks = new SaveableTrack[objectsInScene.Length]
        };

        for(int i = 0; i < objectData.saveableTracks.Length; i++)
        {
            objectData.saveableTracks[i] = new SaveableTrack
            {
                WorldPosition = objectsInScene[i].transform.position,
                WorldRotation = objectsInScene[i].transform.rotation,
                ID = objectsInScene[i].ID
            };
        }

        trackName = inputField.text;
        SaveSystem.Save(objectData, trackName);
        Debug.Log("SAVING...");
        GetSaves();
        
    }

    public void Load(TMP_Text button)
    {
        SaveSystem.Load(out SaveableTracksInScene LoadedObjectData, button.text);

        TrackID[] objectsInScene = FindObjectsByType<TrackID>(FindObjectsSortMode.None);
        for(int i=0; i < objectsInScene.Length; i++)
        {
            Destroy(objectsInScene[i].gameObject);
        }

        for(int i=0; i < LoadedObjectData.saveableTracks.Length; i++)
        {
            Instantiate(SaveableTrackLibrary.SaveableTracks[LoadedObjectData.saveableTracks[i].ID], LoadedObjectData.saveableTracks[i].WorldPosition, LoadedObjectData.saveableTracks[i].WorldRotation);
        }
    }

    public void GetSaves()
    {
        string DirectoryPath = $"{Application.dataPath}/Saves/";
        if(!Directory.Exists(DirectoryPath)) return;

        string[] saves = Directory.GetFiles(DirectoryPath, "*.json");
        Debug.Log(saves.Length);
        Debug.Log(layout.Length);
        for(int i=0; i < saves.Length; i++)
        {
            //SaveSystem.Load(out SaveableTracksInScene LoadedObjectData, saves[i]);
            layout[i].transform.GetChild(0).GetComponent<TMP_Text>().text = Path.GetFileNameWithoutExtension(saves[i]).Substring(1);
            layout[i].SetActive(true);
        }

        for(int i=saves.Length; i < layout.Length; i++)
        {
            layout[i].gameObject.SetActive(false);
        }
    }
}




