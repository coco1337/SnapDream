using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] textList;
    [SerializeField]
    int curretnCutNum = 0;
    [SerializeField]
    float SceanChangeTime = 3f;
    [SerializeField]
    Image paidImage;

    AudioSource audioSource;
    bool canInteratable = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
        audioSource.volume = 0.1f;
        StartCoroutine("PaidImage", true);
        StartCoroutine("PaidAudio", true);

        canInteratable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteratable)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(1))
            {
                if (curretnCutNum == 0)
                {
                    StartCoroutine("fadingText", textList[0]);
                    StartCoroutine("fadeOutText", textList[1]);
                    canInteratable = false;
                    curretnCutNum++;
                }
                else if(curretnCutNum == 1)
                {
                    StartCoroutine("fadingText", textList[1]);
                    StartCoroutine("fadeOutText", textList[2]);
                    canInteratable = false;
                    curretnCutNum++;
                }
                else
                {
                    StartCoroutine("PaidImage", false);
                    StartCoroutine("PaidAudio", false);

                }
            }
        }
    }

    IEnumerator fadeOutText(GameObject textObject)
    {
        yield return new WaitForSeconds(2f);
        textObject.SetActive(true);
        float dirTime = Time.time + 1.5f;
        Text fadeText = textObject.GetComponent<Text>();
        fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 0);
        while (Time.time < dirTime)
        {
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, fadeText.color.a + 0.04f);
            yield return new WaitForSeconds(0.05f);
        }
        canInteratable = true;
    }

    IEnumerator fadingText(GameObject textObject)
    {
        float dirTime = Time.time + 1.5f;
        Text fadeText = textObject.GetComponent<Text>();
        fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 1);
        while (Time.time < dirTime)
        {
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, fadeText.color.a - 0.04f);
            yield return new WaitForSeconds(0.05f);
        }
        textObject.SetActive(false);
    }

    IEnumerator PaidAudio(bool paid)
    {
        float dirTime = Time.time + SceanChangeTime;
        while (Time.time < dirTime)
        {
            audioSource.volume += (paid) ? 0.01f : -0.018f;
            if (paid)
            {
                if (audioSource.volume > 0.4f)
                    StopCoroutine("PaidAudio");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }




    IEnumerator PaidImage(bool paid)
    {
        float dirTime = Time.time + (paid ? SceanChangeTime / 2 : SceanChangeTime);
        paidImage.gameObject.SetActive(true);
        while (Time.time < dirTime)
        {
            paidImage.color = (paid) ? new Color(paidImage.color.r, paidImage.color.g, paidImage.color.b, paidImage.color.a - 0.04f) : new Color(paidImage.color.r, paidImage.color.g, paidImage.color.b, paidImage.color.a + 0.022f);
            yield return new WaitForSeconds(0.05f);
        }
        if (paid)
            paidImage.gameObject.SetActive(false);
        else
            SceneManager.LoadScene("Lobby");
    }
}
