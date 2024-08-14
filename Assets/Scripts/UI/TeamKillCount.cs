using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamKillCount : MonoBehaviour {
    public List<Kills> highestKills = new List<Kills>();
    public TMP_Text[] killAmounts;
    private GameObject m_killCountPanel;
    private GameObject m_namesGameObject;
    private bool m_isKillCountOn = false;
    public bool countDown = true;
    public GameObject winnerPanel;
    public TMP_Text winnerText;
    private int redTeamKills;
    private int greenTeamKills;

    private void Start() {
        m_killCountPanel = GameObject.Find("Kill Count Panel");
        m_namesGameObject = GameObject.Find("Names Background");
        m_killCountPanel.SetActive(false);
    }

    private void Update() {
        if (!Input.GetKeyDown(KeyCode.K) && countDown) {
            return;
        }
        switch (m_isKillCountOn) {
            case false:
                m_killCountPanel.SetActive(true);
                m_isKillCountOn = true;
                highestKills.Clear();
                for (int i = 0; i < 6; i++) {
                    highestKills.Add(new Kills(m_namesGameObject.GetComponent<NicknamesScript>().names[i].text,
                        m_namesGameObject.GetComponent<NicknamesScript>().kills[i]));
                }
                redTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
                greenTeamKills = highestKills[3].playerKills + highestKills[4].playerKills +
                                 highestKills[5].playerKills;
                killAmounts[0].text = redTeamKills.ToString();
                killAmounts[1].text = greenTeamKills.ToString();
                break;
            case true:
                m_killCountPanel.SetActive(false);
                m_isKillCountOn = false;
                break;
        }
    }
    
    public void timeOver() {
        m_killCountPanel.SetActive(true);
        winnerPanel.SetActive(true);
        m_isKillCountOn = true;
        highestKills.Clear();
        for (int i = 0; i < 6; i++) {
            highestKills.Add(new Kills(m_namesGameObject.GetComponent<NicknamesScript>().names[i].text,
                m_namesGameObject.GetComponent<NicknamesScript>().kills[i]));
        }
        redTeamKills = highestKills[0].playerKills + highestKills[1].playerKills + highestKills[2].playerKills;
        greenTeamKills = highestKills[3].playerKills + highestKills[4].playerKills +
                         highestKills[5].playerKills;
        killAmounts[0].text = redTeamKills.ToString();
        killAmounts[1].text = greenTeamKills.ToString();
        if (redTeamKills > greenTeamKills) {
            winnerText.text = "RED TEAM WINS";
        }
        if (redTeamKills < greenTeamKills) {
            winnerText.text = "Green TEAM WINS";
        }
    }
}