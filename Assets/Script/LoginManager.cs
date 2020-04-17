using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public InputField NicNameInput;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NicNameInput.text;
        PhotonNetwork.JoinLobby();
        PhotonNetwork.LoadLevel("Lobby");
    }
}
