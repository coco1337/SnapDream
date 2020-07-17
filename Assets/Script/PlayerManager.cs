using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private InputController inputController;
    
    public void PlayerManagerInit()
    {
        playerList = FindObjectOfType<CutManager>().GetPlayerList;
        inputController = transform.GetComponent<InputController>();
        while (playerList[0] == null) { }
        inputController.SetPlayer(playerList[0]);
    }

    public void MoveToNextCut()
    {
        inputController.SetPlayer(playerList[GameManager.GetInstance().GetCurrentCutNum()]);
    }
}
