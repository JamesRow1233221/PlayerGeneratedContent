using UnityEngine;
using UnityEngine.InputSystem;

public class FinishLine : MonoBehaviour
{
    public BoxCollider collider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.StartPlacingTrack();
            PointSystem pointSystem = FindFirstObjectByType<PointSystem>();
            PlayerInput playerInput = other.TryGetComponent<PlayerInput>(out PlayerInput input) ? input : null;
            if (playerInput != null)
            {
                pointSystem.playerPoints[playerInput.playerIndex] += 5;
                pointSystem.UpdatePointUI();
            }
            Debug.Log("Finish line hit by " + other.gameObject.name);
            
        }
    }
}
