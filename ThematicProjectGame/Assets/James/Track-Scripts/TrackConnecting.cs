using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TrackConnecting : MonoBehaviour
{
    [System.Serializable]
    public class TrackType
    {
        public string name;
        public GameObject prefab;
    }

    [Header("Settings")]
    public TrackType[] trackTypes;
    public float gridSize = 1f;
    private int currentTrackIndex = 0;
    private GameObject ghostObject;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private GameObject trackTurner;
    private Button currentButton;
    private float currentY = 0f;
    public Transform lastPlacedTrack;
    private float[] yValues = new float[] { -5.84f, 0f, 5.84f };
    private int yIndex = 1;
    private int tracksPlaced = 0;
    [SerializeField] private GameObject finishPrefab;
    public LayerMask rayLayer;
    [Header("Script Ref")]
    [SerializeField] private PointSystem pointSystem;
    private int RoundNum = 0;
    public int maxRounds = 4;

    [Header("Gizmos Settings")]
    public Vector3 rayPos = new Vector3(0,0,0);
    public Vector3 rayRot = new Vector3(0,0,0);

    [Header("Ending TrackPlacement settings")]
    public GameObject currentFinishLine;

    public int tracksPerRound = 4;
    public int tracksPlacedThisRound = 0;
    [SerializeField] private GameObject trackPacerCamera;
    [SerializeField] private GameObject RaceCamera;
    [SerializeField] private GameObject saveSystemObj;
    [SerializeField] private TrackSaver saveSystem;
    [Header("SFX settings")]
    [SerializeField] private AudioClip placeTrackSound;
    [SerializeField] private AudioClip finishTrackSound;
    private AudioSource audioSource;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameCamera gameCamera;


    

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        trackTurner = new GameObject("TrackTurner", typeof(Transform));
        if(trackTypes.Length > 0)
        {
            //CreateGhostObject();
        }
        else
        {
            Debug.LogError("No track types assigned to TrackConnecting!");
        }

        Debug.Log("Track Placement Started");

        GameManager.stateSwitched.AddListener(StateSwitched);

        GameManager.StartPlacingTrack();
    }

    private void Update()
    {
        UpdateGhostPosition();
        if(Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }

        Debug.DrawRay(rayPos,rayRot * 50, Color.rebeccaPurple);
    }

    void StateSwitched(GameStates oldState, GameStates newState)
    {
        if (newState == GameStates.Track)
        {
            maxRounds = GameManager.RoundNum;
            RoundNum++;
            if(RoundNum == maxRounds)
            {
                saveSystemObj.SetActive(true);
                panel.SetActive(false);
                GameManager.ChangeState(GameStates.Results);
            }


            if (currentFinishLine != null)
            {
                Destroy(currentFinishLine);
            }
            tracksPlacedThisRound = 0;

            if(lastPlacedTrack != null) trackPacerCamera.GetComponent<CinemachineCamera>().Follow = lastPlacedTrack;
            trackPacerCamera.SetActive(true);
            RaceCamera.SetActive(false);
        }
         else if (newState == GameStates.Race)
        {
            if(GameManager.trackToLoad != null)
            {
                saveSystem.Load(GameManager.trackToLoad);
            }
            for(int i=0; i < pointSystem.playerPoints.Length; i++)
            {
                pointSystem.playerPoints[i] = 0;
            }

            Debug.Log("Track Placement Ended");

            trackPacerCamera.SetActive(false);
            RaceCamera.SetActive(true);
            
        }
    }

    public void SelectTrackType(int index, Button button)
    {
        if(index >= 0 && index < trackTypes.Length)
        {
            currentTrackIndex = index;
            currentButton = button;
            DestroyGhostObject();
            CreateGhostObject();
        }
    }

    public TrackType[] GetTrackTypes()
    {
        return trackTypes;
    }

    void CreateGhostObject()
    {
        if(trackTypes[currentTrackIndex].prefab == null)
        {
            Debug.LogError($"Track type '{trackTypes[currentTrackIndex].name}' has no prefab assigned!");
            return;
        }

        ghostObject = Instantiate(trackTypes[currentTrackIndex].prefab);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;    
            color.a = 0.5f; 
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    void DestroyGhostObject()
    {
        if(ghostObject != null)
        {
            Destroy(ghostObject);
        }
    }

    void UpdateGhostPosition()
    {
        if(ghostObject == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Input.GetButtonDown("Jump"))
            {
                yIndex++;
                if(yIndex > yValues.Length) yIndex = 0;
                currentY = yValues[yIndex];
                //ghostObject.transform.parent = trackTurner.transform;
                //trackTurner.transform.position = new Vector3(ghostObject.transform.position.x,ghostObject.transform.position.y + currentY,transform.position.z);
                Debug.Log(currentY + "    " + ghostObject.transform.position);
            }

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(hit.point.x / gridSize) * gridSize,
                (Mathf.Round(hit.point.y / gridSize) * gridSize) + currentY,
                Mathf.Round(hit.point.z / gridSize) * gridSize
            );


            if(currentTrackIndex >= 4)
            {
                snappedPosition += ghostObject.transform.forward * (-gridSize / 2f);
            }
            if(currentTrackIndex == 3)
            {
                snappedPosition = new Vector3(snappedPosition.x,snappedPosition.y + 0.4f,snappedPosition.z);
            }
            
            ghostObject.transform.position = snappedPosition;


            if (occupiedPositions.Contains(snappedPosition))
                SetGhostColor(Color.red);
            else
                SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
        }


        Vector3 ghostPos = ghostObject.transform.position;
        if(currentTrackIndex < 3)
        {
            trackTurner.transform.position = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z) - ghostObject.transform.forward * 10;
        }
        else trackTurner.transform.position = ghostPos;
        
        trackTurner.transform.rotation = ghostObject.transform.rotation;
        ghostObject.transform.parent = trackTurner.transform;
        if(Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") < 0f)
        {
            ghostObject.transform.parent = trackTurner.transform;
            trackTurner.transform.Rotate(0, 90f,0, Space.Self);

        }
        else if(Input.GetButtonDown("Horizontal") && Input.GetAxis("Horizontal") > 0f)
        {
            ghostObject.transform.parent = trackTurner.transform;
            trackTurner.transform.Rotate(0, -90f,0, Space.Self);
        }


        trackTurner.transform.position = ghostPos;
        ghostObject.transform.parent = null;

    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    void PlaceObject()
    {
        if(ghostObject == null)
            return;

        Vector3 placementPosition = ghostObject.transform.position;
        Quaternion placementRotation = ghostObject.transform.rotation;

        if(!occupiedPositions.Contains(placementPosition) && currentTrackIndex >= 0)
        {
            Instantiate(trackTypes[currentTrackIndex].prefab, placementPosition, placementRotation);
            occupiedPositions.Add(placementPosition);

            lastPlacedTrack = ghostObject.transform;
            Debug.Log("Placed track at: " + lastPlacedTrack);
            currentButton.interactable = false;
            int tempIndex = currentTrackIndex;
            currentTrackIndex = -1;
            ghostObject = null;

            tracksPlaced++;
            tracksPlacedThisRound++;
            
            audioSource.PlayOneShot(placeTrackSound);

            if (tracksPlacedThisRound >= tracksPerRound)
            {
                GameManager.EndTrackPlacement();
                PlaceFinishTrack(lastPlacedTrack, tempIndex);
            }
        }
    }

    public void PlaceFinishTrack(Transform lastPlacedTrack, int tmpIndex)
    {
        Transform lastPos = lastPlacedTrack;
        HashSet<Vector3> positions = GetSavedPositions();
        Vector3 newPos = lastPos.position + lastPos.transform.forward * 20f;
        Quaternion newRot = lastPos.rotation;

       

        if(tmpIndex == 3)
        {
            newPos = lastPos.position + (lastPos.transform.forward * -30f);
            //newRot = new Quaternion(lastPos.rotation.x,-lastPos.rotation.y,lastPos.rotation.z,lastPos.rotation.w);
            rayPos = lastPos.position + lastPos.transform.forward * -11f;
            rayRot = -lastPos.transform.forward;
            RaycastHit hit;
            if(Physics.Raycast(lastPos.position + lastPos.transform.forward * -11f,-lastPos.transform.forward,out hit)) 
            {
                if(hit.distance > 30) return;
                newPos = lastPos.position + (lastPos.transform.right * 30f);
                Debug.Log(hit.transform.parent.name);
            }
            newPos -= lastPos.transform.up * 0.4f;
            //Debug.Log("TurnTrack1");
        }
        else if(tmpIndex == 4)
        {
            RaycastHit hit;
            if(Physics.Raycast(lastPos.position + lastPos.transform.forward,lastPos.transform.forward,out hit,rayLayer))
            {
                if(hit.distance > 50) return;
                newPos = lastPos.position + (-lastPos.transform.forward * 40f);
                Debug.Log(hit.transform.parent.name);
            }
            else
            {
                newPos = lastPos.position + lastPos.transform.forward * 40f;
            }
            rayPos = lastPos.position + lastPos.transform.forward * (gridSize/2);
            rayRot = lastPos.transform.forward;
        }
        else
        {
            RaycastHit hit;
            if(Physics.Raycast(lastPos.position + lastPos.transform.forward,lastPos.transform.forward,out hit,rayLayer))
            {
                if(hit.distance > 30) return;
                newPos = lastPos.position + (-lastPos.transform.forward * (gridSize*2));
                Debug.Log(hit.transform.parent.name);
            }
            rayPos = lastPos.position + lastPos.transform.forward * (gridSize/2);
            rayRot = lastPos.transform.forward;
        }
        var finish = Instantiate(finishPrefab,newPos,newRot);
        finish.transform.LookAt(finish.transform.position - (lastPos.position - finish.transform.position));
        finish.transform.localEulerAngles = new Vector3(0,finish.transform.localEulerAngles.y,0);

        Debug.Log("hi i got placed");
        audioSource.PlayOneShot(finishTrackSound);
        currentFinishLine = finish;
    }

    public HashSet<Vector3> GetSavedPositions()
    {
        return occupiedPositions;
    }


    private void OnDestroy()
    {
        DestroyGhostObject();
    }

    // public void OnCollisionEnter(Collision other)
    // {
    //     if(other.gameObject.layer == LayerMask.NameToLayer("Car"))
    //     {
    //         Destroy(other.transform.parent.gameObject);
    //     }
    // }

}
