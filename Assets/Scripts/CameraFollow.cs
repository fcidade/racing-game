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

    [SerializeField, Range(0f, 1f)]
    float cameraRotationVelocity = .5f;

    [SerializeField]
    float cameraHeight = 10f;

    new Camera camera;

    void Awake() {
        camera = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        if (!target || !camera) return;
        // transform.LookAt(target);
        camera.transform.LookAt(target.position);
        
        var newCameraPosition = target.position + (-target.forward * cameraDistance) + (target.up * cameraHeight);
        camera.transform.position = Vector3.Lerp(camera.transform.position, newCameraPosition, Time.deltaTime * cameraRotationVelocity);

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * cameraVelocity);

        // transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * cameraRotationVelocity);
        
        // transform.RotateAround(target.position, Vector3.up, cameraAngle);
    }
}
