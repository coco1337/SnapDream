using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUIManager : MonoBehaviour
{
    [SerializeField] GameObject chapterPanel;
    [SerializeField] GameObject stagePanel;
    private StageSelectManager stageSelectManager;
    private GameObject currentPanel;
    private bool buttonMode;
    private int currentButtonNum;
    private int selectedChapterNum;
    private int selectedStageNum;

    public void Init()
    {
        stageSelectManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<StageSelectManager>();
        foreach (Image button in chapterPanel.GetComponentsInChildren<Image>())
        {
            button.color = new Color(button.color.r, button.color.g, button.color.b, 0.7f);
        }
        foreach (Image button in stagePanel.GetComponentsInChildren<Image>())
        {
            button.color = new Color(button.color.r, button.color.g, button.color.b, 0.7f);
        }
        stagePanel.SetActive(false);
        chapterPanel.SetActive(true);
        currentPanel = chapterPanel;
        currentButtonNum = -1;
        selectedChapterNum = 0;
        selectedStageNum = 0;
    }

    public void NextButton()
    {
        currentButtonNum = Mathf.Clamp(currentButtonNum + 1, 1, currentPanel.transform.childCount);
        
        SelectedButtonAction(currentButtonNum);
    }

    public void PrevButton()
    {
        currentButtonNum = Mathf.Clamp(currentButtonNum - 1, 1, currentPanel.transform.childCount);
        SelectedButtonAction(currentButtonNum);
    }

    public void ButtonSelect()
    {
        if (currentButtonNum < 1 || currentButtonNum > currentPanel.transform.childCount)
            return;
        if (currentPanel == chapterPanel)
        {
            chapterPanel.SetActive(false);
            stagePanel.SetActive(true);
            currentPanel = stagePanel;
            selectedChapterNum = currentButtonNum;
            currentButtonNum = 0;
            SelectedButtonAction(0);
        }
        else
        {
            stageSelectManager.StageStart(selectedChapterNum, currentButtonNum);
        }
    }

    public void ButtonCancle()
    {
        chapterPanel.SetActive(true);
        stagePanel.SetActive(false);
        currentPanel = chapterPanel;
        currentButtonNum = selectedChapterNum;
        SelectedButtonAction(currentButtonNum);
    }


    private void SelectedButtonAction(int buttonNum = 0)
    {
        Debug.Log(currentButtonNum);
        foreach (Image button in currentPanel.GetComponentsInChildren<Image>())
        {
            button.color = new Color(button.color.r, button.color.g, button.color.b, 0.7f);
        }
        if (buttonNum != 0)
        {
            Image selectedButton = currentPanel.GetComponentsInChildren<Image>()[buttonNum];
            selectedButton.color = new Color(selectedButton.color.r, selectedButton.color.g, selectedButton.color.b, 1f);
        }


    }
}
