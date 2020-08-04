using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform basePoint;
    public Transform player;
    public float bounduryValue = 20f;

    Vector3 targetPosition;
    Vector2 minBounduryPosition;
    Vector2 maxBounduryPosition;
    [SerializeField] float xMargin = 8;
    [SerializeField] float yMargin = 2.5f;

    public void Init()
    {

    }

    //Camera의 Boundury를 설정하는 함수
    //입력 값으로 SpriteRenderer를 가진 Transform을 받는다.
    public void SetCameraBackground(Transform background)
    {
        SpriteRenderer back = background.GetComponent<SpriteRenderer>();
        float width = (basePoint.GetComponent<SpriteRenderer>().sprite.rect.xMax - basePoint.GetComponent<SpriteRenderer>().sprite.rect.xMin)
            / basePoint.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        float height = (basePoint.GetComponent<SpriteRenderer>().sprite.rect.yMax - basePoint.GetComponent<SpriteRenderer>().sprite.rect.yMin) * background.localScale.y
        / basePoint.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        minBounduryPosition.x = basePoint.position.x - (width / 2) + xMargin;
        minBounduryPosition.y = basePoint.position.y - (height / 2) + yMargin;
        //- (height / 2) + ;
        maxBounduryPosition.x = basePoint.position.x + (width / 2) - xMargin;
        maxBounduryPosition.y = basePoint.position.y + (height / 2) - yMargin;
        //+ (height / 2) - yMargin;

        Debug.Log("width : " + width);
        Debug.Log("Height : " + height);
        Debug.Log("Y Min : " + minBounduryPosition.y + "\nY Max : " + maxBounduryPosition.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCameraBackground(basePoint);
        targetPosition.z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = transform.position;
        targetPosition.x = Mathf.Clamp(player.position.x, minBounduryPosition.x, maxBounduryPosition.x);
        targetPosition.y = Mathf.Clamp(player.position.y + yMargin, minBounduryPosition.y, maxBounduryPosition.y);
        transform.position = targetPosition;
    }
}
