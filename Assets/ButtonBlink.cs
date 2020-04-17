using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBlink : MonoBehaviour
{

    public Image image;
    float alphaChange = -0.01f;

    // Update is called once per frame
    void Update()
    {
        if(image.color.a >= 1)
        {
            alphaChange = -0.003f;
        }
        else if(image.color.a <= 0.5f)
        {
            alphaChange = 0.003f;
        }
        Color change = image.color;
        change.a += alphaChange;

        image.color = change;
    }
}
