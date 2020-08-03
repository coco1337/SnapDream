using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour, ISceneManager
{
    private AudioManager audioManager;
    private static LobbyManager instance;
    public static LobbyManager GetInstance() => instance;
    public void SceneInit(int sceneValue) { 

    }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StageStart()
    {
        GameManager.GetInstance().SceneLoad(StageType.Stage, 0);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
