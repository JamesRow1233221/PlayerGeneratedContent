using UnityEngine;

public class WheelController : MonoBehaviour
{

    public float wheelSpinSpeed = 200f;
    public float steeringAngle = 35f;

    public Transform[] frontSteerPivots; // parent objects
    public Transform[] frontSpinMeshes;  

    public Transform[] rearWheelMeshes;
    void Update()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        SpinWheels(verticalAxis);
        SteerFrontWheels(horizontalAxis);
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
}
