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


    // Start is called before the first frame update
    void Start()
    {
        instance = FindObjectOfType<GameManager>();
        sceneName = SceneManager.GetActiveScene().name;
        sceneNum = SceneManager.GetActiveScene().buildIndex;
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
        SceneManager.LoadScene(sceneName);
    }
    public void StageClear()
    {
        SceneManager.LoadScene(LevelName[sceneNum+1]);

    }
}
