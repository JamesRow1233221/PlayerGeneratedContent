using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class TrackSaver : MonoBehaviour
{
    [System.Serializable]
    public class SaveableTracksInScene
    {
        public SaveableTrack[] saveableTracks;
        public string courseName;
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
    [SerializeField] private List<GameObject> layout = new List<GameObject>();
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject buttonParent;
    [Header("Screenshot Settings")]
    public Vector3 pos;
    public Vector3 rot;
    public GameObject screenshotCam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetSaves();
        // screenshotCam = new GameObject("ScreenshotCam", typeof(Camera));
        // screenshotCam.transform.position = pos;
        // screenshotCam.transform.eulerAngles = rot;
        screenshotCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // For Debuging!
        // if(Input.GetKeyDown(KeyCode.X))
        // {
        //     Save();
        // }
        // if(Input.GetKeyDown(KeyCode.Z))
        // {
        //     GetSaves();
        // }

    }

    public void Save()
    {
        TrackID[] objectsInScene = FindObjectsByType<TrackID>(FindObjectsSortMode.None);

        SaveableTracksInScene objectData = new SaveableTracksInScene
        {
            saveableTracks = new SaveableTrack[objectsInScene.Length],
            courseName = inputField.text         
        };

        for(int i = 0; i < objectData.saveableTracks.Length; i++)
        {
            objectData.saveableTracks[i] = new SaveableTrack
            {
                WorldPosition = objectsInScene[i].transform.position,
                WorldRotation = objectsInScene[i].transform.rotation,
                ID = objectsInScene[i].ID,
            };
            Debug.Log("Saving Object: " + objectsInScene[i].name);
        }

        trackName = inputField.text;
        SaveSystem.Save(objectData, trackName);
        StartCoroutine(CaptureScreenshot(trackName));
        Debug.Log("SAVING...");
        
    }

    public void Load(string saveName)
    {
        Debug.Log("LOADING TRACK...");
        SaveSystem.Load(out SaveableTracksInScene LoadedObjectData, saveName);

        TrackID[] objectsInScene = FindObjectsByType<TrackID>(FindObjectsSortMode.None);
        for(int i=0; i < objectsInScene.Length; i++)
        {
            Destroy(objectsInScene[i].gameObject);
        }

        Debug.Log("Saved Track list length: " + LoadedObjectData.saveableTracks.Length);
        for(int i=0; i < LoadedObjectData.saveableTracks.Length; i++)
        {
            Debug.Log("I = " + i);
            Instantiate(SaveableTrackLibrary.SaveableTracks[LoadedObjectData.saveableTracks[i].ID], LoadedObjectData.saveableTracks[i].WorldPosition, LoadedObjectData.saveableTracks[i].WorldRotation);
            Debug.Log("Loading Object: " + LoadedObjectData.saveableTracks[i].ID);
        }

        GameManager.trackToLoad = null;
    }

    public void AcrossSceneLoad(TMP_Text button)
    {
        GameManager.TrackToLoad(button.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void GetSaves()
    {
        string DirectoryPath = $"{Application.dataPath}/Saves/";
        if(!Directory.Exists(DirectoryPath)) return;

        string[] saves = Directory.GetFiles(DirectoryPath, "*.json");
        Debug.Log(saves.Length);
        Debug.Log(layout.Count);
        for(int i=0; i < saves.Length; i++)
        {
            //SaveSystem.Load(out SaveableTracksInScene LoadedObjectData, saves[i]);
            if(i == layout.Count)
            {
                var newButt = Instantiate(buttonPrefab,new Vector3(0,0,0), Quaternion.identity,buttonParent.transform);
                layout.Add(newButt);
            }
            string tempName = Path.GetFileNameWithoutExtension(saves[i]).Substring(1);
            layout[i].transform.GetChild(0).GetComponent<TMP_Text>().text = tempName;
            Texture2D tex = new Texture2D(2,2, TextureFormat.BGRA32, false);
            var fileData = File.ReadAllBytes($"{DirectoryPath}{tempName}.png");
            tex.LoadImage(fileData);
            layout[i].transform.GetChild(1).GetComponent<RawImage>().texture = tex;
            layout[i].SetActive(true);
        }

        for(int i=saves.Length; i < layout.Count; i++)
        {
            layout[i].gameObject.SetActive(false);
        }
    }

    public void Quit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator CaptureScreenshot(string courseName)
    {
        screenshotCam.SetActive(true);
        string DirectoryPath = $"{Application.dataPath}/Saves/";
        yield return null;
        ScreenCapture.CaptureScreenshot($"{DirectoryPath}{courseName}.png");
        yield return null;
        screenshotCam.SetActive(false);
        GetSaves();
    }

}




