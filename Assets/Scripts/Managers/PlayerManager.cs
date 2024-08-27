using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public void Initialize(CinemachineVirtualCamera t_virtualCamera) {
        //t_virtualCamera.Follow = transform;
        t_virtualCamera.Follow = GetComponent<ThirdPersonController>().CinemachineCameraTarget.transform;
    }
}
