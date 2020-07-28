using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject exitGameUI;
    [SerializeField] private GameObject exitStageUI;
    [SerializeField] private GameObject lobbyUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CanvasInit()
    {

    }

    //IEnumerator SceneFading(bool fade)
    //{
    //    float dirTime = Time.time + (fade ? SceanChangeTime / 2 : SceanChangeTime);
    //    fadingImage.gameObject.SetActive(true);
    //    while (Time.time < dirTime)
    //    {
    //        fadingImage.color = (fade) ? new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a - 0.04f) : new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, fadingImage.color.a + 0.022f);
    //        yield return new WaitForSeconds(0.05f);
    //    }
    //    if (fade)
    //        fadingImage.gameObject.SetActive(false);
    //}
}
