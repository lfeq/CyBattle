using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamesScript : MonoBehaviour {
    public TMP_Text[] names;
    
    [SerializeField] private Image[] healthBars;

    private void Start() {
        foreach (TMP_Text nameText in names) {
            nameText.gameObject.SetActive(false);
        }
    }
}