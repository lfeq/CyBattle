using System;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviourPunCallbacks {
    public static LevelManager s_instance;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private HealthBarsManager healthBarsManager;

    private void Start() {
        // Wait for the player to fully join the room before spawning
        if (PhotonNetwork.IsConnectedAndReady) {
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom() {
        //Debug.LogError("Joined room, now spawning player.");
        SpawnPlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        //Debug.LogError($"Player {newPlayer.NickName} joined the room");
    }

    public void SpawnPlayer() {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady) {
            // Only instantiate the player for the local client
            var playerObject = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPoint(), quaternion.identity);
            var playerManager = playerObject.GetComponent<PlayerManager>();

            // Instantiate the camera only for the local client
            if (playerManager.GetComponent<PhotonView>().IsMine) {
                Debug.LogError("is mine");
                var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
                playerManager.Initialize(virtualCamera, healthBarsManager);
            }
        }
        else {
            // Fallback for offline mode or single-player testing
            Debug.LogError("Player being instantiated outside network");
            var playerManager = Instantiate(playerPrefab, GetRandomSpawnPoint(), quaternion.identity)
                .GetComponent<PlayerManager>();
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            playerManager.Initialize(virtualCamera, healthBarsManager);
        }
    }

    public Vector3 GetRandomSpawnPoint() {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}