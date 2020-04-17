﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody2D RB; 
    public float speed = 4;
    public float jumpPower = 5;
    [SerializeField] int playerCutNum;
    int curretnCutNum = 0;
    public bool isGround;
    public Vector2 holdingPosition;
    public bool isHoldingObject;
    public bool canInteractable;
    // public bool isHoldable;

    private InteractableObject interactableObject;
    public GameObject HoldingObject;

    enum PlayerState
    {
        Idle, Move, Jump, Interaction, DIe
    }

    private void Start()
    {
        curretnCutNum = 0;
    }


    void Update()
    {
        //if (axis != 0)
        //{
        //    AN.SetBool("walk", true);
        //}
        //else AN.SetBool("walk", false);

        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
        //AN.SetBool("jump", !isGround);
    }

    public void PlayerMove(float axis)
    {
        RB.velocity = new Vector2(speed * axis, RB.velocity.y);

        // 물건 옮기기
        if (canInteractable && interactableObject != null)
        {
            interactableObject.Drag(axis, speed);
        }
    }

    public void PlayerJump()
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(Vector2.up * jumpPower);
    }

    public void moveNextCut()
    {
        curretnCutNum++;
    }

    public bool isMovable()
    {
        return curretnCutNum <= playerCutNum;
    }

    public void InteractObject()
    {
        if (!canInteractable && HoldingObject == null)
        {
            return;
        }

        if (!this.isHoldingObject)
        {
            interactableObject.Hold(this);
            this.HoldingObject = interactableObject.gameObject;
        }
        else
        {
            this.HoldingObject.GetComponent<InteractableObject>().Release(this);
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
}
