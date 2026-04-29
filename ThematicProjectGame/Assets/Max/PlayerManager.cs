using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cars;
    [SerializeField] private Transform[] spawnPoints;
    int connectedPlayers = 4;

    private void Awake()
    {
        for (int i = 0; i < connectedPlayers; i++)
        {
            GameObject car = Instantiate(cars[i], spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}
