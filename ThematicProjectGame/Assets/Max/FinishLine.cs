using UnityEngine;

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
        if(other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            GameManager.StartPlacingTrack();
        }
    }
}
