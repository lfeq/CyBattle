using System.Collections;
using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private float maxHealth = 100f;
    
    private float currentHealth;
    private CinemachineVirtualCamera m_vcam;
    private Animator m_animator;
    private PhotonView m_photonView;
    
    private void Start() {
        m_animator = GetComponent<Animator>();
        m_photonView = GetComponent<PhotonView>();
        ResetHealth();
    }

    public void Initialize(CinemachineVirtualCamera t_virtualCamera) {
        m_vcam = t_virtualCamera;
        m_vcam.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
        var thirdPersonFollow = m_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraSide = 1;
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float t_damage) {
        m_photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All);
    }

    [PunRPC]
    private void TakeDamageRPC() {
        m_animator.SetBool("Hit", true);
        StartCoroutine(PlayAnimation());
    }
    
    private IEnumerator PlayAnimation() {
        yield return new WaitForSeconds(0.03f);
        m_animator.SetBool("Hit", false);
    }
}