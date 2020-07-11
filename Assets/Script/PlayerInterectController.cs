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
                if(dragObject.GetComponent<InteractableObject>().Drag(axis, player.dragSpeed))
                    player.getDrag();
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
        if (collision.gameObject.CompareTag("Drag") || collision.gameObject.CompareTag("Throw"))
        {
            //Player의 Rigidbody의 충돌판정을 잠시 없앤 뒤(무적상태), 뒤로가는 애니메이션 진행
            if (collision.transform.position.y > player.transform.position.y + 1)
            {
                if(collision.transform.position.x > player.transform.position.x)
                    player.transform.position =  new Vector3(collision.transform.position.x - 2f, player.transform.position.y);
                else
                    player.transform.position = new Vector3(collision.transform.position.x + 2f, player.transform.position.y);
            }
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
