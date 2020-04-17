using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigidbody; 
    public float speed = 4;
    public float jumpPower = 5;
    public int playerCutNum;
    int curretnCutNum = 0;
    public bool isGround;
    public bool isHoldingObject;
    public Vector2 holdingPosition;
    public Vector2 releasePosition;
    public GameObject holdingObject;
    private bool canInteractable;
    private bool isHoldable;
    private InteractableObject interactableObject;
    Animator animator;

    enum PlayerState
    {
        Idle, Move, Jump, Interaction, DIe, Stop
    }

    PlayerState playerState;

    private void Start()
    {
        curretnCutNum = 0;
        playerState = PlayerState.Idle;
        animator = this.GetComponent<Animator>();
    }


    void Update()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
    }

    public void PlayerMove(float axis)
    {
        if (isGround)
            playerState = (axis == 0f) ? PlayerState.Idle : PlayerState.Move;
        else
            playerState = PlayerState.Jump;

        rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);

        // 물건 옮기기
        if (canInteractable && interactableObject != null && isGround)
        {
            interactableObject.Drag(axis, speed);
        }
    }


    public void PlayerJump()
    {
        playerState = PlayerState.Jump;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(Vector2.up * jumpPower);
    }

    public void moveNextCut()
    {
        curretnCutNum++;
    }

    public bool isMovable()
    {
        if (playerState == PlayerState.Stop)
            return false;
        return curretnCutNum <= playerCutNum;
    }

    public void playerStop()
    {
        playerState = PlayerState.Stop;
        animator.SetTrigger("stop");
        rigidbody.velocity = Vector2.zero;
    }

    public void InteractObject()
    {
        if (!canInteractable && holdingObject == null)
        {
            return;
        }

        if (!this.isHoldingObject)
        {
            interactableObject.Hold(this);
            this.holdingObject = interactableObject.gameObject;
        }
        else
        {
            this.holdingObject.GetComponent<InteractableObject>().Release(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
            interactableObject = null;
        }
    }

    public void PlayerDie()
    {
        playerState = PlayerState.DIe;
    }
}
