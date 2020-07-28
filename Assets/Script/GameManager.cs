using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    static GameManager instance;
    [SerializeField] string[] LevelName;
    string sceneName;
    int sceneNum;
    [SerializeField] private GameObject exitGameUI;
    [SerializeField] private GameObject exitStageUI;
    [SerializeField] private GameObject lobbyUI;

    private CutManager cutManager;
    private AudioManager audioManager;
    private PlayerManager playerManager;
    private CanvasManager canvasManager;

    [SerializeField] private float SceanChangeTime = 3f;
    [SerializeField] private float SceanReStartTime = 1f;

    [SerializeField] private Image fadingImage;
    [SerializeField] bool isOption = false;

    public AudioManager GetAudioManager => audioManager;
    public CutManager GetCutManager => cutManager;
    static public GameManager GetInstance() => instance;
    public void ExitStage() => SceneManager.LoadScene("Lobby");
    public void ExitGame() => Application.Quit();
    public bool isOptioning => isOption;
    public void SetIsOption(bool opt) => isOption = opt;
    public int GetCurrentCutNum() => cutManager.GetCurrentCutNum();

    void Start()
    {
        isOption = false;
        Screen.SetResolution(1920, 1080, true);
        instance = FindObjectOfType<GameManager>();
        if (Application.isEditor) {
            if (FindObjectOfType<CutManager>() == null)
                Debug.LogError("No CutManager");
            if (FindObjectOfType<AudioManager>() == null)
                Debug.LogError("No AudioManager");
        }
        cutManager = FindObjectOfType<CutManager>();
        audioManager = FindObjectOfType<AudioManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        sceneName = SceneManager.GetActiveScene().name;
        sceneNum = SceneManager.GetActiveScene().buildIndex;

        if (sceneName != "Lobby")
            cutManager.CutInit();
        else
            lobbyUI.SetActive(true);
        playerManager.PlayerManagerInit();

        exitGameUI.SetActive(false);
        exitStageUI.SetActive(false);

        audioManager.AudioInit();

        StartCoroutine(FadeImage(true));
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
            isOption = true;
            if (sceneName == "Lobby")
            {
                exitGameUI.SetActive(true);
            }
            else
            {
                exitStageUI.SetActive(true);
            }
        }
        if (Application.isEditor && Input.GetKeyDown(KeyCode.N))
        {
            StageClear();
        }
    }

    void StageStart()
    {

    }

    public void StageRestart()
    {
        StartCoroutine(StageRestartCoroutin(sceneName));
        audioManager.FadingAudio(false);
        StartCoroutine(FadeImage(false));
    }

    public void StageClear()
    {
        audioManager.PlaySfx(AudioManager.SfxType.ESTAGE_CLEAR);
        if (sceneName != "Lobby")
        {
            cutManager.StageClear();
        }

        if(sceneNum+1 > LevelName.Length)
            StartCoroutine("MoveStage", "Lobby");
        else
            StartCoroutine("MoveStage", LevelName[sceneNum + 1]);
        audioManager.FadingAudio(false);
        StartCoroutine(StageClearFadeImage(false));
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

    IEnumerator StageRestartCoroutin(string sceneName)
    {
        yield return new WaitForSeconds(SceanReStartTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator MoveStage(string sceneName)
    {
        yield return new WaitForSeconds(SceanChangeTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeImage(bool fade)
    {
        float dirTime = Time.time + (fade ? SceanChangeTime / 2 : SceanChangeTime);
        fadingImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            fadingImage.color = (fade) ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a - 0.04f) : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a + 0.022f);
            yield return new WaitForSeconds(0.05f);
        }
        if (fade)
            fadingImage.gameObject.SetActive(false);
    }

    IEnumerator StageClearFadeImage(bool fade)
    {
        if (!fade && sceneName != "Lobby")
            yield return new WaitForSeconds(1);
        float dirTime = Time.time + (fade ? SceanChangeTime / 2 : SceanChangeTime);
        fadingImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            fadingImage.color = (fade) ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a - 0.04f) : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a + 0.022f);
            yield return new WaitForSeconds(0.05f);
        }
        if (fade)
            fadingImage.gameObject.SetActive(false);
    }
}
