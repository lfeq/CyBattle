using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DisplayColor : MonoBehaviourPunCallbacks {
    public int[] viewId;
    public Color[] teamColors;

    [FormerlySerializedAs("m_gunShotSounds")]
    public AudioClip[] gunShotSounds;

    [SerializeField] private int[] buttonNumbers;
    [SerializeField] private Color[] colors;

    private GameObject m_namesObject;
    private GameObject m_waitFoPlayers;
    private bool teamMode = false;
    private bool isRespawn = false;

    // TODO: Refactor this shit. this class should only handle color selection
    private void Start() {
        // m_namesObject = GameObject.Find("Names Background");
        // m_waitFoPlayers = GameObject.Find("Waiting Background");
        // InvokeRepeating("CheckTime", 1, 1);
        // teamMode = m_namesObject.GetComponent<NicknamesScript>().teamMode;
        // isRespawn = m_namesObject.GetComponent<NicknamesScript>().noRespawn;
        // GetComponent<PlayerMovement>().noRespawn = isRespawn;
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

    public void noRespawnExit() {
        StartCoroutine(waitToExit());
    }

    public void DeliverDamage(string shooterName, string name, float damageAmount) {
        GetComponent<PhotonView>().RPC("GunDamage", RpcTarget.AllBuffered, shooterName, name, damageAmount);
    }

    public void ChooseColor() {
        GetComponent<PhotonView>().RPC("AssignColor", RpcTarget.AllBuffered);
    }

    public void PlayGunShot(string tName, int tWeaponNumber) {
        GetComponent<PhotonView>().RPC("PlaySound", RpcTarget.All, tName, tWeaponNumber);
    }

    [PunRPC]
    private void PlaySound(string tName, int tWeaponNumber) {
        for (int i = 0; i < m_namesObject.GetComponent<HealthBarsManager>().name.Length; i++) {
            if (tName == m_namesObject.GetComponent<HealthBarsManager>().names[i].text) {
                GetComponent<AudioSource>().clip = gunShotSounds[tWeaponNumber];
                GetComponent<AudioSource>().Play();
                return;
            }
        }
    }

    [PunRPC]
    private void GunDamage(string shooterName, string name, float damageAmount) {
        for (int i = 0; i < m_namesObject.GetComponent<HealthBarsManager>().names.Length; i++) {
            if (name == m_namesObject.GetComponent<HealthBarsManager>().names[i].text) {
                if (m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount > 0.1f) {
                    GetComponent<Animator>().SetBool("Hit", true);
                    m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount -= damageAmount;
                }
                else {
                    m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                        .fillAmount = 0;
                    GetComponent<Animator>().SetBool("Dead", true);
                    GetComponent<PlayerMovement>().isDead = true;
                    GetComponent<WeaponChanger>().isDead = true;
                    GetComponentInChildren<AimLookAtRef>().isDead = true;
                    m_namesObject.GetComponent<HealthBarsManager>().RunMessage(shooterName, name);
                    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }
        }
    }

    [PunRPC]
    private void AssignColor() {
        for (int i = 0; i < viewId.Length; i++) {
            if (!teamMode) {
                if (GetComponent<PhotonView>().ViewID == viewId[i]) {
                    transform.GetChild(1).GetComponent<Renderer>().material.color = colors[i];
                    m_namesObject.GetComponent<HealthBarsManager>().names[i].gameObject.SetActive(true);
                    m_namesObject.GetComponent<HealthBarsManager>().names[i].text = GetComponent<PhotonView>().Owner.NickName;
                }
            } else if (teamMode) {
                if (GetComponent<PhotonView>().ViewID == viewId[i]) {
                    transform.GetChild(1).GetComponent<Renderer>().material.color = teamColors[i];
                    m_namesObject.GetComponent<HealthBarsManager>().names[i].gameObject.SetActive(true);
                    m_namesObject.GetComponent<HealthBarsManager>().names[i].text = GetComponent<PhotonView>().Owner.NickName;
                }
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
        for (int i = 0; i < m_namesObject.gameObject.GetComponent<HealthBarsManager>().names.Length; i++) {
            if (GetComponent<PhotonView>().Owner.NickName ==
                m_namesObject.GetComponent<HealthBarsManager>().names[i].text) {
                m_namesObject.GetComponent<HealthBarsManager>().names[i].gameObject.SetActive(false);
                m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator GetReadyToLeave() {
        yield return new WaitForSeconds(1);
        m_namesObject.GetComponent<HealthBarsManager>().Leaving();
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator Recover() {
        yield return new WaitForSeconds(0.03f);
        GetComponent<Animator>().SetBool("Hit", false);
    }

    public void respawn(string ownerNickName) {
        GetComponent<PhotonView>().RPC("ResetForReplay", RpcTarget.AllBuffered, name);
    }

    [PunRPC]
    private void ResetForReplay(string name) {
        for (int i = 0; i < m_namesObject.GetComponent<HealthBarsManager>().names.Length; i++) {
            if (name == m_namesObject.GetComponent<HealthBarsManager>().names[i].text) {
                m_namesObject.GetComponent<HealthBarsManager>().healthBars[i].gameObject.GetComponent<Image>()
                    .fillAmount = 1;
                GetComponent<Animator>().SetBool("Dead", false);
                GetComponent<PlayerMovement>().isDead = false;
                GetComponent<WeaponChanger>().isDead = false;
                GetComponentInChildren<AimLookAtRef>().isDead = false;
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }
    
    private void CheckTime() {
        if (m_namesObject.GetComponent<Timer>().timeStop) {
            print("Check Time");
            GetComponent<PlayerMovement>().isDead = true;
            GetComponent<PlayerMovement>().gameOver = true;
            GetComponent<WeaponChanger>().isDead = true;
            GetComponentInChildren<AimLookAtRef>().isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    private IEnumerator waitToExit() {
        yield return new WaitForSeconds(3);
        RemoveMe();
        RoomExit();
    }
}