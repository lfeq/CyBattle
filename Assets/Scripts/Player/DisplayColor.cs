using System;
using Photon.Pun;
using UnityEngine;

public class DisplayColor : MonoBehaviour {
    public int[] viewId;

    [SerializeField] private int[] buttonNumbers;
    [SerializeField] private Color[] colors;
    
    private GameObject m_namesObject;

    private void Start() {
        m_namesObject = GameObject.Find("Names Background");
    }

    public void chooseColor() {
        GetComponent<PhotonView>().RPC("assignColor", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void assignColor() {
        for (int i = 0; i < viewId.Length; i++) {
            if (GetComponent<PhotonView>().ViewID == viewId[i]) {
                transform.GetChild(1).GetComponent<Renderer>().material.color = colors[i];
                m_namesObject.GetComponent<NicknamesScript>().names[i].gameObject.SetActive(true);
                m_namesObject.GetComponent<NicknamesScript>().names[i].text = GetComponent<PhotonView>().Owner.NickName;
            }
        }
    }
}