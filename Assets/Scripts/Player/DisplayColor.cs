using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DisplayColor : MonoBehaviourPunCallbacks {
    public int[] viewId;
    public AudioClip[] m_gunShotSounds;

    [SerializeField] private int[] buttonNumbers;
    [SerializeField] private Color[] colors;
    
    private GameObject m_namesObject;
    private GameObject m_waitFoPlayers;

    private void Start() {
        Debug.Log(gameObject.name);
        m_namesObject = GameObject.Find("Names Background");
        m_waitFoPlayers = GameObject.Find("Waiting Background");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GetComponent<PhotonView>().IsMine && !m_waitFoPlayers.activeInHierarchy) {
                removeData();
                roomExit();
            }
        }
    }

    public void chooseColor() {
        GetComponent<PhotonView>().RPC("assignColor", RpcTarget.AllBuffered);
    }

    public void playGunShot(string t_name, int t_weaponNumber) {
        GetComponent<PhotonView>().RPC("playSound", RpcTarget.All, t_name, t_weaponNumber);
    }

    [PunRPC]
    private void playSound(string t_name, int t_weaponNumber) {
        for (int i = 0; i < m_namesObject.GetComponent<NicknamesScript>().name.Length; i++) {
            if (t_name == m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
                GetComponent<AudioSource>().clip = m_gunShotSounds[t_weaponNumber];
                GetComponent<AudioSource>().Play();
                return;
            }
        }
    }

    [PunRPC]
    void assignColor() {
        for (int i = 0; i < viewId.Length; i++) {
            if (GetComponent<PhotonView>().ViewID == viewId[i]) {
                transform.GetChild(1).GetComponent<Renderer>().material.color = colors[i];
                m_namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(true);
                m_namesObject.GetComponent<NicknamesScript>().names[i].text = GetComponent<PhotonView>().Owner.NickName;
            }
        }
    }

    void removeData() {
        GetComponent<PhotonView>().RPC("removeMe", RpcTarget.AllBuffered);
    }

    void roomExit() {
        StartCoroutine(getReadyToLeave());
    }

    [PunRPC]
    void removeMe() {
        for (int i = 0; i < m_namesObject.gameObject.GetComponent<NicknamesScript>().names.Length; i++) {
            if (GetComponent<PhotonView>().Owner.NickName ==
                m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
                m_namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(false);
                m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator getReadyToLeave() {
        yield return new WaitForSeconds(1);
        m_namesObject.GetComponent<NicknamesScript>().leaving();
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();
    }
}