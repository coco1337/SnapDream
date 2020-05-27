using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClearManager : MonoBehaviour
{

    [SerializeField]
    int ChapterNum = 0;
    [SerializeField]
    int curretnStageNum = 0;
    [SerializeField]
    float SceanChangeTime = 3f;
    [SerializeField]
    Image paidImage;

    [SerializeField]
    Image clearImage;

    AudioSource audioSource;
    bool canInteratable = true;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(Application.dataPath);
        //byte[] byteTexture;
        //if (Application.isEditor)
        //    byteTexture = System.IO.File.ReadAllBytes(Application.dataPath+ @"\Resources\ClearLobby.jpg");
        //else
        //    byteTexture = System.IO.File.ReadAllBytes(Application.dataPath + @"\Resources\ClearLobby.jpg");
        //Texture2D texture = new Texture2D(0, 0);
        //texture.LoadImage(byteTexture);
        //clearImage.sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f));

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
                StartCoroutine("PaidAudio", false);
                StartCoroutine("PaidImage", false);
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
            SceneManager.LoadScene("End");
    }
}
