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
    [SerializeField] private string[] sceneList;
    [SerializeField] private string[] stageSceneList;
    [SerializeField] private int sceneValue;
    [SerializeField] private AudioManager audioManager;
    private ISceneManager sceneManager;

    static public GameManager GetInstance() => instance;
    public AudioManager GetAudioManager => audioManager;
    public void ExitGame() => Application.Quit();

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        sceneValue = 0;
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();
        Debug.Log(sceneManager.ToString());
        DontDestroyOnLoad(gameObject);
    }

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
        sceneManager.SceneInit(sceneValue);
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{
    //    sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();
    //    Debug.Log(sceneManager.ToString());
    //    sceneManager.SceneInit(sceneValue);
    //    throw new System.NotImplementedException();
    //}

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneLoad(StageType stageType, int sceneChangeValue = 0)
    {
        sceneValue = sceneChangeValue;
        if (stageType == StageType.Stage)
            SceneManager.LoadScene(stageSceneList[sceneValue]);
        else
            SceneManager.LoadScene(sceneList[(int)stageType]);
    }

    public void SceneRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    ////For Error Collect
    //public CutManager GetCutManager => StageManager.GetInstance().GetCutManager;
    //public AudioManager GetAudioManager => StageManager.GetInstance().GetAudioManager;
    //public int GetCurrentCutNum() { return 0; }
}