using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Move : MonoBehaviour, Switchable
{
    [SerializeField] Transform startPositionTransform;
    [SerializeField] Transform endPositionTransform;
    [SerializeField] Transform overlapRectPointA;
    [SerializeField] Transform overlapRectPointB;
    [SerializeField] float speed = 1;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;
    private Collider2D[] companionList;
    private Animator animator;
    private bool isActive;
    private bool isOn;
    private Rigidbody rigidbody;

    private void Start()
    {
        isActive = false;
        isOn = false;
        startPosition = startPositionTransform.position;
        endPosition = endPositionTransform.position;

        transform.position = startPosition;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        companionList = Physics2D.OverlapAreaAll((Vector2)overlapRectPointA.position, (Vector2)overlapRectPointB.position);
        if (isActive)
        {
            if (transform.position != targetPosition)
            {

                var moveVector = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime) - transform.position;

                transform.position += moveVector;
                if (isOn)
                {
                    foreach (Collider2D A in companionList)
                    {
                        if (A.CompareTag("Player") || A.CompareTag("Throw"))
                        {
                            A.transform.position += moveVector;
                        }
                    }
                }
            }
            else
            {
                isActive = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0,0,1,0.3f);
        Gizmos.DrawCube((overlapRectPointA.position + overlapRectPointB.position) / 2, 
            new Vector3(overlapRectPointA.position.x - overlapRectPointB.position.x, overlapRectPointA.position.y - overlapRectPointB.position.y, 1));
    }

    public void SwitchOn()
    {
        if (!isActive)
        {
            isActive = true;
            if (!isOn)
            {
                targetPosition = endPosition;
                isOn = true;
                //StartCoroutine(Move());
            }
            else
            {
                targetPosition = startPosition;
                isOn = false;
                //StartCoroutine(Move());

            }
        }
    }

    public void SwitchOff()
    {
    }

    IEnumerator Move()
    {
        while (transform.position != targetPosition)
        {
            Vector3 moveVector = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime) - transform.position;
            foreach (Collider2D A in companionList)
            {
                if (A.CompareTag("Player") || A.CompareTag("Throw"))
                    A.transform.position += moveVector;
            }
            transform.position += moveVector;
            yield return null;
        }
        isActive = false;
    }   



}
