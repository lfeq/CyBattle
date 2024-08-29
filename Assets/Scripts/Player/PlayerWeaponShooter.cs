using Photon.Pun;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using static UnityEngine.Screen;

[RequireComponent(typeof(StarterAssetsInputs))]
public class PlayerWeaponShooter : MonoBehaviour {
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shootSoundClip;
    [SerializeField] private AudioClip reloadSoundClip;
    [SerializeField] private float weaponDamage = 10f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float fireRate = 0.02f;
    [SerializeField] private Slider reloadSlider;
    [SerializeField] private float reloadTimeInSeconds = 3f;
    
    private StarterAssetsInputs m_inputs;
    private PhotonView m_photonView;
    private int m_currentAmmo;
    private float m_fireRateCounter;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    private void Start() {
        m_inputs = GetComponent<StarterAssetsInputs>();
        m_photonView = GetComponent<PhotonView>();
        m_currentAmmo = maxAmmo;
        reloadSlider.gameObject.SetActive(false);
    }

    private void Update() {
        FireRateCoutDown();
        Reloading();
        Shoot();
        PressedReload();
    }

    private void Shoot() {
        if (!m_inputs.Fire) {
            return;
        }
        if (!FireCooldownComplete()) {
            return;
        }
        if (m_currentAmmo <= 0) {
            StartReloading();
            return;
        }
        m_inputs.Fire = false;
        m_fireRateCounter = fireRate;
        ShootRaycast();
        if (PhotonNetwork.IsConnected && m_photonView != null) {
            // If connected to Photon, call the RPC
            m_photonView.RPC(nameof(PlayEffects), RpcTarget.All);
        } else {
            // If not connected, call the method locally
            PlayEffects();
        }
        m_currentAmmo--;
    }

    private void Reloading() {
        if (!isReloading) {
            return;
        }
        reloadTimer -= Time.deltaTime;
        reloadSlider.value = reloadTimer;
        ActiveReload();
        if (!(reloadTimer <= 0)) {
            return;
        }
        FinishReloading();
    }

    private void ActiveReload() {
        if (!m_inputs.Reload) {
            return;
        }
        m_inputs.Reload = false;
        float sliderValue = reloadSlider.value;
        if (sliderValue is <= 1.4f and >= 1f) {
            FinishReloading();
        }
    }

    private void FinishReloading() {
        m_currentAmmo = maxAmmo;
        reloadSlider.gameObject.SetActive(false);
        isReloading = false;
        reloadTimer = -1;
        AudioSource.PlayClipAtPoint(reloadSoundClip, transform.position); // Play audio clip
    }
    
    private void PressedReload() {
        if (!m_inputs.Reload || isReloading) {
            return;
        }
        StartReloading();
        m_inputs.Reload = false;
    }

    private void StartReloading() {
        if (isReloading) {
            return;
        }
        isReloading = true;
        reloadSlider.maxValue = reloadTimeInSeconds;
        reloadSlider.value = reloadTimeInSeconds;
        reloadSlider.gameObject.SetActive(true);
        reloadTimer = reloadTimeInSeconds;
    }

    private void FireRateCoutDown() {
        if (FireCooldownComplete()) {
            return;
        }
        m_fireRateCounter -= Time.deltaTime;
    }

    private bool FireCooldownComplete() {
        return m_fireRateCounter <= 0;
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