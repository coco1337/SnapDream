using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterectController : MonoBehaviour
{
    private bool canInteractable;
    private bool isHoldable;
    public GameObject holdingObject;
    public bool isHoldingObject;
    public bool isInteracting;

    public GameObject throwObject = null;
    public GameObject dragObject = null;
    public GameObject ladderTarget = null;
    Player player;

    private InteractableObject interactableObject;

    private void Start()
    {
        player = transform.GetComponent<Player>();
    }


    public void InteractObject()
    {
        if (!canInteractable && holdingObject == null)
        {
            return;
        }
        if (!this.isHoldingObject)
        {
            interactableObject.Hold();
            this.holdingObject = interactableObject.gameObject;
            isHoldingObject = true;
        }
        else
        {
            this.holdingObject.GetComponent<InteractableObject>().Release();
            holdingObject = null;
            isHoldingObject = false;
        }

        // Ladder의 경우
        // 장난감 상자의 경우
        // 무거운 상자의 경우
    }

    // 물건 옮기기
    public void MoveInteractObject(float axis)
    {
        if (canInteractable && interactableObject != null && player.isGround)
        {

            interactableObject.Drag(axis, player.dragSpeed);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderTarget = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("Drag"))
        {
            dragObject = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("Throw"))
        {
            throwObject = collision.gameObject;
        }
    }

}
