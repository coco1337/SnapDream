﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Previous : MonoBehaviour, Damageabel
{
    public Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    [SerializeField] public float speed = 4;
    [SerializeField] public float dragSpeed = 4;
    [SerializeField] public float jumpPower = 5;
    [SerializeField] public float throwPower = 2;
    [SerializeField] int playerCutNum;
    [SerializeField] int currentCutNum = 0;
    public bool isGround;
    Animator animator;
    [SerializeField] float PlayerHealth = 1f;
    [SerializeField] float throwAnimationTime = 1f;

    public enum PlayerState
    {
        Idle, Move, Jump, Interaction_Ladder, Interaction_Throw, Interaction_Drag, DIe, Stop, Damaged, Clear
    }
    [SerializeField] PlayerState playerState;


    public bool IsPlyerFlip => spriteRenderer.flipX;


    private void Start()
    {
        currentCutNum = 0;
        playerState = PlayerState.Idle;
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.28f * transform.localScale.y), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(transform.position + new Vector3(0, -1.28f * transform.localScale.y, 0), 0.07f);
    }

    public void SetPlayerCutNumber(int i) => playerCutNum = i;   

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
            animator.SetBool("isGround", isGround);
            animator.SetFloat("moveSpeed", Mathf.Abs(axis));
            rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
            animator.SetFloat("palyerVerticalSpeed", rigidbody.velocity.y);
        }
    }

    public void PlayerLadderMove(float axis)
    {
        if (playerState == PlayerState.Interaction_Ladder)
        {
            animator.SetFloat("ladderSpeed", Mathf.Abs(axis)+0.2f);
            animator.speed = Mathf.Abs(axis);
            rigidbody.velocity = new Vector2(0, speed * axis);
        }
    }

    public void PlayerJump()
    {
        isGround = false;
        animator.SetBool("isGround", isGround);
        animator.SetTrigger("jump");
        playerState = PlayerState.Jump;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(Vector2.up * jumpPower);
    }

    public void moveNextCut()
    {
        currentCutNum += 1;
        //중복호출 방지
        if (playerCutNum == 5)
        {
            GameManager.GetInstance().NextCut();
        }
    }

    public bool isMovable()
    {
        if (playerState == PlayerState.Stop || playerState == PlayerState.DIe || playerState == PlayerState.Interaction_Throw || playerState == PlayerState.Clear)
            return false;
        return currentCutNum <= playerCutNum;
    }

    public void playerStop()
    {
        playerState = PlayerState.Stop;
        animator.enabled = false;
        foreach (var collider in transform.GetComponents<BoxCollider2D>())
        {
            collider.enabled = false;
        }
        rigidbody.velocity = Vector2.zero;
        rigidbody.bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<Player>().enabled = false;
        gameObject.GetComponent<InputController>().enabled = false;
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

    public bool IsThrowable()
    {
        if (playerState == PlayerState.Idle || playerState == PlayerState.Move || playerState == PlayerState.Interaction_Drag)
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
            animator.SetFloat("ladderSpeed", 0);
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
        animator.SetFloat("dragSpeed", 0);
        if (playerState == PlayerState.Interaction_Drag)
        {
            playerState = PlayerState.Idle;
        }
    }

    public void getThrow()
    {
        if (IsThrowable())
        {
            PlayerMove(0);
            animator.SetTrigger("throwObject");
            animator.SetFloat("moveSpeed", 0);
            animator.SetFloat("dragSpeed", 0);
            playerState = PlayerState.Interaction_Throw;
            StartCoroutine(EndThrowObject());
        }
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
        return currentCutNum;
    }


    public void Hit(float damage)
    {
        PlayerHealth -= damage;
        if(PlayerHealth <= 0)
            DieObject();
    }

    public void DieObject()
    {
        playerStop();
        playerState = PlayerState.DIe;
        //중복호출 방지
        //5번 캐릭터는 최후에 죽으니
        if (playerCutNum == 5)
            GameManager.GetInstance().StageRestart();
    }

    public void StageClear()
    {
        playerState = PlayerState.Clear;
        animator.SetBool("isGround", true);
        if(currentCutNum <= playerCutNum)
        {
            StartCoroutine("StageClearAction");
        }
    }

    IEnumerable StageClearAction()
    {
        float startTime = Time.time + 5;
        while(startTime > Time.time)
        {
            ClearMove(1);
            yield return null;
        }
        Destroy(this);
    }

    void ClearMove(float axis)
    {
        animator.SetBool("isGround", isGround);
        animator.SetFloat("moveSpeed", Mathf.Abs(axis));
        rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
    }

}