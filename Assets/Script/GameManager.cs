using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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



    // Start is called before the first frame update
    void Start()
    {
        instance = FindObjectOfType<GameManager>();
        sceneName = SceneManager.GetActiveScene().name;
        sceneNum = SceneManager.GetActiveScene().buildIndex;
        if (sceneName != "Lobby")
            InitiatingCut();
    }

    void InitiatingCut()
    {
        for(int i = 0; i < cutField.childCount; i++)
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

        }
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
            ScreenCapture.CaptureScreenshot("..\\Assets\\ScreenShot\\Clear " + sceneName + ".png");

        }
        SceneManager.LoadScene(LevelName[sceneNum+1]);

    }
}
