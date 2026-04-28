using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXCar : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    private float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carAudio;

    public float minPitch; 
    public float maxPitch;
    private float pitchFromCar;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carAudio = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        EngineSound();
    }
    void EngineSound()
    {
        currentSpeed = carRb.linearVelocity.magnitude;
        pitchFromCar = carRb.linearVelocity.magnitude / 50f;

        if (currentSpeed < minSpeed)
        {
            carAudio.pitch = minPitch;
        }
        
        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            carAudio.pitch = minPitch + pitchFromCar;
        }
    }
}


