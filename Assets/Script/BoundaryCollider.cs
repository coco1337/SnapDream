using System.Collections;
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
        // 상호작용 가능한 오브젝트
        if (collision.gameObject.tag == "InteractableObject")
        {
            var interactableObj = collision.gameObject.GetComponent<InteractableObject>();
            if (interactableObj.triggerEntered)
            {
                return;
            }

            if (interactableObj.CutNum != interactableObj.CurrentCutNum)
            {
                return;
            }

            objectSyncController.HitCollider(interactableObj, verticalBoundary, this.transform.localPosition);
            interactableObj.triggerEntered = true;
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
        // 상호작용 가능한 오브젝트
        if (collision.gameObject.tag == "InteractableObject")
        {
            if (!collision.gameObject.GetComponent<InteractableObject>().IsInstantiated)
                collision.gameObject.GetComponent<InteractableObject>().SyncNeeded(true);
        }
    }
}
