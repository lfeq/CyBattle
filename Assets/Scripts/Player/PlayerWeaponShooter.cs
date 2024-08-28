using Photon.Pun;
using UnityEngine;
using StarterAssets;
using static UnityEngine.Screen;

[RequireComponent(typeof(StarterAssetsInputs))]
public class PlayerWeaponShooter : MonoBehaviour {
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSoundClip;
    [SerializeField] private float weaponDamage = 10f;
    
    private StarterAssetsInputs m_inputs;
    private PhotonView m_photonView;

    private void Start() {
        m_inputs = GetComponent<StarterAssetsInputs>();
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update() {
        Shoot();
    }

    private void Shoot() {
        if (!m_inputs.Fire) {
            return;
        }
        ShootRaycast();
        if (PhotonNetwork.IsConnected && m_photonView != null) {
            // If connected to Photon, call the RPC
            m_photonView.RPC(nameof(PlayEffects), RpcTarget.All);
        } else {
            // If not connected, call the method locally
            PlayEffects();
        }
        m_inputs.Fire = false;
    }

    private void ShootRaycast() {
        Vector2 screenCenterPoint = new Vector2(width / 2, height / 2); // Get the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        const float rayDistance = 500f;
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance)) {
            if (!hit.transform.CompareTag("Player")) {
                return;
            }
            hit.transform.GetComponent<PlayerManager>().TakeDamage(weaponDamage);
        }
    }

    [PunRPC]
    private void PlayEffects() {
        muzzleFlash.Play(); // Play the particle system
        AudioSource.PlayClipAtPoint(shootSoundClip, transform.position); // Play audio clip
    }
}