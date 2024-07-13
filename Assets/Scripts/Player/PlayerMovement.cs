using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 100f;

    private Rigidbody m_rb;
    private Animator m_animator;
    
    // Start is called before the first frame update
    void Start() {
        m_rb = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 rotateY = new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        if (movement != Vector3.zero) {
            m_rb.MoveRotation(m_rb.rotation *  Quaternion.Euler(rotateY));
        }
        m_rb.MovePosition(m_rb.position +
                          transform.forward * (Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime) +
                          transform.right * (Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime));
        m_animator.SetFloat("BlendV", Input.GetAxis("Vertical"));
        m_animator.SetFloat("BlendH", Input.GetAxis("Horizontal"));
    }
}