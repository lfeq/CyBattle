using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class KillCount : MonoBehaviour {
    public List<Kills> highestKillsList = new List<Kills>();
    public TMP_Text[] textNames;
    public TMP_Text[] textKills;

    private GameObject m_killCountPanel;
    private GameObject m_namesGameObject;
    private bool m_isKillCountOn = false;

    private void Start() {
        m_killCountPanel = GameObject.Find("Kill Count Panel");
        m_namesGameObject = GameObject.Find("Names Background");
        m_killCountPanel.SetActive(false);
    }

    private void Update() {
        if (!Input.GetKeyDown(KeyCode.K)) {
            return;
        }
        switch (m_isKillCountOn) {
            case false:
                m_killCountPanel.SetActive(true);
                m_isKillCountOn = true;
                highestKillsList.Clear();
                for (int i = 0; i < textNames.Length; i++) {
                    highestKillsList.Add(new Kills(m_namesGameObject.GetComponent<NicknamesScript>().names[i].text,
                        m_namesGameObject.GetComponent<NicknamesScript>().kills[i]));
                }
                highestKillsList.Sort();
                for (int i = 0; i < textNames.Length; i++) {
                    textNames[i].text = highestKillsList[i].playerName;
                    textKills[i].text = highestKillsList[i].playerKills.ToString();
                }
                for (int i = 0; i < textNames.Length; i++) {
                    if (textNames[i].text != "Name") {
                        continue;
                    }
                    textNames[i].text = "";
                    textKills[i].text = "";
                }
                break;
            case true:
                m_killCountPanel.SetActive(false);
                m_isKillCountOn = false;
                break;
        }
    }
}