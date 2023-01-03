using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    [SerializeField]
    Transform frontTireA, frontTireB, backTireA, backTireB;

    [SerializeField]
    float carTopSpeed = 100f, accelerationSpeed = 100f;

    [SerializeField]
    AnimationCurve powerCurve;

    Rigidbody body;
    float acellerationInputDirection = 0;
    float frontTiresRotation = 0;

    [SerializeField]
    float suspensionRestDist = 1, springStrength = 100f, springDamper = 15f;

    [SerializeField]
    float tireMass, tireGripFactor;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        acellerationInputDirection = Input.GetAxis("Vertical");
        frontTiresRotation = Input.GetAxis("Horizontal");

        frontTireA.localRotation = Quaternion.Euler(0, 45f * frontTiresRotation, 0);
        frontTireB.localRotation = Quaternion.Euler(0, 45f * frontTiresRotation, 0);

        Tire(frontTireA);
        Tire(frontTireB);
        Tire(backTireA);
        Tire(backTireB);
    }

    void Tire(Transform tireTransform)
    {
        RaycastHit hit;
        bool rayDidHit = Physics.Raycast(tireTransform.position, -tireTransform.up, out hit, 1f);

        if (!rayDidHit) return;

        Suspension(tireTransform, hit);
        // Steering(tireTransform);
        AccelerationAndBraking(tireTransform);
        DebugTire(tireTransform);

    }

    void Suspension(Transform tireTransform, RaycastHit hit)
    {
        Vector3 springDir = tireTransform.up;
        Vector3 tireWorldVel = body.GetPointVelocity(tireTransform.position);
        float offset = suspensionRestDist - hit.distance;
        float vel = Vector3.Dot(springDir, tireWorldVel);
        float force = (offset * springStrength) - (vel * springDamper);
        body.AddForceAtPosition(springDir * force, tireTransform.position);
        Debug.DrawRay(tireTransform.position, springDir * force, Color.green);
    }

    void Steering(Transform tireTransform)
    {
        Vector3 steeringDir = tireTransform.right;
        Vector3 tireWorldVel = body.GetPointVelocity(tireTransform.position);

        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
        float desiredVelChange = -steeringVel * tireGripFactor;
        float desiredAccel = desiredVelChange / Time.deltaTime;
        body.AddForceAtPosition(steeringDir * tireMass * desiredAccel, tireTransform.position);
    }

    void AccelerationAndBraking(Transform tireTransform)
    {
        if (acellerationInputDirection == 0) return;
        Vector3 accelDir = tireTransform.forward;
        float carSpeed = Vector3.Dot(transform.forward, body.velocity);
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);
        float availableTorque = powerCurve.Evaluate(normalizedSpeed) * acellerationInputDirection;
        body.AddForceAtPosition(accelDir * availableTorque * accelerationSpeed, tireTransform.position);
        Debug.DrawRay(tireTransform.position, accelDir * availableTorque * 2, Color.cyan);
    }

    void DebugTire(Transform tireTransform)
    {
        Debug.DrawRay(tireTransform.position, tireTransform.forward, Color.blue);
        Debug.DrawRay(tireTransform.position, tireTransform.right, Color.red);
        Debug.DrawRay(tireTransform.position, tireTransform.up, Color.green);
    }
}
