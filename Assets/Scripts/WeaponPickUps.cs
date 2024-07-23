using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class WeaponPickUps : MonoBehaviour {
    [FormerlySerializedAs("m_respawnTime")] [SerializeField]
    private float respawnTime;

    [SerializeField] private int weaponType = 1;

    private AudioSource m_audioPlayer;

    private void Start() {
        m_audioPlayer = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider t_other) {
        if (t_other.CompareTag("Player")) {
            gameObject.GetComponent<PhotonView>().RPC("playPickupAudio", RpcTarget.All);
            gameObject.GetComponent<PhotonView>().RPC("turnOff", RpcTarget.All);
        }
    }

    [PunRPC]
    private void playPickupAudio() {
        m_audioPlayer.Play();
    }

    [PunRPC]
    private void turnOff() {
        if (weaponType == 1) {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
        else {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(waitToRespawn());
    }

    [PunRPC]
    private void turnOn() {
        if (weaponType == 1) {
            gameObject.GetComponent<Renderer>().enabled = true;
        }
        else {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        gameObject.GetComponent<Collider>().enabled = true;
    }

    private IEnumerator waitToRespawn() {
        yield return new WaitForSeconds(respawnTime);
        gameObject.GetComponent<PhotonView>().RPC("turnOn", RpcTarget.All);
    }
}