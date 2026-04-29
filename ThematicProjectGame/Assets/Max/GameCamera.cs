using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCamera : MonoBehaviour
{
    public CinemachineCamera thisCamera;

    public Vector3 targetPos;
    public float lerpAmount = 0.3f;

    public int[] playerCheckpoints = new int[4];
    private int currentLeadPlayer = 0;
    [SerializeField] private PlayerManager playerManager;

    private void Awake()
    {
        targetPos = transform.position;
        thisCamera = GetComponent<CinemachineCamera>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateTarget();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateTarget()
    {
        for(int i=0; i < playerCheckpoints.Length; i++)
        {
            if(playerCheckpoints[i] > playerCheckpoints[currentLeadPlayer])
            {
                currentLeadPlayer = i;
            }
        }

        thisCamera.Follow = playerManager.players[currentLeadPlayer].transform;
        thisCamera.LookAt = playerManager.players[currentLeadPlayer].transform;


    }
}
