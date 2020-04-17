using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterectControler : MonoBehaviour
{
    private bool canInteractable;
    private bool isHoldable;
    public GameObject holdingObject;
    public bool isHoldingObject;
    public bool isInteracting;
    Player player;

    private InteractableObject interactableObject;

    private void Start()
    {
        player = transform.GetComponent<Player>();
    }

    public void getLabber() { }

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


        // Lobber의 경우
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
        if (collision.gameObject.tag == "InteractableObject" && collision.transform.name == "Clear")
        {
            GameManager.getInstance().StageClear();
        }
        if (collision.gameObject.tag == "InteractableObject")
        {
            canInteractable = true;
            interactableObject = collision.gameObject.GetComponent<InteractableObject>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            canInteractable = false;
            // interactableObject = null;
        }
    }
}
