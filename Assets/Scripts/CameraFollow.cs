using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    float cameraDistance = 2f;

    [SerializeField]
    float cameraVelocity = 3f;

    [SerializeField]
    float cameraHeight = 10f;

    void Update()
    {
        if (!target) return;
        transform.LookAt(target);

        Vector3 newCameraPosition = target.position + (-Vector3.forward * cameraDistance) + (Vector3.up * cameraHeight);
        transform.position = Vector3.Lerp(transform.position, newCameraPosition, Time.deltaTime * cameraVelocity);

        // transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime);
        
        // transform.RotateAround(target.position, Vector3.up, cameraAngle);
    }
}
