﻿using System.Collections;
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
    public Vector2 holdingPosition;
    public Vector2 releasePosition;
    Animator animator;
    

    public enum PlayerState
    {
        Idle, Move, Jump, Interaction_Labber, Interaction_Throw, DIe, Stop, Damaged
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
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.28f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
    }

    public void PlayerMove(float axis)
    {
        if (isGround)
            playerState = (axis == 0f) ? PlayerState.Idle : PlayerState.Move;
        else
            playerState = PlayerState.Jump;

        rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
    }

    public void PlayerLabberMove(float axis)
    {

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
        if (playerState == PlayerState.Stop || playerState == PlayerState.DIe)
            return false;
        return curretnCutNum <= playerCutNum;
    }

    public void playerStop()
    {
        playerState = PlayerState.Stop;
        animator.SetTrigger("stop");
        rigidbody.velocity = Vector2.zero;
    }


    public void PlayerAttacked()
    {
        PlayerDie();
    }

    void PlayerDie()
    {
        playerState = PlayerState.DIe;
        GameManager.getInstance().StageRestart();
    }

    public PlayerState GetPlayerState()
    {
        return playerState;    
    }
}
