using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        if(trackTypes.Length > 0)
        {
            CreateGhostObject();
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

    public void SelectTrackType(int index)
    {
        if(index >= 0 && index < trackTypes.Length)
        {
            currentTrackIndex = index;
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

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(hit.point.x / gridSize) * gridSize,
                Mathf.Round(hit.point.y / gridSize) * gridSize,
                Mathf.Round(hit.point.z / gridSize) * gridSize
            );
            ghostObject.transform.position = snappedPosition;

            if (occupiedPositions.Contains(snappedPosition))
                SetGhostColor(Color.red);
            else
                SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
        }
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

        if(!occupiedPositions.Contains(placementPosition))
        {
            Instantiate(trackTypes[currentTrackIndex].prefab, placementPosition, Quaternion.identity);
            occupiedPositions.Add(placementPosition);
        }
    }

    private void OnDestroy()
    {
        DestroyGhostObject();
    }
}
