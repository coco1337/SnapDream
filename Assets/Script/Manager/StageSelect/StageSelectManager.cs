using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour, ISceneManager
{
    public StageSelectUIManager uiManager;

    public void SceneInit(int sceneValue)
    {
        uiManager.Init();
    }

    void Start()
    {
        SceneInit(0);
    }

    public void StageStart(int chapterNum, int stageNum)
    {
        GameManager.GetInstance().SceneLoad(StageType.Stage, ((chapterNum - 1) * 5 + stageNum -1));
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GetInstance().isGameManagerActioning)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                uiManager.NextButton();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                uiManager.PrevButton();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                uiManager.ButtonSelect();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            {
                uiManager.ButtonCancle();
            }
        }
    }
}
