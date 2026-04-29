using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Camera thisCamera;

    public Vector3 targetPos;
    public float lerpAmount = 0.3f;

    private void Awake()
    {
        targetPos = transform.position;
        thisCamera = GetComponent<Camera>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        thisCamera.transform.position = Vector3.Lerp(thisCamera.transform.position, targetPos, lerpAmount);
    }
}
