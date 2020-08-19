using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform basePoint;
    public Transform player;
    public float boundaryValue = 20f;

    Vector3 targetPosition;
    [SerializeField] Vector2 minBoundaryPosition;
    [SerializeField] Vector2 maxBoundaryPosition;
    [SerializeField] float xRange;
    [SerializeField] float yRange;
    [SerializeField] float yMargine;

    public void Init()
    {
        transform.localPosition = Vector3.zero + new Vector3(0, 0, -9);
        Camera cam = transform.GetComponent<Camera>();

        //cam.fieldOfView = 2.0f * Mathf.Atan(20 * 0.5f / -transform.localPosition.z) * Mathf.Rad2Deg;

        SetCameraBackground(basePoint);
        targetPosition.z = transform.position.z;

    }

    //Camera의 Boundary를 설정하는 함수
    //입력 값으로 SpriteRenderer를 가진 Transform을 받는다.
    public void SetCameraBackground(Transform background)
    {
        SpriteRenderer back = background.GetComponent<SpriteRenderer>();
        float width = (basePoint.GetComponent<SpriteRenderer>().sprite.rect.xMax - basePoint.GetComponent<SpriteRenderer>().sprite.rect.xMin)
            / basePoint.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        float height = (basePoint.GetComponent<SpriteRenderer>().sprite.rect.yMax - basePoint.GetComponent<SpriteRenderer>().sprite.rect.yMin) * background.localScale.y
        / basePoint.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        Camera cam = transform.GetComponent<Camera>();
        var a = cam.transform.position.z;
        var fov = cam.fieldOfView * .5f;
        fov = fov * Mathf.Deg2Rad;
        var h = (Mathf.Tan(fov) * a) * (-1);
        var w = (h / cam.pixelHeight) * cam.pixelWidth * (-1);

        Debug.Log("W : " + w + "\nH : " + h);

        xRange = (width / 2) - w;
        yRange = (height / 2) - h;
        yMargine = h - 3f;
        CalculateCameraBoundaryPosition();
    }

    public void CalculateCameraBoundaryPosition()
    {
        minBoundaryPosition.x = basePoint.position.x - xRange;
        minBoundaryPosition.y = basePoint.position.y - yRange;
        maxBoundaryPosition.x = basePoint.position.x + xRange;
        maxBoundaryPosition.y = basePoint.position.y + yRange;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            CalculateCameraBoundaryPosition();

            targetPosition = transform.position;
            targetPosition.x = Mathf.Clamp(player.position.x, minBoundaryPosition.x, maxBoundaryPosition.x);
            targetPosition.y = Mathf.Clamp(player.position.y + yMargine, minBoundaryPosition.y, maxBoundaryPosition.y);
            transform.position = targetPosition;
        }
    }
}
