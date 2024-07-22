using System;
using UnityEngine;

public class RotationComponent : MonoBehaviour {
    [SerializeField] private float speed = 20f;

    private void Update() {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}