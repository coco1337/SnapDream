using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    [SerializeField] string[] levelName;
    string sceneName;
    int sceneNum;
    [SerializeField] private GameObject exitGameUI;
    [SerializeField] private GameObject exitStageUI;
    [SerializeField] private GameObject lobbyUI;

    private CutManager cutManager;
    private AudioManager audioManager;

    [SerializeField] private float SceanChangeTime = 3f;
    [SerializeField] private float SceanReStartTime = 1f;

    [SerializeField] private Image fadingImage;
    [SerializeField] bool isOption = false;

    // Start is called before the first frame update
    void Start()
    {
        isOption = false;
        Screen.SetResolution(1920, 1080, true);
        instance = FindObjectOfType<GameManager>();
        cutManager = FindObjectOfType<CutManager>();
        audioManager = FindObjectOfType<AudioManager>();
        sceneName = SceneManager.GetActiveScene().name;
        sceneNum = SceneManager.GetActiveScene().buildIndex;

        if (sceneName != "Lobby")
            cutManager.CutInit();
        else
            lobbyUI.SetActive(true);

        exitGameUI.SetActive(false);
        exitStageUI.SetActive(false);

        audioManager.AudioInit();

        StartCoroutine("fadeImage", true);
    }

    static public GameManager getInstance() => instance;
    

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

    public void StageRestart()
    {
        StartCoroutine("StageRestart_Coroutin", sceneName);
        audioManager.FadingAudio(false);
        StartCoroutine("fadeImage", false);
    }

    public void StageClear()
    {
        audioManager.PlayStageClaerAudio();
        if (sceneName != "Lobby")
        {
            cutManager.StageClear();
        }

        StartCoroutine("MoveStage", levelName[sceneNum + 1]);
        audioManager.FadingAudio(false);
        StartCoroutine("stageCLearfadeImage", false);
    }

    public void ExitStage()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void ExitGame()
    {
        Application.Quit();
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
        }
    }

    IEnumerator StageRestart_Coroutin()
    {
        yield return new WaitForSeconds(SceanReStartTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator MoveStage(string _sceneName)
    {
        yield return new WaitForSeconds(SceanChangeTime);
        SceneManager.LoadScene(_sceneName);
    }

    IEnumerator fadeImage(bool fade)
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

    IEnumerator stageCLearfadeImage(bool fade)
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

    public bool isOptioning()
    {
        return isOption;
    }

    public void SetIsOption(bool opt)
    {
        Debug.Log(opt);
        isOption = opt;
    }

    public int GetCurrentCutNum()
    {
        return cutManager.GetCurrentCutNum();
    }

}
