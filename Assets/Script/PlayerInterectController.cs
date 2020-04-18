using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterectController : MonoBehaviour
{
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

    private void FixedUpdate()
    {
        if(player.GetPlayerState() == Player.PlayerState.Interaction_Drag && dragObject == null)
        {
            player.realeaseDrag();
        }

        if(player.GetPlayerState() == Player.PlayerState.Interaction_Ladder && ladderTarget == null)
        {
            player.realeaseLadder();
        }
    }

    public bool CanInterectable()
    {
        if (throwObject != null)
            return true;
        else 
            return false;
    }

    public void Interacting()
    {
        player.realeaseDrag();
        throwObject.GetComponent<InteractableObject>().Throw(player.throwPower);
        player.getThrow();
        dragObject = null;
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
                dragObject.GetComponent<InteractableObject>().Drag(0, player.dragSpeed);
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
        if (collision.gameObject.CompareTag("Drag") || collision.gameObject.CompareTag("Throw"))
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
        if (collision.gameObject.CompareTag("Ladder"))
        {
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

        if (collision.gameObject.CompareTag("Drag") || collision.gameObject.CompareTag("Throw"))
        {
            dragObject = null;
        }
        if (collision.gameObject.CompareTag("Throw"))
        {
            throwObject = null;
        }
    }
}
