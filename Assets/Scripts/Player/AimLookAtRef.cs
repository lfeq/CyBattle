using Photon.Pun;
using UnityEngine;

public class AimLookAtRef : MonoBehaviour {
    private GameObject m_lookAtObject;
    private PhotonView m_photonView;

    private void Start() {
        m_lookAtObject = GameObject.Find("Aim Reference");
        m_photonView = gameObject.GetComponentInParent<PhotonView>();
    }

    private void FixedUpdate() {
        if (m_photonView.IsMine) {
            transform.position = m_lookAtObject.transform.position;
        }
    }
}