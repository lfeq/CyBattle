using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponChanger : MonoBehaviour {
    [SerializeField] private TwoBoneIKConstraint leftHand;
    [SerializeField] private TwoBoneIKConstraint rightHand;
    [SerializeField] private RigBuilder rig;
    [SerializeField] private Transform[] leftTargets;
    [SerializeField] private Transform[] rightTargets;
    [SerializeField] private GameObject[] weapons;

    private int m_weaponNumber = 0;
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
}