using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WeaponChanger : MonoBehaviour {
    public bool isDead = false;
    
    [SerializeField] private TwoBoneIKConstraint leftHand;
    [SerializeField] private TwoBoneIKConstraint rightHand;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private Transform[] leftTargets;
    [SerializeField] private Transform[] rightTargets;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private MultiAimConstraint[] aimObjects;
    [SerializeField] private Sprite[] weaponIcons;
    [SerializeField] public int[] ammoAmounts;
    [SerializeField] private GameObject[] muzzleFlash;
    [SerializeField] private float[] damageAmounts;

    private CinemachineVirtualCamera m_camera;
    private GameObject m_cameraGameObject;
    private Transform m_aimTarget;
    private int m_weaponNumber = 0;
    private GameObject m_testForWeapons;
    private PhotonView m_photonView;
    private Image m_weaponIcon;
    private TMP_Text m_ammoText;
    private string shooterName;
    private string gotShotName;
    private GameObject choosePanel;

    private void Start() {
        m_photonView = GetComponent<PhotonView>();
        m_cameraGameObject = GameObject.Find("Player Camera");
        choosePanel = GameObject.Find("ChoosePanel");
        ammoAmounts[0] = 60;
        ammoAmounts[1] = 0;
        ammoAmounts[2] = 0;
        //m_aimTarget = GameObject.Find("Aim Reference").transform;
        if (!gameObject.GetComponent<PhotonView>().IsMine) {
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            return;
        }
        m_camera = m_cameraGameObject.GetComponent<CinemachineVirtualCamera>();
        m_camera.Follow = transform;
        m_camera.LookAt = transform;
        m_testForWeapons = GameObject.Find("Weapon1Pickup(Clone)");
        if (m_testForWeapons != null) {
            return;
        }
        if (gameObject.GetComponent<PhotonView>().Owner.IsMasterClient) {
            GameObject spawner = GameObject.Find("SpawnScript");
            spawner.GetComponent<SpawnCharacters>().spawnWeaponStart();
        }
        m_weaponIcon = GameObject.Find("Weapon UI").GetComponent<Image>();
        m_ammoText = GameObject.Find("AmmoText").GetComponent<TMP_Text>();
        m_ammoText.text = ammoAmounts[0].ToString();
        //Invoke(nameof(setLookAt), 0.1f);
    }


    private void Update() {
        if (!m_photonView.IsMine) {
            return;
        }
        if (isDead) {
            return;
        }
        if (choosePanel.activeInHierarchy) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            ammoAmounts[m_weaponNumber]--;
            m_ammoText.text = ammoAmounts[m_weaponNumber].ToString();
            GetComponent<DisplayColor>().PlayGunShot(GetComponent<PhotonView>().Owner.NickName, m_weaponNumber);
            GetComponent<PhotonView>().RPC("gunMuzzleFlash", RpcTarget.All);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (Physics.Raycast(ray, out hit, 500)) {
                if (hit.transform.gameObject.GetComponent<PhotonView>() != null) {
                    gotShotName = hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName;
                }
                if (hit.transform.gameObject.GetComponent<DisplayColor>() != null) {
                    hit.transform.gameObject.GetComponent<DisplayColor>().DeliverDamage(
                        GetComponent<PhotonView>().Owner.NickName,
                        hit.transform.gameObject.GetComponent<PhotonView>().Owner.NickName,
                        damageAmounts[m_weaponNumber]);
                }
                shooterName = GetComponent<PhotonView>().Owner.NickName;
            }
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        if (Input.GetMouseButtonDown(1)) {
            m_weaponNumber++;
            m_photonView.RPC("change", RpcTarget.AllBuffered);
            if (m_weaponNumber > weapons.Length - 1) {
                m_weaponNumber = 0;
                m_weaponIcon.sprite = weaponIcons[0];
                m_ammoText.text = ammoAmounts[0].ToString();
            }
            foreach (GameObject t in weapons) {
                t.SetActive(false);
            }
            weapons[m_weaponNumber].SetActive(true);
            m_weaponIcon.sprite = weaponIcons[m_weaponNumber];
            m_ammoText.text = ammoAmounts[m_weaponNumber].ToString();
            leftHand.data.target = leftTargets[m_weaponNumber];
            rightHand.data.target = rightTargets[m_weaponNumber];
            rig.Build();
        }
    }

    public void updatePickup() {
        m_ammoText.text = ammoAmounts[m_weaponNumber].ToString();
    }

    [PunRPC]
    private void change() {
        if (m_weaponNumber > weapons.Length - 1) {
            m_weaponNumber = 0;
        }
        foreach (GameObject t in weapons) {
            t.SetActive(false);
        }
        weapons[m_weaponNumber].SetActive(true);
        leftHand.data.target = leftTargets[m_weaponNumber];
        rightHand.data.target = rightTargets[m_weaponNumber];
        rig.Build();
    }

    [PunRPC]
    private void gunMuzzleFlash() {
        muzzleFlash[m_weaponNumber].SetActive(true);
        StartCoroutine(muzzleOff());
    }

    [PunRPC]
    private void muzzleFlashOff() {
        muzzleFlash[m_weaponNumber].SetActive(false);
    }

    private IEnumerator muzzleOff() {
        yield return new WaitForSeconds(0.03f);
        GetComponent<PhotonView>().RPC("muzzleFlashOff", RpcTarget.All);
    }
}