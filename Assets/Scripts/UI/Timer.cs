using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {
    public GameObject canvas;
    public bool timeStop = false;
    
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float gameTimeInMinutes;

    private float m_timeRemaining;
    private bool m_timeIsRunning = false;

    private void Start() {
        m_timeRemaining = gameTimeInMinutes * 60;
    }

    private void Update() {
        if (!GetComponent<NicknamesScript>().noRespawn) {
            timerText.text = "";
            return;
        }
        if (!m_timeIsRunning) {
            return;
        }
        m_timeRemaining -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(m_timeRemaining / 60); 
        float seconds = Mathf.FloorToInt(m_timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
        if (m_timeRemaining <= 0) {
            if (this.gameObject.GetComponent<NicknamesScript>().teamMode == false) {
                canvas.GetComponent<KillCount>().countDown = false;
                canvas.GetComponent<KillCount>().timeOver();
                timeStop = true;
            }
            if (this.gameObject.GetComponent<NicknamesScript>().teamMode == true) {
                canvas.GetComponent<TeamKillCount>().countDown = false;
                canvas.GetComponent<TeamKillCount>().timeOver();
                timeStop = true;
            }
        }
    }

    public void beginTimer() {
        GetComponent<PhotonView>().RPC("count", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void count() {
        m_timeIsRunning = true;
    }
}
