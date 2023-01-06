using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstantiator : MonoBehaviour
{

    [SerializeField]
    List<PlayerSettings> playerSettings;

    [SerializeField]
    Player playerPrefab;
    [SerializeField]
    CameraFollow vehicleCameraPrefab;

    [SerializeField]
    List<Transform> spawnLocations;

    List<Player> players;

    void Awake()
    {
        players = new List<Player>();

        float verticalScreenSize = 1 / playerSettings.Count;

        for (var i = 0; i < playerSettings.Count; i++)
        {
            var settings = playerSettings[i];
            var player = SpawnPlayer(settings, spawnLocations[i]);
            
            float verticalPosition = i * verticalScreenSize;
            var camera = SpawnCamera(player.transform, verticalPosition, verticalScreenSize);

            if (i > 0)
            {
                // Keep only the first audio listener active
                camera.RemoveAudioListener();
            }

            players.Add(player);
        }

    }

    Player SpawnPlayer(PlayerSettings playerSettings, Transform spawnLocation)
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = spawnLocation.position;
        player.PlayerSettings = playerSettings;
        return player;
    }

    CameraFollow SpawnCamera(Transform target, float verticalPosition, float verticalScreenSize)
    {
        var camera = Instantiate(vehicleCameraPrefab);
        camera.SetTarget(target);
        camera.SetCameraViewport(new Rect(0, verticalPosition, 1, verticalScreenSize));
        return camera;
    }

}
