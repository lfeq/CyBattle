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
    private Vector3 m_startPosition;
    private bool m_respawned = false;
    private GameObject m_respawnPanel;
    private bool m_startChecking = false;
    private GameObject m_canvas;
    private static readonly int BlendV = Animator.StringToHash("BlendV");
    private static readonly int BlendH = Animator.StringToHash("BlendH");
    
    // Start is called before the first frame update
    void Start() {
        m_rb = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_startPosition = transform.position;
        m_respawnPanel = GameObject.Find("Respawn Panel");
        m_canvas = GameObject.Find("Canvas");
        m_canJump = true;
    }

    // TODO: Move to helper methods
    private void Update() {
        if (isDead) {
            return;
        }
        if (isDead && !m_respawned && !gameOver && !noRespawn) {
            m_respawned = true;
            m_respawnPanel.SetActive(true);
            m_respawnPanel.GetComponent<RespawnTimer>().enabled = true;
            StartCoroutine(respawnWait());
        }
        if (isDead && !m_respawned && !gameOver && noRespawn) {
            m_respawned = true;
            GetComponent<DisplayColor>().noRespawnExit();
        }
        if (Input.GetButtonDown("Jump") && m_canJump) {
            m_canJump = false;
            m_rb.AddForce(Vector3.up * (1200 * Time.deltaTime), ForceMode.VelocityChange);
            StartCoroutine(jumpAgain());
        }
        // TODO: This should be in the level manager
        // if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && !startChecking) {
        //     startChecking = true;
        //     InvokeRepeating("CheckForWinner", 3, 3);
        // }
    }
    
    // TODO: Move to helper methods
    void FixedUpdate() {
        if (isDead) {
            return;
        }
        //respawnPanel.SetActive(false);
        Vector3 rotateY = new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        // TODO: Change rotation to its own script
        //m_rb.MoveRotation(m_rb.rotation *  Quaternion.Euler(rotateY));
        m_rb.MovePosition(m_rb.position +
                          transform.forward * (Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime) +
                          transform.right * (Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime));
        m_animator.SetFloat(BlendV, Input.GetAxis("Vertical"));
        m_animator.SetFloat(BlendH, Input.GetAxis("Horizontal"));
    }
    
    private IEnumerator jumpAgain() {
        yield return new WaitForSeconds(1);
        m_canJump = true;
    }
    
    private void CheckForWinner() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 0) {
            m_canvas.GetComponent<KillCount>().noRespawnWinner(GetComponent<PhotonView>().Owner.NickName);
        }
    }

    private IEnumerator respawnWait() {
        yield return new WaitForSeconds(3);
        isDead = false;
        m_respawned = false;
        transform.position = m_startPosition;
        GetComponent<DisplayColor>().respawn(GetComponent<PhotonView>().Owner.NickName);
    }
}