using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftingVehicle : MonoBehaviour
{

    [SerializeField, Header("Movement")]
    float speed = 100f, maxSpeed = 200f, steeringAngle = 20f;


    [SerializeField, Header("Suspension Spring")]
    float springRestDistance = 5f, springStrength = 100, springDampling = 100;

    [SerializeField]
    Transform[] tires;

    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        foreach (var tire in tires)
        {
            SuspendTire(tire);
        }

        var accelDirection = Input.GetAxis("Vertical");
        var steeringDirection = Input.GetAxis("Horizontal");
        if (accelDirection == 0) return;

        body.velocity += transform.forward * speed * accelDirection * Time.deltaTime;
        body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
        body.MoveRotation(Quaternion.Euler(body.rotation.eulerAngles.x, body.rotation.eulerAngles.y + steeringDirection * steeringAngle * Time.deltaTime, body.rotation.eulerAngles.z));

    }

    void SuspendTire(Transform tireTransform)
    {
        RaycastHit hit;
        bool rayHadHit = Physics.Raycast(tireTransform.position, -tireTransform.up, out hit, springRestDistance);
        Debug.DrawRay(tireTransform.position, -tireTransform.up * springRestDistance, Color.green);
        if (!rayHadHit) return;

        Vector3 springDir = tireTransform.up;
        Vector3 tireWorldVel = body.GetPointVelocity(tireTransform.position);
        float offset = springRestDistance - hit.distance;
        float vel = Vector3.Dot(springDir, tireWorldVel);
        float force = (offset * springStrength) - (vel * springDampling);

        body.AddForceAtPosition(springDir * force, tireTransform.position);

    }
}
