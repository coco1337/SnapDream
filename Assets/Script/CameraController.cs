using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform basePoint;
    public Transform player;

    public float bounduryValue = 20f;
    float[] bounduryPointX = new float[2];

    Vector3 targetPosition;


    // Start is called before the first frame update
    void Start()
    {
        bounduryPointX[0] = basePoint.position.x - bounduryValue;
        bounduryPointX[1] = basePoint.position.x + bounduryValue;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = transform.position;
        targetPosition.x = Mathf.Clamp(player.position.x, bounduryPointX[0], bounduryPointX[1]);
        transform.position = targetPosition;
    }
}
