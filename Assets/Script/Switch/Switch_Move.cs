using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Move : MonoBehaviour, Switchable
{
    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 endPosition;
    [SerializeField] float speed = 1;
    Vector3 targetPosition;

    private void Start()
    {
        transform.position = startPosition;
    }

    public void SwitchOn()
    {
        StopCoroutine(Move());
        targetPosition = endPosition;
        StartCoroutine(Move());
    }

    public void SwitchOff()
    {
        StopCoroutine(Move());
        targetPosition = startPosition;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }   

}
