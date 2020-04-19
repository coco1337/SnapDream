using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CartoonManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> whiteImageList = new List<GameObject>();

    public Transform parrentWhiteImage;

    [SerializeField]
    int curretnCutNum = 0;
    [SerializeField]
    int MaxCutNum;
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
        for (int i = 0; i < parrentWhiteImage.childCount; i++)
        {
            whiteImageList.Add(parrentWhiteImage.GetChild(i).gameObject);
        }
        MaxCutNum = whiteImageList.Count;
        canInteratable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteratable)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(1))
            {
                if (curretnCutNum < MaxCutNum)
                {
                    StartCoroutine("PaidingWhiteImage", whiteImageList[curretnCutNum]);
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

    IEnumerator PaidingWhiteImage(GameObject imageObject)
    {
        float dirTime = Time.time + 1.5f;
        Image image = imageObject.GetComponent<Image>();
        while (Time.time < dirTime)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 0.04f);
            yield return new WaitForSeconds(0.05f);
        }
        imageObject.SetActive(false);
        canInteratable = true;
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
            SceneManager.LoadScene("Stage1");
        
    }
}
