using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ListManager : MonoBehaviour
{
    public GameObject roomListPanel;
    const int roomPerPanel = 6;
    public Text lobbyNumText;
    int currentRoomPage;
    int maxPage;

    [SerializeField]
    List<RoomInfo> roomList = new List<RoomInfo>();

    //room
    Button[] buttons;
    // Start is called before the first frame update

    void Start()
    {
        currentRoomPage = 0;
        buttons = roomListPanel.GetComponentsInChildren<Button>();
        lobbyNumText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + " 접속";
    }

    public void ReloadRoomList(List<RoomInfo> rList)
    {
        int updateRoomCount = rList.Count;
        for (int i = 0; i < updateRoomCount; i++)
        {
            //꽉찬방, 숨겨진방, 닫힌방 제외한 방 추가
            if (!rList[i].RemovedFromList)
            {
                if (!roomList.Contains(rList[i]))
                {
                    roomList.Add(rList[i]);
                }
                else
                {
                    roomList[roomList.IndexOf(rList[i])] = rList[i];
                }
            }
            //-1 : hidden 방으로 생성됬거나, 그 hidden방이 닫힐경우 roomList에는 존재하지 않으니 pass
            else if (roomList.IndexOf(rList[i]) != -1)
            {
                roomList.RemoveAt(roomList.IndexOf(rList[i]));
            }
        }

        if (roomList.Count != 0)
            maxPage = roomList.Count / roomPerPanel;
        else
            maxPage = 0;

        if (maxPage < currentRoomPage)
            currentRoomPage = maxPage;

        Debug.Log("current room page : " + currentRoomPage);


        ReloadRoomPage();
    }

    public void ReloadRoomPage()
    {
        Debug.Log("Page Reload!!");
        for (int i = 0; i < roomPerPanel; i++)
        {
            if (roomPerPanel * currentRoomPage + i > roomList.Count - 1)
            {
                buttons[i].GetComponentInChildren<Text>().text = "";
                buttons[i].interactable = false;
            }
            else
            {
                buttons[i].GetComponentInChildren<Text>().text = roomList[roomPerPanel * currentRoomPage + i].Name;
                buttons[i].interactable = true;
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //다른 사람이 들어올 때, 나갈 때 아래 함수 호출하게 한다면 더욱 최적화 가능
        lobbyNumText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + " 접속";
    }


    public void PageNext()
    {
        if (currentRoomPage < maxPage)
        {
            currentRoomPage++;
            ReloadRoomPage();
        }
    }

    public void PagePrev()
    {
        if (currentRoomPage != 0)
        {
            currentRoomPage--;
            ReloadRoomPage();
        }
    }
}
