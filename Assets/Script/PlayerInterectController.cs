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

    private void Update()
    {
        if(player.GetPlayerState() == Player.PlayerState.Interaction_Drag && dragObject == null)
        {
            player.RealeaseDrag();
        }

        if(player.GetPlayerState() == Player.PlayerState.Interaction_Ladder && ladderTarget == null)
        {
            player.RealeaseLadder();
        }
    }

    public bool CanInterectable()
    {
        int playerDirection = player.IsPlyerFlip ? -1 : 1;
        Collider2D collider = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(playerDirection * 1.7f, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        if (collider != null && collider.CompareTag("Throw"))
        {
            throwObject = collider.gameObject;
            return true;
        }
        else
            return false;
    }

    public bool CanThrow()
    {
        Collider2D collider = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2((player.IsPlyerFlip ? -1 : 1) * 1.7f, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        if (collider != null && collider.CompareTag("Throw"))
        {
            throwObject = collider.gameObject;
            return true;
        }
        else
            return false;
    }

    public bool CanDrag()
    {
        Collider2D collider = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2((player.IsPlyerFlip ? -1 : 1) * 1f, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        if (collider != null && (collider.CompareTag("Throw") || collider.CompareTag("Drag")))
        {
            throwObject = collider.gameObject;
            return true;
        }
        else
        {
            player.RealeaseDrag();
            if (dragObject != null)
            {
                dragObject.GetComponent<InteractableObject>().Drag(0, player.dragSpeed);
                dragObject = null;
            }
            return false;
        }
    }

    public void Interacting()
    {
        if (throwObject.GetComponent<InteractableObject>().Throw(player.throwPower))
        {
            player.GetThrow();
            player.RealeaseDrag();
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
                if(dragObject.GetComponent<InteractableObject>().Drag(axis, player.dragSpeed))
                    player.GetDrag();
            }
            else
            {
                dragObject.GetComponent<InteractableObject>().Drag(0, player.dragSpeed);
                player.RealeaseDrag();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Drag") || collision.gameObject.CompareTag("Throw"))
        {
            if (collision.transform.position.y > player.transform.position.y + 1)
            {
                if (collision.transform.position.x > player.transform.position.x)
                    player.transform.position = new Vector3(collision.transform.position.x - 2f, player.transform.position.y);
                else
                    player.transform.position = new Vector3(collision.transform.position.x + 2f, player.transform.position.y);
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
    

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderTarget = null;
            player.RealeaseLadder();
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
