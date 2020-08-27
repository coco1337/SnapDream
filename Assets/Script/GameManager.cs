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
    public bool isGameManagerActioning;
    public bool isExeMenuActioning;

    private ISceneManager sceneManager;
    private static GameManager instance;
    //SceneList
    //0 : Lobby Scene
    //1 : Stage Select Scene
    //2 : Stage Scene
    //3 : Endding Scene
    [SerializeField] private string[] sceneList;
    [SerializeField] private string[] stageSceneList;
    [SerializeField] private int sceneValue;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private float sceneChangeTime;
    [SerializeField] private float sceneRestartTime;


    static public GameManager GetInstance() => instance;
    public AudioManager GetAudioManager => audioManager;
    public void ExitGame() => Application.Quit();

    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        sceneValue = 0;
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();
        //Debug.Log(sceneManager.ToString());
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        isGameManagerActioning = false;
        isExeMenuActioning = false;
        uiManager.Init();
        uiManager.FadeOut(sceneChangeTime);
        //Debug.Log(SceneManager.GetActiveScene().name + " Start");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ISceneManager>();
        isGameManagerActioning = false;
        isExeMenuActioning = false;
        uiManager.FadeOut(sceneChangeTime);
        sceneManager.SceneInit(sceneValue);
        Debug.Log(SceneManager.GetActiveScene().name + " Start");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneLoad(StageType stageType, int sceneChangeValue = 0)
    {
        if (!isGameManagerActioning && !uiManager.isFading)
        {
            isGameManagerActioning = true;
            sceneValue = sceneChangeValue;
            if (stageType == StageType.Stage)
                StartCoroutine(SceneChangeAction(stageSceneList[0], sceneChangeTime));
                //StartCoroutine(SceneChangeAction(stageSceneList[sceneValue], sceneChangeTime));
            else
                StartCoroutine(SceneChangeAction(sceneList[(int)stageType], sceneChangeTime));
        }
    }

    public void SceneRestart()
    {
        if (!isGameManagerActioning && !uiManager.isFading)
        {
            StartCoroutine(SceneChangeAction(SceneManager.GetActiveScene().name, sceneRestartTime));
        }
    }

    IEnumerator SceneChangeAction(string sceneName, float changeTime)
    {
        if (!uiManager.isFading)
        {
            isGameManagerActioning = true;
            uiManager.FadeIn(changeTime);
            while (uiManager.isFading)
                yield return null;
            isGameManagerActioning = false;
            SceneManager.LoadScene(sceneName);
        }
    }
}