using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    [SerializeField]
    string[] LevelName;
    string sceneName;
    int sceneNum;

    [SerializeField]
    GameObject backGround;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject cutCamera;
    [SerializeField]
    RenderTexture[] cameraRawImage;
    [SerializeField]
    float cameraBoundury = 20f;
    [SerializeField]
    Vector2 spawnPosition;
    [SerializeField]
    Transform cutField;
    List<Player> playerList = new List<Player>();

    [SerializeField]
    RectTransform canvas;
    [SerializeField]
    List<GameObject> camImage = new List<GameObject>();
    int currentCut = 0;

    [SerializeField]
    GameObject exitGameUI;
    [SerializeField]
    GameObject exitStageUI;
    [SerializeField]
    GameObject lobbyUI;

    [SerializeField]
    AudioSource audioBGMSource;
    [SerializeField]
    AudioSource audioCutChangeSource;
    [SerializeField]
    AudioSource audioStageClearSource;

    [SerializeField]
    float SceanChangeTime = 3f;

    [SerializeField]
    Image fadingImage;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        instance = FindObjectOfType<GameManager>();
        sceneName = SceneManager.GetActiveScene().name;
        sceneNum = SceneManager.GetActiveScene().buildIndex;
        if (sceneName != "Lobby")
            InitiatingCut();
        else
            lobbyUI.SetActive(true);

        exitGameUI.SetActive(false);
        exitStageUI.SetActive(false);


        audioBGMSource = this.GetComponent<AudioSource>();
        audioBGMSource.volume = 0.1f;
        audioStageClearSource.volume = 0.1f;

        StartCoroutine("fadeAudio", true);
        StartCoroutine("fadeImage", true);
    }

    void InitiatingCut()
    {
        currentCut = 0;

        for (int i = 0; i < cutField.childCount; i++)
        {
            Transform cut = cutField.GetChild(i);
            GameObject tempBackGround = Instantiate(backGround, Vector3.zero, Quaternion.identity);
            GameObject tempPlayer = Instantiate(player, Vector3.zero, Quaternion.identity);
            GameObject tempCamera = Instantiate(cutCamera, Vector3.zero, Quaternion.identity);
            
            tempCamera.GetComponent<Camera>().targetTexture = cameraRawImage[i];

            //Player Setting
            tempPlayer.GetComponent<Player>().SetPlayerCutNumber(i);

            //Camera Setting
            CameraController tempCameraController = tempCamera.GetComponent<CameraController>();
            tempCameraController.basePoint = tempBackGround.transform;
            tempCameraController.player = tempPlayer.transform;
            tempCameraController.bounduryValue = cameraBoundury;

            tempBackGround.transform.parent = cut;
            tempPlayer.transform.parent = cut;
            tempCamera.transform.parent = cut;

            tempBackGround.transform.localPosition = Vector3.zero;
            tempPlayer.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            tempCamera.transform.localPosition = Vector3.zero + new Vector3(0,0, -9);

            playerList.Add(tempPlayer.GetComponent<Player>());

        }

        for(int i = 0; i < canvas.childCount; i++)
        {
            camImage.Add(canvas.GetChild(i).gameObject);
            camImage[i].SetActive(false);
            camImage[i].GetComponent<RawImage>().texture = cameraRawImage[i];
        }
        camImage[0].SetActive(true);
    }

    static public GameManager getInstance()
    {
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(sceneName);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(sceneName == "Lobby")
            {
                exitGameUI.SetActive(true);
            }
            else
            {
                exitStageUI.SetActive(true);
            }
        }
    }

    public void StageRestart()
    {
        StartCoroutine("MoveStage", sceneName);
        StartCoroutine("fadeAudio", false);
        StartCoroutine("fadeImage", false);
    }

    public void StageClear()
    {
        if(!Application.isEditor)
            ScreenCapture.CaptureScreenshot(@".\Resources\Clear" + sceneName+ @".jpg");
        audioStageClearSource.Play();
        if (sceneName != "Lobby")
        {
            foreach (var player in playerList)
            {
                player.StageClear();
            }
        }

        StartCoroutine("MoveStage", LevelName[sceneNum + 1]);
        StartCoroutine("fadeAudio", false);
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
        currentCut++;
        if (currentCut > 5)
            return;
        audioCutChangeSource.Play();
        camImage[currentCut].SetActive(true);
    }

    IEnumerator MoveStage(string sceneName)
    {
        yield return new WaitForSeconds(SceanChangeTime);
        SceneManager.LoadScene(sceneName);
    }

    

    IEnumerator fadeAudio(bool fade)
    {
        float dirTime = Time.time + SceanChangeTime;
        while (Time.time < dirTime)
        {
            audioBGMSource.volume += (fade) ? 0.01f : -0.018f;
            if (fade)
            {
                if (audioBGMSource.volume > 0.3f)
                    StopCoroutine("fadeAudio");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator fadeImage(bool fade)
    {
        float dirTime = Time.time + (fade ? SceanChangeTime/2 : SceanChangeTime);
        fadingImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            fadingImage.color = (fade) ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a - 0.04f) : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a + 0.022f);

            yield return new WaitForSeconds(0.05f);
        }
        if(fade)
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
}
