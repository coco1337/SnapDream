using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private PlayerInput inputController;
    
    public void PlayerManagerInit()
    {
        playerList = FindObjectOfType<CutManager>().GetPlayerList;
        inputController = transform.GetComponent<PlayerInput>();
        while (playerList[0] == null) { }
        foreach(Player player in playerList)
        {
            player.PlayerInit();
        }
        inputController.SetPlayer(playerList[0]);
    }

    public void MoveToNextCut()
    {
        inputController.SetPlayer(playerList[StageManager.GetInstance().GetCurrentCutNum()]);
    }

    public void StageClear()
    {
        foreach (var player in playerList)
        {
            player.StageClear();
        }

    }
}
