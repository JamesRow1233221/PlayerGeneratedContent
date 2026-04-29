using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] public GameObject[] cars;
    [SerializeField] public Transform[] spawnPoints;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    public PlayerInput[] players = new PlayerInput[4];
    private int connectedPlayers = 4;

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
                car.GetComponent<CarController>().playerNumber = i + 1;
                car.GetComponent<CarController>().spawnPoint = spawnPoints[i];

                var playerInput = car.GetComponent<PlayerInput>();
                playerInput.SwitchCurrentControlScheme("Gamepad", devices[i]);
                players[playerInput.playerIndex] = playerInput;
                targetGroup.AddMember(car.transform, 1f, 0f);
            }
        }
    }
}
