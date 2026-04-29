using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private GameObject finishPrefab;
    private TrackConnecting trackConnecting;
    private GameObject finishTrack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<TrackConnecting>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
