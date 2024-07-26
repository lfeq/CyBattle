using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameTimeInMinutes;

    private float m_timeRemaining;
    private bool m_timeIsRunning = false;

    private void Start() {
        m_timeRemaining = gameTimeInMinutes * 60;
    }

    private void Update() {
        if (!m_timeIsRunning) {
            return;
        }
        m_timeRemaining -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(m_timeRemaining / 60); 
        float seconds = Mathf.FloorToInt(m_timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void beginTimer() {
        GetComponent<PhotonView>().RPC("count", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void count() {
        m_timeIsRunning = true;
    }
}