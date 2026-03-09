using System.Collections.Generic;
using UnityEngine;

public class SaveableTrackLibrary : MonoBehaviour
{
    public static Dictionary<int, GameObject> SaveableTracks;

    [SerializeField] private GameObject[] RegisteredObjects;

    private void Awake()
    {
        SaveableTracks = new Dictionary<int, GameObject>();

        for(int i=0; i < RegisteredObjects.Length; i++)
        {
            int iDToRegister = RegisteredObjects[i].GetComponent<TrackID>().ID;
            SaveableTracks.Add(iDToRegister, RegisteredObjects[i]);
        }
    }
}
