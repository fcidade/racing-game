using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    float cameraDistance = 2f;

    void Update()
    {
        if (!target) return;
        transform.LookAt(target);
        if (Vector3.Distance(transform.position, target.position) > cameraDistance)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + (-target.forward * cameraDistance), Time.deltaTime);
        }
    }
}
