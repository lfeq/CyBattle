using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
        levelName = "SampleScene";
        //levelName = "KillCount"; // Original level
        PhotonNetwork.JoinLobby(m_killCount);
    }
    
    public void joinGameTeamBattle() {
        levelName = "TeamBattle";
        PhotonNetwork.JoinLobby(m_teamBattle);
    }
    
    public void joinGameNoRespawn() {
        levelName = "NoRespawn";
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