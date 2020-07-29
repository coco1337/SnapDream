using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum StageType { 
    Lobby = 0, StageSelect = 1, Stage = 2, Endding = 3
}


public sealed class GameManager : MonoBehaviour
{
    static GameManager instance;

    //0 : Lobby Scene
    //1 : Stage Select Scene
    //2 : Stage Scene
    //3 : Endding Scene
    [SerializeField] string[] SceneList;
    [SerializeField] int sceneValue;
    ISceneManager sceneManager;

    static public GameManager GetInstance() => instance;
    public void ExitGame() => Application.Quit();

    public int GetCurrentCutNum() => cutManager.GetCurrentCutNum();

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        instance = FindObjectOfType<GameManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        sceneValue = 0;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();
        sceneManager.SceneInit(sceneValue);
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneLoad(StageType stageType, int sceneChangeValue = 0)
    {
        sceneValue = sceneChangeValue;
        SceneManager.LoadScene(SceneList[(int)stageType]);
    }


    public void SceneRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
