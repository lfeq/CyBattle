using System.Collections;
using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public bool isDead = false;
    public bool gameOver = false;
    public bool noRespawn;
    
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 100f;

    private Rigidbody m_rb;
    private Animator m_animator;
    private bool m_canJump;
    private Vector3 startPosition;
    private bool respawned = false;
    private GameObject respawnPanel;
    private bool startChecking = false;
    private GameObject canvas;
    
    // Start is called before the first frame update
    void Start() {
        m_rb = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        startPosition = transform.position;
        respawnPanel = GameObject.Find("Respawn Panel");
        canvas = GameObject.Find("Canvas");
    }

    private void Update() {
        if (isDead) {
            return;
        }
        if (isDead && !respawned && !gameOver && !noRespawn) {
            respawned = true;
            respawnPanel.SetActive(true);
            respawnPanel.GetComponent<RespawnTimer>().enabled = true;
            StartCoroutine(respawnWait());
        }
        if (isDead && !respawned && !gameOver && noRespawn) {
            respawned = true;
            GetComponent<DisplayColor>().noRespawnExit();
        }
        if (Input.GetButtonDown("Jump") && m_canJump) {
            m_canJump = false;
            m_rb.AddForce(Vector3.up * (1200 * Time.deltaTime), ForceMode.VelocityChange);
            StartCoroutine(jumpAgain());
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && !startChecking) {
            startChecking = true;
            InvokeRepeating("CheckForWinner", 3, 3);
        }
    }

    private void CheckForWinner() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 0) {
            canvas.GetComponent<KillCount>().noRespawnWinner(GetComponent<PhotonView>().Owner.NickName);
        }
    }

    void FixedUpdate() {
        if (isDead) {
            return;
        }
        respawnPanel.SetActive(false);
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
    
    private IEnumerator jumpAgain() {
        yield return new WaitForSeconds(1);
        m_canJump = true;
    }

    private IEnumerator respawnWait() {
        yield return new WaitForSeconds(3);
        isDead = false;
        respawned = false;
        transform.position = startPosition;
        GetComponent<DisplayColor>().respawn(GetComponent<PhotonView>().Owner.NickName);
    }
}