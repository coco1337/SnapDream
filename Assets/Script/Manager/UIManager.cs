using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    public bool isFading;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fadingImage;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Init()
    {
        canvas = GetComponent<Canvas>();
        isFading = false;
    }

    public void FadeIn(float fadingTime)
    {
        if (isFading)
            return;
        StartCoroutine(Fading(false, fadingTime));
    }

    public void FadeOut(float fadingTime)
    {
        if (isFading)
            return;
        StartCoroutine(Fading(true, fadingTime));
    }

    void Update()
    {
        
    }


    //Scene Fading Function
    //fade - true : Faiding Out
    //fade - false : Fading In
    //fadingTime : Second to fade in/out
    IEnumerator Fading(bool fade, float fadingTime)
    {
        isFading = true;
        //-0.5f : 해당 시간동안의 변화는 유저가 인지하지 못하고 오히려 버그로 보일 가능성이 있어서, 0.5초 일찍 종료
        float dirTime = Time.time + fadingTime - 0.2f;
        float fadingAlphaValue = 1 / fadingTime * Time.deltaTime;
        fadingImage.gameObject.SetActive(true);
        fadingImage.color = (fade)
            ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, 1)
            : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, 0);
        Color changeColorValue = (fade) ? new Color(0, 0, 0, -fadingAlphaValue) : new Color(0, 0, 0, fadingAlphaValue);

        Debug.Log("Fading Start\nFading Start Time : " + Time.time + "\nFading dirTime : " + dirTime);
        while (Time.time < dirTime)
        {
            fadingImage.color += changeColorValue;
            yield return null;
        }

        fadingImage.color = (fade)
            ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, 0)
            : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, 1);
        if (fade)
            fadingImage.gameObject.SetActive(false);
        isFading = false;
        Debug.Log("Fading End");
    }
}
