using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    public bool isHoldableObject;
    private void Start()
    {
        if (this.gameObject.tag != "InteractableObject")
        {
            this.gameObject.tag = "InteractableObject";
        }
    }
}
