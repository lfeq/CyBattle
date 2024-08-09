using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RespawnTimer : MonoBehaviour {
    public TMP_Text spawnTime;

    private void OnEnable() {
        StartCoroutine(SpawnStarting());
    }

    private IEnumerator SpawnStarting() {
        spawnTime.text = "3";
        yield return new WaitForSeconds(1);
        spawnTime.text = "2";
        yield return new WaitForSeconds(1);
        spawnTime.text = "1";
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}