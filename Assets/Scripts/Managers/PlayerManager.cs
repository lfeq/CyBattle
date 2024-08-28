using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private CinemachineVirtualCamera vcam;
    
    public void Initialize(CinemachineVirtualCamera t_virtualCamera) {
        vcam = t_virtualCamera;
        vcam.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
        var thirdPersonFollow = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPersonFollow.CameraSide = 1;
    }
}