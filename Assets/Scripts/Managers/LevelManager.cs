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
            var weaponChanger = playerObject.GetComponent<WeaponChanger>();

            // Instantiate the camera only for the local player
            if (!weaponChanger.GetComponent<PhotonView>().IsMine) {
                return;
            }
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            weaponChanger.Initialize(virtualCamera);
        } else {
            // Fallback for offline mode or single-player testing
            var weaponChanger = Instantiate(playerPrefab, Vector3.zero, quaternion.identity).GetComponent<WeaponChanger>();
            var virtualCamera = Instantiate(cameraPrefab).GetComponent<CinemachineVirtualCamera>();
            weaponChanger.Initialize(virtualCamera);
        }
    }
}