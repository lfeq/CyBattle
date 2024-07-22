using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using Photon.Pun;

public class WeaponChanger : MonoBehaviour {
    [SerializeField] private TwoBoneIKConstraint leftHand;
    [SerializeField] private TwoBoneIKConstraint rightHand;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private Transform[] leftTargets;
    [SerializeField] private Transform[] rightTargets;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private MultiAimConstraint[] aimObjects;

    private CinemachineVirtualCamera m_camera;
    private GameObject m_cameraGameObject;
    private Transform m_aimTarget;
    private int m_weaponNumber = 0;
    private GameObject m_testForWeapons;

    private void Start() {
        m_cameraGameObject = GameObject.Find("Player Camera");
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
        GameObject spawner = GameObject.Find("SpawnScript");
        spawner.GetComponent<SpawnCharacters>().spawnWeaponStart();
        //Invoke(nameof(setLookAt), 0.1f);
    }


    private void Update() {
        if (!Input.GetMouseButtonDown(1)) {
            return;
        }
        m_weaponNumber++;
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

    // private void setLookAt() {
    //     if (m_aimTarget == null) {
    //         return;
    //     }
    //     foreach (MultiAimConstraint t in aimObjects) {
    //         WeightedTransformArray target = t.data.sourceObjects;
    //         target.SetTransform(0, m_aimTarget.transform);
    //         t.data.sourceObjects = target;
    //     }
    //     rig.Build();
    // }
}