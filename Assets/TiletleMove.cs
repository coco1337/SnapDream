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
            float dirPositionY = transform.position.y + ((moveDirection) ? maxMovePosition : -maxMovePosition);
            rectTransform.position = new Vector3(960, Mathf.Lerp(transform.position.y, dirPositionY, (moveTime - (directionTime - Time.time)) / moveTime), 0);
        }
        else
        {
            directionTime = Time.time + moveTime;
            moveDirection = !moveDirection;
        }
    }
}
