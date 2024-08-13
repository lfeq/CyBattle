using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScript : MonoBehaviourPunCallbacks {
    private readonly TypedLobby m_killCount = new TypedLobby("killCount", LobbyType.Default);
    private readonly TypedLobby m_teamBattle = new TypedLobby("teamBattle", LobbyType.Default);
    private readonly TypedLobby m_noRespawn = new TypedLobby("noRespawn", LobbyType.Default);
    private string levelName = "";

    public void backToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void joinGameKillCount() {
        levelName = "KillCount";
        PhotonNetwork.JoinLobby(m_killCount);
    }
    
    public void joinGameTeamBattle() {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(m_teamBattle);
    }
    
    public void joinGameNoRespawn() {
        levelName = "Floor layout";
        PhotonNetwork.JoinLobby(m_noRespawn);
    }

    public override void OnJoinedLobby() {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short t_returnCode, string t_message) {
       Debug.LogWarning("Joined random room failed, creating new room");
       RoomOptions roomOptions = new RoomOptions();
       roomOptions.MaxPlayers = 6;
       PhotonNetwork.CreateRoom($"Arena {Random.Range(1, 1000)}", roomOptions);
    }

    public override void OnJoinedRoom() {
        Debug.Log($"Joining room: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel(levelName);
    }
}