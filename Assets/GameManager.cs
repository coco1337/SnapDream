using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class GameManager : MonoBehaviourPun
{
    public Text scoreText;
    public PhotonView photonView;
    Dictionary<int, int> scoreList = new Dictionary<int, int>();

    private static GameManager Instance;
    public static GameManager instance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameManager>();
            }
            return Instance;
        }
    }

    public bool IsMasterClient => PhotonNetwork.IsMasterClient && photonView.IsMine;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) Instance = this.GetComponent<GameManager>();

        photonView = this.GetComponent<PhotonView>();
        SetScoreText();
    }

    void SetScoreText()
    {
        string textForScore = "";
        foreach(KeyValuePair<int, int> score in scoreList)
        {
            textForScore +=  GetPlayerNickname(score.Key) + " : " + score.Value + "\n";
        }

        photonView.RPC("PrintScoreText", RpcTarget.AllBuffered, textForScore);
    }

    [PunRPC]
    void PrintScoreText(string text)
    {
        scoreText.text = text;
    }

    string GetPlayerNickname(int playerNumber)
    {
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerNumber)
                return player.NickName;
        }

        return "None Player";
    }

    [PunRPC]
    public void AddPlayer(int playerNum)
    {        
        scoreList.Add(playerNum, 0);
        SetScoreText();
    }

    [PunRPC]
    public void RemovePlayer(int playerNum)
    {
        scoreList.Remove(playerNum);
        SetScoreText();
    }

    [PunRPC]
    public void AddScore(int killPalyerNum)
    {
        scoreList[killPalyerNum] += 1;
        SetScoreText();
    }
}
