using System;
using System.Collections;
using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public bool IsDead { get; private set; }

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float respawnTimeInSeconds = 4f;

    private float m_currentHealth;
    private CinemachineVirtualCamera m_vcam;
    private Animator m_animator;
    private PhotonView m_photonView;
    private HealthBarsManager m_healthBarsManager;
    private int m_healthBarIndex;
    private float resspawnTimer;

    private void Start() {
        m_healthBarsManager = FindObjectOfType<HealthBarsManager>();
        m_animator = GetComponent<Animator>();
        m_photonView = GetComponent<PhotonView>();
        ResetHealth();
        IsDead = false;
    }

    private void Update() {
        Respawn();
    }

    public void Initialize(CinemachineVirtualCamera t_virtualCamera, HealthBarsManager t_healthBarsManager) {
        m_vcam = t_virtualCamera;
        m_vcam.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
        var thirdPersonFollow = m_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraSide = 1;
        m_healthBarsManager = FindObjectOfType<HealthBarsManager>();
        m_healthBarIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        m_healthBarsManager.EnableHealthBar(m_healthBarIndex,
            PhotonNetwork.IsConnected ? GetComponent<PhotonView>().Owner.NickName : "TEST PLAYER");
    }

    public void TakeDamage(float t_damage) {
        if (PhotonNetwork.IsConnected && m_photonView is not null) {
            m_photonView.RPC(nameof(PlayDamageAnimationRPC), RpcTarget.All); // Play damage animation
            m_photonView.RPC(nameof(ReduceHealthRPC), RpcTarget.All, t_damage); // Reduce health
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
    
    private void Respawn() {
        if (!IsDead) {
            return;
        }
        resspawnTimer -= Time.deltaTime;
        if (resspawnTimer > 0) {
            return;
        }

        // Ensure only the owning player triggers the respawn
        if (m_photonView.IsMine) {
            m_photonView.RPC(nameof(RespawnRPC), RpcTarget.AllBuffered, LevelManager.s_instance.GetRandomSpawnPoint());
        }
    }

    [PunRPC]
    private void RespawnRPC(Vector3 newPosition) {
        IsDead = false;
        m_animator.SetBool("Dead", IsDead);
        transform.position = newPosition;
        ResetHealth();
        m_healthBarsManager.UpdateHealthBar(m_healthBarIndex, m_currentHealth);
        Debug.LogError($"Respawning player at: {newPosition}");
    }

    [PunRPC]
    private void PlayDamageAnimationRPC() {
        m_animator.SetBool("Hit", true);
        StartCoroutine(StopHurtAnimation());
        m_healthBarsManager.UpdateHealthBar(m_healthBarIndex, m_currentHealth);
    }

    [PunRPC]
    private void ReduceHealthRPC(float t_damage) {
        // Check if this instance is owned by the local player
        if (!m_photonView.IsMine) return;

        m_currentHealth -= t_damage;

        // Update health bar for the local player only
        if (m_healthBarsManager != null) {
            m_healthBarsManager.UpdateHealthBar(m_healthBarIndex, m_currentHealth);
        }

        if (m_currentHealth <= 0) {
            KillPlayerRPC();
        }
    }

    [PunRPC]
    private void KillPlayerRPC() {
        IsDead = true;
        resspawnTimer = respawnTimeInSeconds;
        m_animator.SetBool("Dead", IsDead);
    }

    private IEnumerator StopHurtAnimation() {
        yield return new WaitForSeconds(0.03f);
        m_animator.SetBool("Hit", false);
    }
}