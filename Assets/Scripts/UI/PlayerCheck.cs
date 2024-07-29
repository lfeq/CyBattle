using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerCheck : MonoBehaviour {
    [SerializeField] private int maxPlayersInRoom = 2;
    [SerializeField] private TMP_Text currentPlayersText;

    void Update() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersInRoom) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            gameObject.SetActive(false);
        }
        currentPlayersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayersInRoom} joined";
    }
}