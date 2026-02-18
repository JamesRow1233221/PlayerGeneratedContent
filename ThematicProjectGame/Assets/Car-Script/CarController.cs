using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float moveInput;
    private float turnInput;
    public Rigidbody sphereRB;

    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;

    void Start()
    {
        sphereRB.transform.parent = null;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal"); 

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        // set cars position to sphere
        transform.position = sphereRB.transform.position;

        float newRotation = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0, newRotation, 0, Space.World);
    }

    private void FixedUpdate()
    {
        sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
    }
}
