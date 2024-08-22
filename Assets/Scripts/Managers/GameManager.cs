using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks {
    [SerializeField] private TMP_InputField playerNickname;
    [SerializeField] private GameObject connectingGameObject;

    private string setName = "";

    private void Start() {
        connectingGameObject.SetActive(false);
    }

    public void updateText() {
        setName = playerNickname.text;
        PhotonNetwork.LocalPlayer.NickName = setName;
    }
    
    public void enterButton() {
        if (setName == "") {
            return;
        }
        //PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        connectingGameObject.SetActive(true);
    }

    public void exitButton() {
        Application.Quit();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("I'm connected to the server!!!");
        SceneManager.LoadScene("Lobby");
    }
}