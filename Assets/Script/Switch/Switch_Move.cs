using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Move : MonoBehaviour, Switchable
{
    [SerializeField] Transform startPosition;
    [SerializeField] Transform endPosition;
    [SerializeField] float speed = 1;
    Vector3 targetPosition;

    private void Start()
    {
        transform.position = startPosition.position;
    }

    public void SwitchOn()
    {
        StopCoroutine(Move());
        targetPosition = endPosition.position;
        StartCoroutine(Move());
    }

    public void SwitchOff()
    {
        StopCoroutine(Move());
        targetPosition = startPosition.position;
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
