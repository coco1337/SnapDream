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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, -0.5f, 0), new Vector3(1.7f, 0, 0));

    }

    public bool CanInterectable()
    {
        int playerDirection = player.IsPlyerFlip() ? -1 : 1;
        Collider2D collider = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(playerDirection * 1.7f, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        if (collider != null && collider.CompareTag("Throw"))
        {
            Debug.Log(collider.name);
            throwObject = collider.gameObject;
            return true;
        }
        else
            return false;
    }

    public void Interacting()
    {
        if (throwObject.GetComponent<InteractableObject>().Throw(player.throwPower))
        {
            player.getThrow();
            player.realeaseDrag();
            dragObject = null;
        }
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
