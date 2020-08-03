using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour, ISceneManager
{
    //Manager List
    private static StageManager instance;
    private CutManager cutManager;
    private AudioManager audioManager;
    private PlayerManager playerManager;
    private StageUIManager uiManager;

    private int stageValue;

    [SerializeField] GameObject[] StageBackgroundList;
    [SerializeField] private float sceneChangeTime = 3f;
    [SerializeField] private float sceneReStartTime = 1f;


    [SerializeField] bool isOption = false;
    static public StageManager GetInstance() => instance;
    public AudioManager GetAudioManager => audioManager;
    public CutManager GetCutManager => cutManager;
    public bool isOptioning => isOption;
    public int GetCurrentCutNum() => cutManager.GetCurrentCutNum();

    public void SceneInit(int sceneValue) {
        if (sceneValue > StageBackgroundList.Length)
            Debug.LogError("Out of bound Stage List");

        isOption = false;
        stageValue = sceneValue;
        if ((cutManager = FindObjectOfType<CutManager>()) == null)
            Debug.LogError("No CutManager");
        if ((audioManager = FindObjectOfType<AudioManager>()) == null)
            Debug.LogError("No AudioManager");
        if ((playerManager = FindObjectOfType<PlayerManager>()) == null)
            Debug.LogError("No PlayerManager");
        if ((uiManager = FindObjectOfType<StageUIManager>()) == null)
            Debug.LogError("No uiManagerr");
        cutManager.CutInit();
        playerManager.PlayerManagerInit();
        audioManager.AudioInit();
        uiManager.CanvasInit(sceneChangeTime, sceneReStartTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isOption)
        {
            StageRestart();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOption)
            {
                uiManager.ESCMenuActive(false);
                isOption = false;

            }
            else
            {
                uiManager.ESCMenuActive(true);
                isOption = true;
            }
        }
        if (Application.isEditor && Input.GetKeyDown(KeyCode.N))
        {
            StageClear();
        }

    }

    public void StageRestart() {
        StartCoroutine(StageRestartTime());
    }

    IEnumerator StageRestartTime()
    {
        yield return new WaitForSeconds(sceneReStartTime);
        audioManager.FadingAudio(false);
        GameManager.GetInstance().SceneRestart();
    }

    public void StageClear()
    {
        audioManager.PlaySfx(AudioManager.ESfxType.STAGE_CLEAR);
        playerManager.StageClear();
        SceneChange(StageType.Stage);
    }

    public void SceneChange(StageType stageType)
    {
        audioManager.FadingAudio(false);
        uiManager.StageClaer();
        StartCoroutine(StageChangeTime(stageType));
    }

    IEnumerator StageChangeTime(StageType stageType)
    {
        yield return new WaitForSeconds(sceneChangeTime);
        if (stageType == StageType.Stage)
        {
            if (stageValue < StageBackgroundList.Length)
                GameManager.GetInstance().SceneLoad(StageType.Stage, stageValue);
            else
                GameManager.GetInstance().SceneLoad(StageType.Endding);
        }
        else
        {
            GameManager.GetInstance().SceneLoad(stageType);
        }
    }


    public void NextCut()
    {
        cutManager.MoveToNextCut();
        if (cutManager.GetCurrentCutNum() > 5)
        {
            StageRestart();
        }
        else
        {
            audioManager.PlayCutChangeAudio();
            playerManager.MoveToNextCut();
        }

    }
}
