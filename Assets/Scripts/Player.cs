using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TODO
        - Handle animations
        - Handle sound
*/

[RequireComponent(typeof(Vehicle))]
public class Player : MonoBehaviour
{

    [SerializeField, Header("Player Settings")]
    PlayerSettings playerSettings;

    Vehicle vehicle;

    public PlayerSettings PlayerSettings { get => playerSettings; set => playerSettings = value; }

    void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    void Update()
    {
        float accelerationDir = Input.GetAxis(playerSettings.VerticalInputName);
        vehicle.Accelerate(accelerationDir);

        float steeringDir = Input.GetAxis(playerSettings.HorizontalInputName);
        vehicle.SteerWheels(steeringDir);

        if (Input.GetButton(playerSettings.DriftInputName))
        {
            vehicle.Drift();
        }
        else
        {
            vehicle.StopDrift();
        }
    }

}
