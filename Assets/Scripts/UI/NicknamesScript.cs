using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamesScript : MonoBehaviourPunCallbacks {
    public TMP_Text[] names;
    public Image[] healthBars;
    public GameObject displayPanel;
    public TMP_Text messageText;
    public int[] kills;

    private GameObject waitObject;

    private void Start() {
        displayPanel.SetActive(false);
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

    public void runMessage(string win, string lose) {
        GetComponent<PhotonView>().RPC("DisplayMessage", RpcTarget.All, win, lose);
        updateKills(win);
    }

    private void updateKills(string win) {
        for (int i = 0; i < names.Length; i++) {
            if (win == names[i].text) {
                kills[i]++;
            }
        }
    }

    [PunRPC]
    private void DisplayMessage(string win, string lose) {
        displayPanel.SetActive(true);
        messageText.text = $"{win} killed {lose}";
        StartCoroutine(SwitchOffMessage());
    }

    IEnumerator SwitchOffMessage() {
        yield return new WaitForSeconds(3);
        GetComponent<PhotonView>().RPC("MessageOff", RpcTarget.All);
    }

    [PunRPC]
    private void MessageOff() {
        displayPanel.SetActive(false);
    }
}