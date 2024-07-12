using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float movementSpeed = 3.5f;

    private Rigidbody m_rb;
    
    // Start is called before the first frame update
    void Start() {
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        m_rb.MovePosition(m_rb.position +
                          transform.forward * (Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime) +
                          transform.right * (Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime));
    }
}