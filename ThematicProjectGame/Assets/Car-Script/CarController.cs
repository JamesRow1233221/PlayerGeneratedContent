using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    public float airDrag;
    public float groundDrag;

    public Rigidbody sphereRB;

    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;
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

        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        transform.Rotate(0, newRotation, 0, Space.World);

        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

        if (isCarGrounded)
        {
            sphereRB.linearDamping = groundDrag;
        }
        else
        {
            sphereRB.linearDamping = airDrag;
        }
    }

            private void FixedUpdate()
            {
                if (isCarGrounded)
                {
                    sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
                }
                else
                {
                    sphereRB.AddForce(transform.up * -30f);
                }
            }
}
