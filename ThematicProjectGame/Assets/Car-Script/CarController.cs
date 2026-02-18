using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float moveInput;

    public Rigidbody sphereRB;

    public float fwdSpeed;

    void Start()
    {
        sphereRB.transform.parent = null;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        moveInput *= fwdSpeed;

        // set cars position to sphere
        transform.position = sphereRB.transform.position;
    }

    private void FixedUpdate()
    {
        sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
    }
}
