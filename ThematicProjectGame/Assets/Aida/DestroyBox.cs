using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class DestroyBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.carsDestroyed >= 4)
        {
            Debug.Log("haaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            GameManager.carsDestroyed = 0;
            PlayerInput[] players = FindFirstObjectByType<PlayerManager>().players;
            foreach(PlayerInput player in players)
            {
                FindFirstObjectByType<CinemachineTargetGroup>().AddMember(player.transform, 1f, 0f);
                player.transform.localEulerAngles = Vector3.zero;
            }
            GameManager.StartPlacingTrack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            if(FindFirstObjectByType<CinemachineTargetGroup>().FindMember(other.transform) >= 0 )
            {
                FindFirstObjectByType<CinemachineTargetGroup>().RemoveMember(other.transform);
                GameManager.carsDestroyed++;
            }

        }
    }
}
