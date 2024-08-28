using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviourPunCallbacks {
    public static LevelManager s_instance;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;

    private void Awake() {
        SpawnPlayer();
        if (s_instance == null) {
            s_instance = this;
        }
        else if (s_instance != this) {
            // Destroy any duplicate instances
            Destroy(gameObject);
        }
    }

    public void SpawnPlayer() {
        if (PhotonNetwork.IsConnected) {
            // Only instantiate the player and camera for the local player
            Debug.LogError("PhotonNetwork.IsConnected");
            if (!PhotonNetwork.IsConnectedAndReady) {
                return;
            }
            var playerObject = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPoint(), quaternion.identity);
            var playerManager = playerObject.GetComponent<PlayerManager>();
            Debug.LogError("Spawned player");
            // Instantiate the camera only for the local player
            if (!playerManager.GetComponent<PhotonView>().IsMine) {
                return;
            }
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            Debug.LogError("Spawned camera");
            playerManager.Initialize(virtualCamera);
            Debug.LogError("Initialized camera");
        }
        else {
            // Fallback for offline mode or single-player testing
            var playerManager = Instantiate(playerPrefab, GetRandomSpawnPoint(), quaternion.identity)
                .GetComponent<PlayerManager>();
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            playerManager.Initialize(virtualCamera);
        }
    }

    public Vector3 GetRandomSpawnPoint() {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}