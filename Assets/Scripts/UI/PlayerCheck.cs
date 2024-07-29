using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerCheck : MonoBehaviour {
    [SerializeField] private int maxPlayersInRoom = 2;
    [SerializeField] private TMP_Text currentPlayersText;
    [SerializeField] private GameObject hint1, hint2, enterButton;

    private void Start() {
        enterButton.SetActive(false);
    }

    void Update() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersInRoom) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            hint1.SetActive(false);
            hint2.SetActive(false);
            enterButton.SetActive(true);
        }
        currentPlayersText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayersInRoom} joined";
    }

    public void enterArena() {
        gameObject.SetActive(false);
    }
}