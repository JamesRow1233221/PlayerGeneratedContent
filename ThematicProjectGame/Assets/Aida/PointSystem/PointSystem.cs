using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private TrackConnecting trackConnecting;
    [SerializeField] private GameObject finishPrefab;
    private GameObject finishTrack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            PlaceFinishTrack();
        }
    }

    public void PlaceFinishTrack()
    {
        Transform lastPos = trackConnecting.lastPlacedTrack;
        Instantiate(finishPrefab,lastPos.position + lastPos.transform.forward * 20f,lastPos.rotation);

    }
}
