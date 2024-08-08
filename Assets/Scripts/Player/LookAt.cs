using Photon.Pun;
using TMPro;
using UnityEngine;

public class LookAt : MonoBehaviour {
    [SerializeField] private GameObject crosshair;
    [SerializeField] private TMP_Text nicknameText;
    
    private Vector3 m_worldPosition;
    private Vector3 m_screenPosition;
    private Camera m_camera;

    private void Start() {
        m_camera = Camera.main;
        nicknameText.text = PhotonNetwork.LocalPlayer.NickName;
    }

    private void FixedUpdate() {
        m_screenPosition = Input.mousePosition;
        m_screenPosition.z = 6f;
        m_worldPosition = m_camera.ScreenToWorldPoint(m_screenPosition);
        transform.position = m_worldPosition;
        crosshair.transform.position = Input.mousePosition;
    }
}
