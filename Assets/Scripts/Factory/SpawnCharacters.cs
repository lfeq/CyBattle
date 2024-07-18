using System;
using Photon.Pun;
using UnityEngine;

public class SpawnCharacters : MonoBehaviour {
    public GameObject character;
    public Transform[] spawnPoints;

    private void Start() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.Instantiate(character.name, spawnPoints[PhotonNetwork.CountOfPlayers - 1].position,
                spawnPoints[PhotonNetwork.CountOfPlayers - 1].rotation);
        }
    }
}