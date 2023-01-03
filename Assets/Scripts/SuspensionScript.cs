using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionScript : MonoBehaviour
{

    [SerializeField]
    List<GameObject> springs;

    [SerializeField, Header("Suspension")]
    float restDistance;

    [SerializeField]
    float springStrength = 1600f, springDamping = 100f;

    [SerializeField, Header("Acceleration")]
    float speed = 10f;

    [SerializeField, Header("Steering")]
    float tireMass = 5f;
    [SerializeField, Range(0f, 1f)]
    float tireGripFactor = .5f;


    [SerializeField, Header("Anti Roll")]
    float antiRollFlipStrength = 10f;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        var torqueDirection = Input.GetAxis("Vertical");
        var steeringRotation = Input.GetAxis("Horizontal") * 45f;
        springs[0].transform.localRotation = Quaternion.Euler(0, steeringRotation, 0);
        springs[1].transform.localRotation = Quaternion.Euler(0, steeringRotation, 0);

        foreach (var spring in springs)
        {

            Debug.DrawRay(spring.transform.position, spring.transform.up, Color.green);
            Debug.DrawRay(spring.transform.position, spring.transform.right, Color.red);
            Debug.DrawRay(spring.transform.position, spring.transform.forward, Color.blue);

            RaycastHit hit;
            var rayHadHit = Physics.Raycast(spring.transform.position, -transform.up, out hit, restDistance);
            Debug.DrawRay(spring.transform.position, -transform.up * restDistance, Color.magenta);

            if (rayHadHit)
            {
                /* Suspension */
                // The spring force direction can be relative to the tire up, or the rigidbody up, or to the ground normal
                Vector3 springForceDirection = transform.up;

                // Distance between the spring rest position and the ground (raycast)
                float offset = restDistance - hit.distance;

                // Current spring world space velocity
                Vector3 currSpringVelocity = body.GetPointVelocity(spring.transform.position);

                // Calculate the velocity of the spring along with the spring direction
                // Since springForceDirection is a unit vector, it returns the magnitude of the spring world velocity
                float vel = Vector3.Dot(currSpringVelocity, springForceDirection);

                // Force of the spring reduced by the spring damping (thickness of the fluid inside the spring)
                float force = (offset * springStrength) - (vel * springDamping);

                // Apply force to the position of the spring
                body.AddForceAtPosition(springForceDirection * force, spring.transform.position);
                Debug.DrawRay(spring.transform.position, springForceDirection * force, Color.yellow);

                /* Acceleration */
                body.AddForceAtPosition(spring.transform.forward * torqueDirection * speed, spring.transform.position);
                /* Steering */
                Vector3 steeringDir = spring.transform.right;
                float steeringVel = Vector3.Dot(steeringDir, currSpringVelocity);
                float desiredVelChange = -steeringVel * tireGripFactor;
                float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
                body.AddForceAtPosition(steeringDir * tireMass * desiredAccel, spring.transform.position);
            }
        }

        if (Mathf.Abs(transform.localRotation.eulerAngles.z) > 90f)
        {
            // transform.rotation = Quaternion.identity;
            // body.AddRelativeTorque(0f, 0f, antiRollFlipStrength, ForceMode.Acceleration);
        }
    }
}
