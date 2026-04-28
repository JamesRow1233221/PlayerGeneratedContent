using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointsHolder : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPoints
    {
        public int points;
        public int playerID;
    }

    
    public List<PlayerPoints> playersPointHolder = new List<PlayerPoints>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindPlayers()
    {
        List<PlayerInput> tmp = new List<PlayerInput>(FindObjectsByType<PlayerInput>(FindObjectsSortMode.None));
    }
}
