using Photon.Pun;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    private GameObject[] m_players;
    private int m_myId;

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
    }

    [PunRPC]
    private void selectColor(int t_buttonNumber, int t_myId) {
        m_players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject t in m_players) {
            // Run functions on another script
        }
        gameObject.SetActive(false);
    }
}