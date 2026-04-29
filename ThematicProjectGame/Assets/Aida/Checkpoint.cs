using UnityEngine;
using UnityEngine.InputSystem;

public class Checkpoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            Debug.Log("Checkpoint hit by " + other.gameObject.name);
            if(other.gameObject.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
            {
                int playerNumber = playerInput.playerIndex; // Assuming playerIndex starts from 0
                GameCamera gameCamera = FindFirstObjectByType<GameCamera>();
                gameCamera.playerCheckpoints[playerNumber]++;
                gameCamera.UpdateTarget();

            }
        }
    }
}
