using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    bool isControllable = false;
    Vector3 startingPosition;
    Vector3 startingRotation;

    public Transform spawnPoint;

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
    Vector3 sphereRBstartingRotation;
    public Rigidbody carRB;
    Vector3 carRBstartingPosition;
    Vector3 carRBstartingRotation;

    public LayerMask groundLayer;

    public float alignToGroundTime;

    public InputActionAsset inputActions;

    private InputAction m_accelerate;
    private InputAction m_steer;

    public int playerNumber;

    private void OnEnable()
    {
        inputActions.FindActionMap("Car").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Car").Disable();
    }

    private void Awake()
    {
        inputActions = GetComponent<PlayerInput>().actions;

        m_accelerate = inputActions.FindAction("Accelerate");
        m_steer = inputActions.FindAction("Steer");
    }

    void Start()
    {
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;

        normalDrag = sphereRB.linearDamping;

        startingPosition = spawnPoint.position;
        startingRotation = spawnPoint.eulerAngles;
        sphereRBstartingPosition = sphereRB.transform.position;
        sphereRBstartingRotation = sphereRB.transform.eulerAngles;
        carRBstartingPosition = carRB.transform.position;
        carRBstartingRotation = carRB.transform.eulerAngles;    

        GameManager.stateSwitched.AddListener(StateSwitched);
    }

    void Update()
    {
        if (isControllable)
        {
            moveInput = m_accelerate.ReadValue<float>();
            turnInput = m_steer.ReadValue<float>();

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
        else
        {
            sphereRB.linearVelocity = Vector3.zero;
            sphereRB.angularVelocity = Vector3.zero;
        }
    }

    void StateSwitched(GameStates oldState, GameStates newState)
    {
        switch (newState)
        {
            case GameStates.Race:
                isControllable = true;
                Debug.Log(playerNumber + " starting at " + sphereRB.transform.position.ToString());
                break;

            case GameStates.Track:
                isControllable = false;
                sphereRB.linearVelocity = Vector3.zero;
                sphereRB.angularVelocity = Vector3.zero;
                carRB.angularVelocity = Vector3.zero;
                carRB.linearVelocity = Vector3.zero;

                sphereRB.MovePosition(sphereRBstartingPosition);
                sphereRB.MoveRotation(Quaternion.Euler(sphereRBstartingRotation));
                carRB.MovePosition(carRBstartingPosition);
                carRB.MoveRotation(Quaternion.Euler(carRBstartingRotation));
                transform.position = startingPosition;
                transform.rotation = Quaternion.Euler(startingRotation);

                Debug.Log(playerNumber + " reset to " + sphereRB.transform.position.ToString());
                break;
            
            case GameStates.PreLoadedRace:
                isControllable = true;
                Debug.Log(playerNumber + " starting at " + sphereRB.transform.position.ToString());
                TrackConnecting connector = FindFirstObjectByType<TrackConnecting>();
                connector.trackPacerCamera.SetActive(false);
                connector.RaceCamera.SetActive(true);
                break;
        }
    }
}
