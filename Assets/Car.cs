using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 50f;
    public float acceleration = 5f;
    public float deceleration = 5f;
    public float brakeForce = 100f; // Force applied to brakes

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private bool isBraking = false;
    private bool isAccelerating = false;

    // Wheel Colliders and Wheel Mesh Transforms
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    public Transform wheelFLTransform;
    public Transform wheelFRTransform;
    public Transform wheelRLTransform;
    public Transform wheelRRTransform;

    // Audio Sources
    public AudioSource engineSound;
    public AudioSource brakingSound;
    public AudioSource idleSound;

    // Audio Clips
    public AudioClip engineClip;
    public AudioClip brakingClip;
    public AudioClip idleClip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize audio sources
        engineSound.clip = engineClip;
        brakingSound.clip = brakingClip;
        idleSound.clip = idleClip;

        // Ensure sound sources are set up correctly
        engineSound.loop = true;
        brakingSound.loop = false;
        idleSound.loop = true;

        engineSound.Play();
        idleSound.Play();
    }

    void FixedUpdate()
    {
        // Check for input and update current speed
        isBraking = Input.GetKey(KeyCode.P);
        isAccelerating = Input.GetKey(KeyCode.Space);

        // Calculate speed based on input
        if (isAccelerating && !isBraking)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + acceleration * Time.deltaTime, 0, speed);
            // Play engine sound when accelerating
            if (!engineSound.isPlaying)
            {
                engineSound.Play();
            }
            if (idleSound.isPlaying)
            {
                idleSound.Stop();
            }
        }
        else if (isBraking)
        {
            // Apply braking logic
            currentSpeed = Mathf.Clamp(currentSpeed - brakeForce * Time.deltaTime, 0, speed);
            // Play braking sound
            if (!brakingSound.isPlaying)
            {
                brakingSound.Play();
            }
            if (engineSound.isPlaying)
            {
                engineSound.Stop();
            }
            if (idleSound.isPlaying)
            {
                idleSound.Stop();
            }
        }
        else
        {
            // Apply deceleration when neither accelerating nor braking
            currentSpeed = Mathf.Clamp(currentSpeed - deceleration * Time.deltaTime, 0, speed);
            // Play idle sound when stopped
            if (!idleSound.isPlaying)
            {
                idleSound.Play();
            }
            if (engineSound.isPlaying)
            {
                engineSound.Stop();
            }
            if (brakingSound.isPlaying)
            {
                brakingSound.Stop();
            }
        }

        // Input for turning
        float turnInput = Input.GetAxis("Horizontal");

        // Movement and rotation
        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        Quaternion turn = Quaternion.Euler(0f, turnInput * turnSpeed * Time.deltaTime, 0f);

        rb.MovePosition(rb.position + movement);
        rb.MoveRotation(rb.rotation * turn);

        // Update Wheel Colliders
        UpdateWheelColliders(turnInput);

        // Update Wheel Meshes
        UpdateWheelMeshes();
    }

    void UpdateWheelColliders(float turnInput)
    {
        // Apply steering angle to front wheels
        wheelFL.steerAngle = turnInput * 30f; // Adjust 30f to the desired max steering angle
        wheelFR.steerAngle = turnInput * 30f;

        // Apply braking force if braking
        if (isBraking)
        {
            wheelRL.brakeTorque = brakeForce;
            wheelRR.brakeTorque = brakeForce;
        }
        else
        {
            wheelRL.brakeTorque = 0f;
            wheelRR.brakeTorque = 0f;
        }
    }

    void UpdateWheelMeshes()
    {
        // Update wheel meshes based on wheel colliders
        UpdateWheelMesh(wheelFL, wheelFLTransform);
        UpdateWheelMesh(wheelFR, wheelFRTransform);
        UpdateWheelMesh(wheelRL, wheelRLTransform);
        UpdateWheelMesh(wheelRR, wheelRRTransform);
    }

    void UpdateWheelMesh(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
