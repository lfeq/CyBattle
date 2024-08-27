using UnityEngine;
using StarterAssets;

// TODO: Finish this class to shoot
[RequireComponent(typeof(StarterAssetsInputs))]
public class PlayerWeaponShooter : MonoBehaviour {
    private StarterAssetsInputs m_inputs;

    private void Start() {
        m_inputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update() {
        Shoot();
    }

    private void Shoot() {
        if (!m_inputs.Fire) {
            return;
        }
        Debug.Log("Shooting");
        m_inputs.Fire = false;
    }
}