using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

    public TrackType[] trackTypes;
    public float gridSize = 1f;
    private int currentTrackIndex = 0;
    private GameObject ghostObject;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private GameObject trackTurner;
    private Button currentButton;
    private float currentY = 0f;
    private Transform lastPlacedTrack;
    private float[] yValues = new float[] { -5.84f, 0f, 5.84f };
    private int yIndex = 1;


    private void Start()
    {
        trackTurner = new GameObject("TrackTurner", typeof(Transform));
        if(trackTypes.Length > 0)
        {
            //CreateGhostObject();
        }
        else
        {
            Debug.LogError("No track types assigned to TrackConnecting!");
        }
        
    }

    private void Update()
    {
        UpdateGhostPosition();
        if(Input.GetMouseButtonDown(0))
        {
            PlaceObject();
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


            if(currentTrackIndex == 4)
            {
                snappedPosition += ghostObject.transform.forward * (-gridSize / 2f);
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

            currentButton.interactable = false;
            currentTrackIndex = -1;
            ghostObject = null;

        }
    }

    private void OnDestroy()
    {
        DestroyGhostObject();
    }



}
