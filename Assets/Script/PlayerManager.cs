using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private PlayerMoveController playerMoveController;
    
    public void PlayerManagerInit()
    {
        playerList = FindObjectOfType<CutManager>().GetPlayerList;
        playerMoveController = transform.GetComponent<PlayerMoveController>();
        while (playerList[0] == null) { }
        Debug.Log(playerList[0]);
        Debug.Log(playerMoveController);
        playerMoveController.SetPlayer(playerList[0]);
    }

    public void MoveToNextCut()
    {
        playerMoveController.SetPlayer(playerList[GameManager.GetInstance().GetCurrentCutNum()]);
    }
}
