using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiletleMove : MonoBehaviour
{
    [SerializeField]
    float maxMovePosition = 3f;
    [SerializeField]
    float moveTime = 1f;
    float directionTime;
    bool moveDirection;
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        directionTime = Time.time + moveTime;
        moveDirection = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time < directionTime)
        {
            Vector3 movePosition = new Vector3(960, transform.position.y + ((moveDirection) ? maxMovePosition : -maxMovePosition) * Time.deltaTime/moveTime, 0);
            rectTransform.position = movePosition;
        }
        else
        {
            directionTime = Time.time + moveTime;
            moveDirection = !moveDirection;
        }
    }
}
