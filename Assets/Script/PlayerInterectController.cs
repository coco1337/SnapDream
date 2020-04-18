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
    public GameObject ladderExit = null;
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
        if (dragObject != null && player.isGround)
        {
            if (axis > 0 && dragObject.transform.position.x > player.transform.position.x
                || axis < 0 && dragObject.transform.position.x < player.transform.position.x)
            {
                dragObject.GetComponent<InteractableObject>().Drag(axis, player.dragSpeed);
            }
            else
            {
                player.realeaseDrag();
            }
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

        if (collision.gameObject.CompareTag("Ladder Exit"))
        {
            ladderExit = collision.gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player.GetPlayerState() == Player.PlayerState.Interaction_Ladder)
        {
            if (collision.gameObject.CompareTag("Ladder Exit"))
            {
                player.realeaseLadder();
            }
        }


        if (collision.gameObject.CompareTag("Ladder"))
        {
            Debug.Log("Enter Ladder");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderTarget = null;
            player.realeaseLadder();
        }


        if (collision.gameObject.CompareTag("Ladder Exit"))
        {
            ladderExit = null;
        }
    }
}
