using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cars;
    [SerializeField] private Transform[] spawnPoints;
    int connectedPlayers = 4;

    private void Awake()
    {
        var devices = InputSystem.devices.OfType<Gamepad>().ToList();

        for (int i = 0; i < connectedPlayers; i++)
        {
            if (i >= devices.Count)
            {
                Debug.LogWarning($"Not enough gamepads connected for player {i + 1}. Expected {connectedPlayers}, but found {devices.Count}.");
            }
            else
            {
                Debug.Log($"Player {i + 1} assigned to gamepad: {devices[i].displayName}");

                GameObject car = Instantiate(cars[i], spawnPoints[i].position, spawnPoints[i].rotation);

                var playerInput = car.GetComponent<PlayerInput>();
                playerInput.SwitchCurrentControlScheme("Gamepad", devices[i]);
            }
        }
    }
}
