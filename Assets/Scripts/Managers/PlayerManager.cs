using System;
using System.Collections.Generic;
using System.Collections;
using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private CinemachineVirtualCamera m_vcam;
    private Animator m_animator;
    private PhotonView m_photonView;
    
    private void Start() {
        m_animator = GetComponent<Animator>();
        m_photonView = GetComponent<PhotonView>();
    }

    public void Initialize(CinemachineVirtualCamera t_virtualCamera) {
        m_vcam = t_virtualCamera;
        m_vcam.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
        var thirdPersonFollow = m_vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraSide = 1;
    }

    public void TakeDamage(float t_damage) {
        Debug.Log("Taking damage");
        m_photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All);
    }

    [PunRPC]
    private void TakeDamageRPC() {
        m_animator.SetBool("Hit", true);
        StartCoroutine(PlayAnimation());
        //m_animator.SetBool("Hit", false);
    }
    
    private IEnumerator PlayAnimation() {
        // Wait for the end of the frame to reset the bool
        //m_animator.SetBool("Hit", true);
        yield return new WaitForSeconds(0.03f);
        m_animator.SetBool("Hit", false);
    }
}