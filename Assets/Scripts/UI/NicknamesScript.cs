using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamesScript : MonoBehaviourPunCallbacks {
    public TMP_Text[] names;
    public Image[] healthBars;

    private GameObject waitObject;

    private void Start() {
        waitObject = GameObject.Find("Waiting Background");
        foreach (TMP_Text nameText in names) {
            nameText.gameObject.SetActive(false);
        }
    }

    public void returnToLobby() {
        waitObject.SetActive(false);
        roomExit();
    }

    public void leaving() {
        StartCoroutine(nameof(backToLobby));
    }

    private void roomExit() {
        StartCoroutine(toLobby());
    }
    
    IEnumerator backToLobby() {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel("Lobby");
    }

    IEnumerator toLobby() {
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel("Lobby");
    }
}