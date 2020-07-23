using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Active : MonoBehaviour, Switchable
{
    ScriptableObject targetScript;

    public void SwitchOff()
    {
        gameObject.SetActive(false);
    }

    public void SwitchOn()
    {
        gameObject.SetActive(true);
    }
}
