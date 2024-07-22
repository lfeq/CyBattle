using System;
using Photon.Pun;
using UnityEngine;

public class SpawnCharacters : MonoBehaviour {
    public GameObject character;
    public Transform[] spawnPoints;
    public GameObject[] weapons;
    public Transform[] weaponSpawnPoints;
    public float weaponRespawnTime = 10;

    private void Start() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.Instantiate(character.name, spawnPoints[PhotonNetwork.CountOfPlayers - 1].position,
                spawnPoints[PhotonNetwork.CountOfPlayers - 1].rotation);
        }
    }

    public void spawnWeaponStart() {
        for (int i = 0; i < weapons.Length; i++) {
            PhotonNetwork.Instantiate(weapons[i].name, weaponSpawnPoints[i].position, weaponSpawnPoints[i].rotation);
        }
    }
}