using System;
using Photon.Pun;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    private GameObject[] m_players;
    private int m_myId;
    private GameObject m_panel;
    private GameObject m_timerGameObject;

    private void Start() {
        Cursor.visible = true;
        m_panel = GameObject.Find("ChoosePanel");
        m_timerGameObject = GameObject.Find("Timer");
    }

    public void selectButton(int t_buttonNumber) {
        m_players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject t in m_players) {
            if (!t.GetComponent<PhotonView>().IsMine) {
                continue;
            }
            m_myId = t.GetComponent<PhotonView>().ViewID;
            break;
        }
        GetComponent<PhotonView>().RPC("selectColor", RpcTarget.AllBuffered, t_buttonNumber, m_myId);
        Cursor.visible = false;
        m_panel.SetActive(false);
    }

    [PunRPC]
    private void selectColor(int t_buttonNumber, int t_myId) {
        m_players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in m_players) {
            player.GetComponent<DisplayColor>().viewId[t_buttonNumber] = t_myId;
            player.GetComponent<DisplayColor>().ChooseColor();
        }
        m_timerGameObject.GetComponent<Timer>().beginTimer();
        gameObject.SetActive(false);
    }
}