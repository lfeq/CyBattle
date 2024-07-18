using System;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks {
    private void Start() {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("I'm connected to the server!!!");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel("Floor layout");
    }

    public override void OnJoinRandomFailed(short t_returnCode, string t_message) {
        PhotonNetwork.CreateRoom("Arena1");
    }
}