using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendMailToDev : MonoBehaviour
{
    public InputField input;
    public GameObject SendResultBurron;

    public void OnClickSend()
    {
        StartCoroutine(Send());
    }
    
    IEnumerator Send()
    {
        var formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(input.text + "\n"));
        UnityWebRequest www = UnityWebRequest.Post("https://coco1337.asuscomm.com/feedback", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            // TODO : 실패
            Debug.Log(www.error);
            SendResultBurron.SetActive(true);
        }
        else
        {
            // TODO : Send 완료 메시지 연결
            Debug.Log("Send Complete");
            SendResultBurron.SetActive(true);
        }
    }
}
