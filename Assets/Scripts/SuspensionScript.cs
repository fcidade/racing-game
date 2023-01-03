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

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (var spring in springs)
        {
            RaycastHit hit;
            var rayHadHit = Physics.Raycast(spring.transform.position, -transform.up, out hit, restDistance);
            Debug.DrawRay(spring.transform.position, -transform.up * restDistance, Color.green);

            if (rayHadHit)
            {
                // Suspension
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
            }
        }
    }
}
