using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DisplayColor : MonoBehaviourPunCallbacks {
    public int[] viewId;
    [FormerlySerializedAs("m_gunShotSounds")] public AudioClip[] gunShotSounds;

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
                RemoveData();
                RoomExit();
            }
        }
        if (GetComponent<Animator>().GetBool("Hit")) {
            StartCoroutine(Recover());
        }
    }

    public void DeliverDamage(string name, float damageAmount) {
        GetComponent<PhotonView>().RPC("GunDamage", RpcTarget.AllBuffered, name, damageAmount);
    }

    public void ChooseColor() {
        GetComponent<PhotonView>().RPC("AssignColor", RpcTarget.AllBuffered);
    }

    public void PlayGunShot(string tName, int tWeaponNumber) {
        GetComponent<PhotonView>().RPC("PlaySound", RpcTarget.All, tName, tWeaponNumber);
    }

    [PunRPC]
    private void PlaySound(string tName, int tWeaponNumber) {
        for (int i = 0; i < m_namesObject.GetComponent<NicknamesScript>().name.Length; i++) {
            if (tName == m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
                GetComponent<AudioSource>().clip = gunShotSounds[tWeaponNumber];
                GetComponent<AudioSource>().Play();
                return;
            }
        }
    }

    [PunRPC]
    private void GunDamage(string name, float damageAmount) {
        for (int i = 0; i < m_namesObject.GetComponent<NicknamesScript>().names.Length; i++) {
            if (name == m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
                if (m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount > 0.1f) {
                    GetComponent<Animator>().SetBool("Hit", true);
                    m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount -= damageAmount;
                }
                else {
                    m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount = 0;
                    GetComponent<Animator>().SetBool("Dead", true);
                    GetComponent<PlayerMovement>().isDead = true;
                    GetComponent<WeaponChanger>().isDead = true;
                }
            }
        }
    }

    [PunRPC]
    private void AssignColor() {
        for (int i = 0; i < viewId.Length; i++) {
            if (GetComponent<PhotonView>().ViewID == viewId[i]) {
                transform.GetChild(1).GetComponent<Renderer>().material.color = colors[i];
                m_namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(true);
                m_namesObject.GetComponent<NicknamesScript>().names[i].text = GetComponent<PhotonView>().Owner.NickName;
            }
        }
    }

    private void RemoveData() {
        GetComponent<PhotonView>().RPC("RemoveMe", RpcTarget.AllBuffered);
    }

    private void RoomExit() {
        StartCoroutine(GetReadyToLeave());
    }

    [PunRPC]
    private void RemoveMe() {
        for (int i = 0; i < m_namesObject.gameObject.GetComponent<NicknamesScript>().names.Length; i++) {
            if (GetComponent<PhotonView>().Owner.NickName ==
                m_namesObject.GetComponent<NicknamesScript>().names[i].text) {
                m_namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(false);
                m_namesObject.GetComponent<NicknamesScript>().healthBars[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator GetReadyToLeave() {
        yield return new WaitForSeconds(1);
        m_namesObject.GetComponent<NicknamesScript>().leaving();
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator Recover() {
        yield return new WaitForSeconds(0.03f);
        GetComponent<Animator>().SetBool("Hit", false);
    }
}