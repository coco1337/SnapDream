using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testImageS : MonoBehaviour
{
    // Start is called before the first frame update

    Text text;
    Image image;
    void Start()
    {
        image = this.GetComponent<Image>();
        Debug.Log(Application.dataPath);
        if(!Application.isEditor)
            image.sprite = Resources.Load<Sprite>("Clear Stage1");
        else
            image.sprite = Resources.Load<Sprite>("Clear Stage1");
        Debug.Log(Resources.Load<Sprite>("Clear Stage1"));
        text = transform.parent.GetComponentInChildren<Text>();
        text.text = Resources.Load<Sprite>("Clear Stage1").ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
