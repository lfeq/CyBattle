using Cinemachine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;

    private void Start() {
        if (PhotonNetwork.IsConnected) {
            // Only instantiate the player and camera for the local player
            if (!PhotonNetwork.IsConnectedAndReady) {
                return;
            }
            var playerObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, quaternion.identity);
            var playerManager = playerObject.GetComponent<PlayerManager>();

            // Instantiate the camera only for the local player
            if (!playerManager.GetComponent<PhotonView>().IsMine) {
                return;
            }
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            playerManager.Initialize(virtualCamera);
        } else {
            // Fallback for offline mode or single-player testing
            var playerManager = Instantiate(playerPrefab, Vector3.zero, quaternion.identity).GetComponent<PlayerManager>();
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            playerManager.Initialize(virtualCamera);
        }
    }
}