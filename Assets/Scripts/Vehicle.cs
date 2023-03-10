using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour
{
    /* Suspension */
    [SerializeField]
    List<Transform> springs;

    [SerializeField]
    List<Transform> wheelsGraphics;

    [SerializeField, Header("Suspension")]
    float restDistance;

    [SerializeField]
    float springStrength = 1600f, springDamping = 100f;

    /* Acceleration */
    [SerializeField, Header("Acceleration")]
    float speed = 10f;

    /* Steering */
    [SerializeField, Header("Steering")]
    float steeringSpeed = 2f;

    [SerializeField]
    float tireMass = 5;

    [SerializeField, Range(0f, 1f)]
    float tireGripFactor = .5f;

    [SerializeField, Range(0f, 1f)]
    float driftingTireGripFactor = .9f;

    Rigidbody body;
    ParticleSystem[] wheelDustParticleSystems;
    List<TrailRenderer> wheelDriftingTrails;

    bool isDrifting = false;
    bool atLeastOneTireIsOnTheGround = false;
    bool allTiresAreOnTheGround = false;

    float accelerationDirection;
    float steeringRotationDirection;

    public void Accelerate(float direction)
    {
        accelerationDirection = direction;
    }
    public void SteerWheels(float direction)
    {
        steeringRotationDirection = direction;
    }
    public void Drift()
    {
        isDrifting = true;
    }
    public void StopDrift()
    {
        isDrifting = false;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        wheelDustParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (var wheelDustParticleSystem in wheelDustParticleSystems)
        {
            wheelDustParticleSystem.Stop();
        }

        wheelDriftingTrails = new List<TrailRenderer>();
        foreach (var trail in GetComponentsInChildren<TrailRenderer>())
        {
            if (trail.tag == "Drifting")
            {
                trail.emitting = false;
                wheelDriftingTrails.Add(trail);
            }
        }
    }

    void Update()
    {
        var isCarMoving = body.velocity.magnitude > .5;
        foreach (var particleSystem in wheelDustParticleSystems)
        {
            if (isCarMoving && !particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
            else if (!isCarMoving && !particleSystem.isStopped)
            {
                particleSystem.Stop();
            }
        }

        foreach (var trail in wheelDriftingTrails)
        {
            trail.emitting = isDrifting && allTiresAreOnTheGround;
        }
    }

    void FixedUpdate()
    {
        RotateWheels(steeringRotationDirection);

        atLeastOneTireIsOnTheGround = false;
        allTiresAreOnTheGround = true;
        for (var i = 0; i < springs.Count; i++)
        {
            var spring = springs[i];
            var wheelGraphics = wheelsGraphics[i];
            atLeastOneTireIsOnTheGround |= CalculateSpringSuspension(spring, wheelGraphics);
            allTiresAreOnTheGround &= atLeastOneTireIsOnTheGround;
        }

        /* Rotation (Allow rotation in the air) */
        RotateCar(accelerationDirection, steeringRotationDirection);
        AccelerateCar(accelerationDirection, atLeastOneTireIsOnTheGround);
    }

    private void AccelerateCar(float inputDirection, bool atLeastOneTireIsOnTheGround)
    {
        if (atLeastOneTireIsOnTheGround)
        {
            /* Acceleration */
            body.AddForce(transform.forward * inputDirection * speed);

            /* Steering */
            Vector3 steeringDir = transform.right;
            float steeringVel = Vector3.Dot(steeringDir, body.velocity);
            float desiredVelChange = -steeringVel * (isDrifting ? driftingTireGripFactor : tireGripFactor);
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            body.AddForce(steeringDir * tireMass * desiredAccel);
        }
    }

    private void RotateCar(float inputDirection, float steeringRotationDirection)
    {
        if (inputDirection != 0 || body.velocity.z > 1)
        {
            body.AddTorque(transform.up * steeringRotationDirection * steeringSpeed * (inputDirection == 0 ? 1 : inputDirection));
        }
    }

    private void RotateWheels(float steeringRotationDirection)
    {
        var rot = Quaternion.Euler(wheelsGraphics[0].localRotation.x, (30f * steeringRotationDirection), wheelsGraphics[0].localRotation.z);

        wheelsGraphics[0].localRotation = Quaternion.Lerp(wheelsGraphics[0].localRotation, rot, Time.deltaTime * 3);
        wheelsGraphics[1].localRotation = Quaternion.Lerp(wheelsGraphics[1].localRotation, rot, Time.deltaTime * 3);
    }

    private bool CalculateSpringSuspension(Transform spring, Transform wheelGraphics)
    {

        wheelGraphics.Rotate(body.velocity.z, 0, 0);
        DebugSpring(spring);

        RaycastHit hit;
        var rayHadHit = Physics.Raycast(spring.position, -transform.up, out hit, restDistance);
        Debug.DrawRay(spring.position, -transform.up * restDistance, Color.magenta);

        if (rayHadHit)
        {
            wheelGraphics.position = Vector3.Lerp(wheelGraphics.position, hit.point + Vector3.up * .4f, Time.deltaTime * 2);

            /* Suspension */
            // The spring force direction can be relative to the tire up, or the rigidbody up, or to the ground normal
            Vector3 springForceDirection = transform.up;

            // Distance between the spring rest position and the ground (raycast)
            float offset = restDistance - hit.distance;

            // Current spring world space velocity
            Vector3 currSpringVelocity = body.GetPointVelocity(spring.position);

            // Calculate the velocity of the spring along with the spring direction
            // Since springForceDirection is a unit vector, it returns the magnitude of the spring world velocity
            float vel = Vector3.Dot(currSpringVelocity, springForceDirection);

            // Force of the spring reduced by the spring damping (thickness of the fluid inside the spring)
            float force = (offset * springStrength) - (vel * springDamping);

            // Apply force to the position of the spring
            body.AddForceAtPosition(springForceDirection * force, spring.position);
            Debug.DrawRay(spring.position, springForceDirection * force, Color.yellow);
        }
        else
        {
            wheelGraphics.position = Vector3.Lerp(wheelGraphics.position, spring.position, Time.deltaTime * 2);
        }

        return rayHadHit;
    }

    private static void DebugSpring(Transform spring)
    {
        Debug.DrawRay(spring.position, spring.up, Color.green);
        Debug.DrawRay(spring.position, spring.right, Color.red);
        Debug.DrawRay(spring.position, spring.forward, Color.blue);
    }
}
