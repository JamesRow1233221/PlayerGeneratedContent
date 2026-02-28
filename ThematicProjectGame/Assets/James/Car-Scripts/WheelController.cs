using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour
{
    float verticalAxis;
    float horizontalAxis;

    public float wheelSpinSpeed = 200f;
    public float steeringAngle = 35f;

    public Transform[] frontSteerPivots; // parent objects
    public Transform[] frontSpinMeshes;  

    public Transform[] rearWheelMeshes;

    public TrailRenderer[] trails;
    void Update()
    {
        SpinWheels(verticalAxis);
        SteerFrontWheels(horizontalAxis);

        if (horizontalAxis != 0)
        {
            foreach(var trail in trails)
            {
                trail.emitting = true;
            }
        }
        else
        {
            foreach(var trail in trails)
            {
                trail.emitting = false;
            }
        }
    }

    void SpinWheels(float input)
    {
        float spin = input * wheelSpinSpeed * Time.deltaTime;

        // Front wheels spin
        foreach (Transform wheel in frontSpinMeshes)
            wheel.Rotate(Vector3.right * spin, Space.Self);

        // Rear wheels spin
        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right * spin, Space.Self);
    }

    void SteerFrontWheels(float input)
    {
        float steer = input * steeringAngle;

        foreach (Transform pivot in frontSteerPivots)
        {
            // Quaternion prevents the -180° flip
            pivot.localRotation = Quaternion.Euler(0f, steer, 0f);
        }
    }
    public void OnSteer(InputAction.CallbackContext input)
    {
        horizontalAxis = input.ReadValue<float>();
    }

    public void OnThrottle(InputAction.CallbackContext input)
    {
        verticalAxis = input.ReadValue<float>();
    }
}
