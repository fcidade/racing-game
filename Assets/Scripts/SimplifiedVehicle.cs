using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifiedVehicle : MonoBehaviour
{
    /* Suspension */
    [SerializeField]
    List<Transform> springs;

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

    /* Anti Roll */
    [SerializeField, Header("Anti Roll")]
    float antiRollFlipStrength = 10f;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        var inputDirection = Input.GetAxis("Vertical");
        var steeringRotationDirection = Input.GetAxis("Horizontal");

        bool atLeastOneTireIsOnTheGround = false;
        foreach (var spring in springs)
        {

            Debug.DrawRay(spring.position, spring.up, Color.green);
            Debug.DrawRay(spring.position, spring.right, Color.red);
            Debug.DrawRay(spring.position, spring.forward, Color.blue);

            RaycastHit hit;
            var rayHadHit = Physics.Raycast(spring.position, -transform.up, out hit, restDistance);
            Debug.DrawRay(spring.position, -transform.up * restDistance, Color.magenta);

            atLeastOneTireIsOnTheGround |= rayHadHit;
            if (rayHadHit)
            {
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
        }

        /* Rotation (Allow rotation in the air) */
        body.AddTorque(transform.up * steeringRotationDirection * steeringSpeed * inputDirection);

        if (atLeastOneTireIsOnTheGround)
        {
            /* Acceleration */
            body.AddForce(transform.forward * inputDirection * speed);

            // /* Steering */
            Vector3 steeringDir = transform.right;
            float steeringVel = Vector3.Dot(steeringDir, body.velocity);
            float desiredVelChange = -steeringVel * tireGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            body.AddForce(steeringDir * tireMass * desiredAccel);
        }

        // if (Mathf.Abs(transform.localRotation.eulerAngles.z) > 90f)
        // {
        //     body.AddRelativeTorque(0f, 0f, antiRollFlipStrength, ForceMode.Acceleration);
        // }
    }
}
