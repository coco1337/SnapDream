﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryCollider : MonoBehaviour
{
    // public Vector2[] MiddleOfEachCut;
    public int currentCut;
    public bool verticalBoundary;
    public bool moveOtherCut;
    public Vector2 middlePosition;
    public Vector2 hitPosition;
    public Vector2 spawnPosition;
    private ObjectSyncController objectSyncController;

    // Start is called before the first frame update
    void Start()
    {
        middlePosition = this.transform.parent.position;
        // 추후 수정
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            objectSyncController.HitCollider(
                collision.gameObject.GetComponent<InteractableObject>(), verticalBoundary, this.transform.localPosition);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            objectSyncController.ExitCollider(collision.gameObject.GetComponent<InteractableObject>());
            collision.gameObject.GetComponent<InteractableObject>().SyncNeeded(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            if (!collision.gameObject.GetComponent<InteractableObject>().IsInstantiated)
                collision.gameObject.GetComponent<InteractableObject>().SyncNeeded(true);
        }
    }
}
