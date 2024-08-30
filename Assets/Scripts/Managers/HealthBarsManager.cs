using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarsManager : MonoBehaviourPunCallbacks {
    public static HealthBarsManager s_instance;
    public TMP_Text[] names;
    public Image[] healthBars;
    public GameObject displayPanel;
    public TMP_Text messageText;
    public int[] kills;
    public bool teamMode = false;
    public bool noRespawn = false;
    public GameObject eliminationPanel;

    private GameObject m_waitObject;

    public void UpdateHealthBar(int index, float currentHealth) {
        photonView.RPC(nameof(RPCUpdateHealthBar), RpcTarget.AllBuffered, index, currentHealth);
    }

    [PunRPC]
    private void RPCUpdateHealthBar(int index, float currentHealth) {
        healthBars[index].fillAmount = currentHealth / 100f;
    }

    public void EnableHealthBar(int t_index, string t_name) {
        photonView.RPC(nameof(RPCEnableHealthBar), RpcTarget.AllBuffered, t_index, t_name);
    }

    [PunRPC]
    private void RPCEnableHealthBar(int t_index, string t_name) {
        names[t_index].gameObject.SetActive(true);
        names[t_index].text = t_name;
    }

    public void ReturnToLobby() {
        m_waitObject.SetActive(false);
        RoomExit();
    }

    public void Leaving() {
        StartCoroutine(nameof(BackToLobby));
    }

    private void RoomExit() {
        StartCoroutine(ToLobby());
    }

    IEnumerator BackToLobby() {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel("Lobby");
    }

    IEnumerator ToLobby() {
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void RunMessage(string t_win, string t_lose) {
        GetComponent<PhotonView>().RPC("DisplayMessage", RpcTarget.All, t_win, t_lose);
        UpdateKills(t_win);
    }

    private void UpdateKills(string t_win) {
        for (int i = 0; i < names.Length; i++) {
            if (t_win == names[i].text) {
                kills[i]++;
            }
        }
    }

    [PunRPC]
    private void DisplayMessage(string t_win, string t_lose) {
        displayPanel.SetActive(true);
        messageText.text = $"{t_win} killed {t_lose}";
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