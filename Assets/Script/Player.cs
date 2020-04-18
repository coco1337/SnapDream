using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, Damageabel
{
    public Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    public float speed = 4;
    public float dragSpeed = 4;
    public float jumpPower = 5;
    [SerializeField]
    int playerCutNum;
    [SerializeField]
    int curretnCutNum = 0;
    public bool isGround;
    public Vector2 holdingPosition;
    public Vector2 releasePosition;
    Animator animator;
    [SerializeField] float PlayerHealth = 1f;
    [SerializeField] float throwAnimationTime = 3f;

    public enum PlayerState
    {
        Idle, Move, Jump, Interaction_Ladder, Interaction_Throw, Interaction_Drag, DIe, Stop, Damaged
    }
    [SerializeField]
    PlayerState playerState;

    private void Start()
    {
        curretnCutNum = 0;
        playerState = PlayerState.Idle;
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.28f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
    }

    public void PlayerMove(float axis)
    {
        if (playerState == PlayerState.Interaction_Drag)
        {
            if (axis != 0)
            {
                spriteRenderer.flipX = (axis == -1);
            }
            animator.SetFloat("dragSpeed", Mathf.Abs(axis));
            rigidbody.velocity = new Vector2(dragSpeed * axis, rigidbody.velocity.y);
        }
        else
        {
            if (isGround)
                playerState = (axis == 0f) ? PlayerState.Idle : PlayerState.Move;
            else
                playerState = PlayerState.Jump;
            if (axis != 0)
            {
                spriteRenderer.flipX = (axis == -1);
            }
            animator.SetFloat("moveSpeed", Mathf.Abs(axis));
            rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
        }
    }

    public void PlayerLadderMove(float axis)
    {
        if (playerState == PlayerState.Interaction_Ladder)
        {
            animator.SetFloat("ladderSpeed", Mathf.Abs(axis));
            animator.speed = axis;
            rigidbody.velocity = new Vector2(0, speed * axis);
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
        if (playerState == PlayerState.Stop || playerState == PlayerState.DIe || playerState == PlayerState.Interaction_Throw)
            return false;
        return curretnCutNum <= playerCutNum;
    }

    public void playerStop()
    {
        playerState = PlayerState.Stop;
        animator.SetTrigger("stop");
        rigidbody.velocity = Vector2.zero;
    }

    public PlayerState GetPlayerState()
    {
        return playerState;    
    }

    public bool IsJumpable()
    {
        if (playerState == PlayerState.Idle || playerState == PlayerState.Move)
            return true;
        else
            return false;
    }

    public void getLadder()
    {
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        if (playerState == PlayerState.Idle || playerState == PlayerState.Move || playerState == PlayerState.Jump)
        {
            playerState = PlayerState.Interaction_Ladder;
        }
    }

    public void realeaseLadder()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        animator.speed = 1;
        if (playerState == PlayerState.Interaction_Ladder)
        {
            playerState = PlayerState.Idle;
        }
    }


    public void getDrag()
    {
        if (playerState == PlayerState.Move)
        {
            playerState = PlayerState.Interaction_Drag;
        }
    }

    public void realeaseDrag()
    {
        if (playerState == PlayerState.Interaction_Drag)
        {
            playerState = PlayerState.Idle;
        }
    }

    public void getThrow()
    {
        if (playerState == PlayerState.Idle)
        {
            playerState = PlayerState.Interaction_Throw;
            ThrowObject();
        }
    }

    public void ThrowObject()
    {
        playerState = PlayerState.Interaction_Throw;
        StartCoroutine(EndThrowObject());
    }

    IEnumerator EndThrowObject()
    {
        yield return new WaitForSeconds(throwAnimationTime);
        if (playerState == PlayerState.Interaction_Throw)
        {
            playerState = PlayerState.Idle;
        }
    }

    public int GetPlayerCutNumber()
    {
        return playerCutNum;
    }

    public int GetCurrentCutNumber()
    {
        return curretnCutNum;
    }


    public void Hit(float damage)
    {
        PlayerHealth -= damage;
        if(PlayerHealth <= 0)
            DieObject();
    }

    public void DieObject()
    {
        playerState = PlayerState.DIe;
        //중복호출 방지
        //5번 캐릭터는 최후에 죽으니
        if (playerCutNum == 5)
            GameManager.getInstance().StageRestart();
    }

    
}
