using System.Collections.Generic;
using UnityEngine;

public class TrackGridManager : MonoBehaviour
{
    public static TrackGridManager Instance { get; private set; }
    
    [Header("Grid Visualization")]
    public bool showGrid = true;
    public int gridWidth = 20;
    public int gridLength = 20;
    public float gridSize = 10f;
    public Color gridColor = Color.green;
    
    private Dictionary<Vector3Int, TrackConnecting> placedTracks = new Dictionary<Vector3Int, TrackConnecting>();
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool IsPositionOccupied(Vector3 worldPosition, TrackConnecting excludeTrack = null)
    {
        Vector3Int gridCoord = WorldToGridCoordinates(worldPosition);
        
        if (placedTracks.ContainsKey(gridCoord))
        {
            // Check if it's not the same track we're moving
            return placedTracks[gridCoord] != excludeTrack;
        }
        
        return false;
    }
    
    public bool PlaceTrackOnGrid(TrackConnecting track, Vector3 worldPosition)
    {
        Vector3Int gridCoord = WorldToGridCoordinates(worldPosition);
        
        if (IsPositionOccupied(worldPosition, track))
        {
            Debug.LogWarning("Cannot place track - position already occupied!");
            return false;
        }
        
        placedTracks[gridCoord] = track;
        Debug.Log($"Track placed at grid position: {gridCoord}");
        
        // Check for connections with neighboring tracks
        CheckAndConnectNeighbors(track, gridCoord);
        
        return true;
    }
    
    public void RemoveTrackFromGrid(TrackConnecting track)
    {
        Vector3Int gridCoord = track.GetGridCoordinates();
        
        if (placedTracks.ContainsKey(gridCoord) && placedTracks[gridCoord] == track)
        {
            placedTracks.Remove(gridCoord);
            Debug.Log($"Track removed from grid position: {gridCoord}");
        }
    }
    
    void CheckAndConnectNeighbors(TrackConnecting track, Vector3Int gridCoord)
    {
        TrackConnectionSide[] availableSides = track.GetAvailableSides();
        
        // Check all four cardinal directions
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
            new Vector3Int(0, 0, 1),   // North
            new Vector3Int(0, 0, -1),  // South
            new Vector3Int(1, 0, 0),   // East
            new Vector3Int(-1, 0, 0)   // West
        };
        
        TrackConnectionSide[] directions = new TrackConnectionSide[]
        {
            TrackConnectionSide.North,
            TrackConnectionSide.South,
            TrackConnectionSide.East,
            TrackConnectionSide.West
        };
        
        for (int i = 0; i < neighborOffsets.Length; i++)
        {
            Vector3Int neighborCoord = gridCoord + neighborOffsets[i];
            
            if (placedTracks.ContainsKey(neighborCoord))
            {
                TrackConnecting neighbor = placedTracks[neighborCoord];
                
                // Check if both tracks have compatible connection sides
                if (System.Array.Exists(availableSides, side => side == directions[i]))
                {
                    TrackConnectionSide oppositeSide = GetOppositeSide(directions[i]);
                    if (System.Array.Exists(neighbor.GetAvailableSides(), side => side == oppositeSide))
                    {
                        Debug.Log($"Track connected to neighbor on {directions[i]} side!");
                        // Here you could trigger connection events, visual effects, etc.
                    }
                }
            }
        }
    }
    
    TrackConnectionSide GetOppositeSide(TrackConnectionSide side)
    {
        switch (side)
        {
            case TrackConnectionSide.North: return TrackConnectionSide.South;
            case TrackConnectionSide.South: return TrackConnectionSide.North;
            case TrackConnectionSide.East: return TrackConnectionSide.West;
            case TrackConnectionSide.West: return TrackConnectionSide.East;
            default: return TrackConnectionSide.North;
        }
    }
    
    Vector3Int WorldToGridCoordinates(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x / gridSize),
            0,
            Mathf.RoundToInt(worldPosition.z / gridSize)
        );
    }
    
    public TrackConnecting GetTrackAtPosition(Vector3Int gridCoord)
    {
        if (placedTracks.ContainsKey(gridCoord))
        {
            return placedTracks[gridCoord];
        }
        return null;
    }
    
    void OnDrawGizmos()
    {
        if (!showGrid) return;
        
        Gizmos.color = gridColor;
        
        float startX = -(gridWidth / 2) * gridSize;
        float startZ = -(gridLength / 2) * gridSize;
        
        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(startX + x * gridSize, 0, startZ);
            Vector3 end = new Vector3(startX + x * gridSize, 0, startZ + gridLength * gridSize);
            Gizmos.DrawLine(start, end);
        }
        
        // Draw horizontal lines
        for (int z = 0; z <= gridLength; z++)
        {
            Vector3 start = new Vector3(startX, 0, startZ + z * gridSize);
            Vector3 end = new Vector3(startX + gridWidth * gridSize, 0, startZ + z * gridSize);
            Gizmos.DrawLine(start, end);
        }
    }
}
