using UnityEngine;

public class TrackConnecting : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize = 10f; // Size of one grid cell
    
    [Header("Connection Points")]
    public TrackConnectionSide[] availableSides; // Which sides can connect (North, South, East, West)
    
    [Header("Visuals")]
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    public Material normalMaterial;
    
    private Vector3 gridPosition;
    private bool isDragging = false;
    private bool canPlace = true;
    private Camera mainCamera;
    private Renderer trackRenderer;
    private Collider trackCollider;
    private Vector3 dragOffset;
    
    void Start()
    {
        mainCamera = Camera.main;
        trackRenderer = GetComponent<Renderer>();
        trackCollider = GetComponent<Collider>();
        
        if (normalMaterial == null && trackRenderer != null)
        {
            normalMaterial = trackRenderer.material;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            DragTrack();
            CheckPlacementValidity();
        }
    }
    
    void OnMouseDown()
    {
        isDragging = true;
        
        // Calculate offset between mouse position and object position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            dragOffset = transform.position - hit.point;
        }
        
        // Remove from grid if already placed
        if (TrackGridManager.Instance != null)
        {
            TrackGridManager.Instance.RemoveTrackFromGrid(this);
        }
    }
    
    void OnMouseDrag()
    {
        isDragging = true;
    }
    
    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            
            if (canPlace && TrackGridManager.Instance != null)
            {
                // Snap to grid and register
                transform.position = gridPosition;
                TrackGridManager.Instance.PlaceTrackOnGrid(this, gridPosition);
                
                // Reset material
                if (trackRenderer != null && normalMaterial != null)
                {
                    trackRenderer.material = normalMaterial;
                }
            }
            else
            {
                // Invalid placement - could implement return to original position or destroy
                Debug.Log("Invalid placement! Track overlaps with existing track.");
            }
        }
    }
    
    void DragTrack()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Raycast to a ground plane or use a fixed Y position
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        
        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance) + dragOffset;
            
            // Snap to grid
            gridPosition = SnapToGrid(worldPosition);
            transform.position = gridPosition;
        }
    }
    
    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        
        return new Vector3(x, position.y, z);
    }
    
    void CheckPlacementValidity()
    {
        if (TrackGridManager.Instance == null)
        {
            canPlace = true;
            return;
        }
        
        canPlace = !TrackGridManager.Instance.IsPositionOccupied(gridPosition, this);
        
        // Update visual feedback
        if (trackRenderer != null)
        {
            if (canPlace && validPlacementMaterial != null)
            {
                trackRenderer.material = validPlacementMaterial;
            }
            else if (!canPlace && invalidPlacementMaterial != null)
            {
                trackRenderer.material = invalidPlacementMaterial;
            }
        }
    }
    
    public Vector3 GetGridPosition()
    {
        return gridPosition;
    }
    
    public TrackConnectionSide[] GetAvailableSides()
    {
        return availableSides;
    }
    
    public Vector3Int GetGridCoordinates()
    {
        return new Vector3Int(
            Mathf.RoundToInt(gridPosition.x / gridSize),
            0,
            Mathf.RoundToInt(gridPosition.z / gridSize)
        );
    }
}

[System.Serializable]
public enum TrackConnectionSide
{
    North,
    South,
    East,
    West
}
