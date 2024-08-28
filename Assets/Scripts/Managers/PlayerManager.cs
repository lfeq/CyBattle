using System.Collections;
using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public bool IsDead { get; private set; }
    
    [SerializeField] private float maxHealth = 100f;

    private float m_currentHealth;
    private CinemachineVirtualCamera m_vcam;
    private Animator m_animator;
    private PhotonView m_photonView;

    private void Start() {
        m_animator = GetComponent<Animator>();
        m_photonView = GetComponent<PhotonView>();
        ResetHealth();
        IsDead = false;
    }

    public void Initialize(CinemachineVirtualCamera t_virtualCamera) {
        m_vcam = t_virtualCamera;
        m_vcam.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
        var thirdPersonFollow = m_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraSide = 1;
    }
    
    public void TakeDamage(float t_damage) {
        if (PhotonNetwork.IsConnected && m_photonView is not null) {
            m_photonView.RPC(nameof(PlayDamageAnimationRPC), RpcTarget.All); // Play damage animation
            m_photonView.RPC(nameof(ReduceHealthRPC), RpcTarget.AllBuffered, t_damage); // Reduce health
        }
        else {
            PlayDamageAnimationRPC();
            ReduceHealthRPC(t_damage);                  
        }
    }
    
    [PunRPC]
    public void ResetHealth() {
        m_currentHealth = maxHealth;
    }

    [PunRPC]
    private void PlayDamageAnimationRPC() {
        m_animator.SetBool("Hit", true);
        StartCoroutine(StopHurtAnimation());
    }

    [PunRPC]
    private void ReduceHealthRPC(float t_damage) {
        m_currentHealth -= t_damage;
        if (!(m_currentHealth <= 0)) {
            return;
        }
        if (PhotonNetwork.IsConnected && m_photonView is not null) {
            m_photonView.RPC(nameof(KillPlayerRPC), RpcTarget.All); // Kill player
        }
        else {
            KillPlayerRPC();             
        }
    }
    
    //TODO: kill player
    [PunRPC]
    private void KillPlayerRPC() {
        IsDead = true;
        m_animator.SetBool("Dead", IsDead);
    }

    private IEnumerator StopHurtAnimation() {
        yield return new WaitForSeconds(0.03f);
        m_animator.SetBool("Hit", false);
    }
}