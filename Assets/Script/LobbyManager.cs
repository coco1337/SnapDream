using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public Text wellcomText;
    public ListManager listManager;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        listManager.ReloadRoomList(roomList);
    }

    void Start()
    {
        wellcomText.text = PhotonNetwork.NickName + "님 환영합니다.";
        listManager.ReloadRoomPage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Login");
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        PhotonNetwork.CreateRoom(null);
    }


    public void CreateRoom(Text roomName)
    {
        PhotonNetwork.CreateRoom((roomName.text == "") ? null : roomName.text);
    }

    public void CreateRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public void JoinRoom(Text roomName)
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        PhotonNetwork.LoadLevel("Battle");
    }
}
