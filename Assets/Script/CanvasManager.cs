using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject exitGameUI;
    [SerializeField] private GameObject exitStageUI;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private Image fadingImage;
    [SerializeField] private float sceneChangeTime;
    [SerializeField] private float sceneRestartTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void CanvasInit(float sChangeTime, float sRestartTime)
    {
        sceneChangeTime = sChangeTime;
        sceneRestartTime = sRestartTime;
        exitGameUI.SetActive(false);
        exitStageUI.SetActive(false);
        StartCoroutine(FadeImage(true));
    }

    public void ESCMenuActive(bool active)
    {
        exitStageUI.SetActive(active);
    }

    public void StageClaer()
    {
        StartCoroutine(StageClearFadeImage(false));
    }


    IEnumerator FadeImage(bool fade)
    {
        float dirTime = Time.time + (fade ? sceneChangeTime / 2 : sceneChangeTime);
        fadingImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            fadingImage.color = (fade)
                ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a - 0.04f)
                : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a + 0.022f);
            yield return new WaitForSeconds(0.05f);
        }
        if (fade)
            fadingImage.gameObject.SetActive(false);
    }

    IEnumerator StageClearFadeImage(bool fade)
    {
        if (!fade)
            yield return new WaitForSeconds(1);
        float dirTime = Time.time + (fade ? sceneChangeTime / 2 : sceneChangeTime);
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
