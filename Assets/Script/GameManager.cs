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
    AudioSource audioSource;

    [SerializeField]
    float SceanChangeTime = 3f;

    [SerializeField]
    Image paidImage;

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


        audioSource = this.GetComponent<AudioSource>();
        audioSource.volume = 0.24f;
        StartCoroutine("PaidAudio", true);

        StartCoroutine("PaidImage", true);
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
            StageRestart();
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
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void StageClear()
    {
        if (Application.isEditor == true)
        {
            ScreenCapture.CaptureScreenshot("Assets\\ScreenShot\\Clear " + sceneName + ".png");
        }
        else
        {
            ScreenCapture.CaptureScreenshot("Clear " + sceneName + ".png");

        }

        if (sceneName != "Lobby")
        {
            foreach (var player in playerList)
            {
                player.StageClear();
            }
        }

        StartCoroutine("MoveNextStage");
        StartCoroutine("PaidAudio", false);
        StartCoroutine("PaidImage", false);
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
        camImage[currentCut].SetActive(true);
    }

    IEnumerator MoveNextStage()
    {
        yield return new WaitForSeconds(SceanChangeTime);
        SceneManager.LoadScene(LevelName[sceneNum + 1]);
    }

    

    IEnumerator PaidAudio(bool paid)
    {
        float dirTime = Time.time + SceanChangeTime;
        while (Time.time < dirTime)
        {
            audioSource.volume += (paid) ? 0.01f : -0.018f;
            if (paid)
            {
                if (audioSource.volume > 0.5f)
                    StopCoroutine("PaidAudio");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator PaidImage(bool paid)
    {
        if(!paid && sceneName != "Lobby")
            yield return new WaitForSeconds(1);
        float dirTime = Time.time + (paid ? SceanChangeTime/2 : SceanChangeTime);
        paidImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            paidImage.color = (paid) ? new Color(paidImage.color.r, paidImage.color.g, paidImage.color.b, paidImage.color.a - 0.04f) : new Color(paidImage.color.r, paidImage.color.g, paidImage.color.b, paidImage.color.a + 0.022f);

            Debug.Log(paidImage.color.a);
            yield return new WaitForSeconds(0.05f);
        }
        if(paid)
            paidImage.gameObject.SetActive(false);
    }
}
