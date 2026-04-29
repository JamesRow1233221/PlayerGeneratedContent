using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    bool isControllable = false;
    Vector3 startingPosition;
    Vector3 startingRotation;

    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;

    public Rigidbody sphereRB;
    Vector3 sphereRBstartingPosition;
    public Rigidbody carRB;

    public LayerMask groundLayer;

    public float alignToGroundTime;

    void Start()
    {
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;

        normalDrag = sphereRB.linearDamping;
        startingPosition = transform.position;
        startingRotation = transform.eulerAngles;
        sphereRBstartingPosition = sphereRB.transform.position;

        GameManager.stateSwitched.AddListener(StateSwitched);
    }

    void Update()
    {
        if (isControllable)
        {
            float triggerAxis = Input.GetAxisRaw("Throttle");
            moveInput = triggerAxis;
            turnInput = Input.GetAxisRaw("Horizontal");

            float newRot = turnInput * turnSpeed * Time.deltaTime * moveInput;

            if (isCarGrounded)
            {
                transform.Rotate(0, newRot, 0, Space.World);
            }

            transform.position = sphereRB.transform.position;



            RaycastHit hit;
            isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

            moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

            sphereRB.linearDamping = isCarGrounded ? normalDrag : modifiedDrag;
        }
    }

    private void FixedUpdate()
    {
        if (isControllable)
        {
            if (isCarGrounded)
            {
                sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
            }
            else
            {
                sphereRB.AddForce(transform.up * -40f);
            }

            carRB.MoveRotation(transform.rotation);
        }
    }

    void StateSwitched(GameStates oldState, GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Race:
                isControllable = true;
                break;

            case GameStates.Track:
                isControllable = false;
                sphereRB.transform.position = sphereRBstartingPosition;
                transform.position = startingPosition;
                transform.eulerAngles = startingRotation;
                break;
        }
    }
}
