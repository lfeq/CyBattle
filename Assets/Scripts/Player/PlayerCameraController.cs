using System;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cinemachineCameraTarget;
    
    [Tooltip("How far in degrees can you move the camera up")]
    public float topClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float bottomClamp = -30.0f;
    
    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    public float cameraAngleOverride = 0.0f;
    
    private Vector2 m_lookInput;
    private const float Threshold = 0.01f;
    private float m_cinemachineTargetYaw;
    private float m_cinemachineTargetPitch;

    private void Start() {
        m_cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update() {
        UpdateLookInput();
    }

    private void LateUpdate() {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input
        if (m_lookInput.sqrMagnitude >= Threshold)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f; // For old input system, use a constant multiplier

            m_cinemachineTargetYaw += m_lookInput.x * deltaTimeMultiplier;
            m_cinemachineTargetPitch += m_lookInput.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        m_cinemachineTargetYaw = ClampAngle(m_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        m_cinemachineTargetPitch = ClampAngle(m_cinemachineTargetPitch, bottomClamp, topClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(m_cinemachineTargetPitch + cameraAngleOverride,
            m_cinemachineTargetYaw, 0.0f);
    }
    
    private void UpdateLookInput() {
        m_lookInput.x = Input.GetAxis("Mouse X");
        m_lookInput.y = Input.GetAxis("Mouse Y");
    }
    
    private static float ClampAngle(float t_lfAngle, float t_lfMin, float t_lfMax)
    {
        if (t_lfAngle < -360f) t_lfAngle += 360f;
        if (t_lfAngle > 360f) t_lfAngle -= 360f;
        return Mathf.Clamp(t_lfAngle, t_lfMin, t_lfMax);
    }
}