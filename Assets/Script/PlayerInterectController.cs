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
    public GameObject ladderTarget = null;
    Player player;

    private InteractableObject interactableObject;

    private void Start()
    {
        player = transform.GetComponent<Player>();
    }

    public void getLadder()
    {
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
        }
        else
        {
            this.holdingObject.GetComponent<InteractableObject>().Release();
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
            interactableObject.Drag(axis, player.speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            if (collision.transform.name == "Clear")
            {
                GameManager.getInstance().StageClear();
            }

            else
            {
                canInteractable = true;
                interactableObject = collision.gameObject.GetComponent<InteractableObject>();
            }
        }
        else if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderTarget = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            canInteractable = false;
            // interactableObject = null;
        }

        else if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderTarget = null;
        }
    }
}
