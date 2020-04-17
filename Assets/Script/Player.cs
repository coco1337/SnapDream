using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody2D RB; 
    public float speed = 4;
    public float jumpPower = 5;
    int cutNum;
    public bool isGround;

    enum PlayerState
    {
        Idle, Move, Jump, Interaction, DIe
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

    }

    public void PlayerJump()
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(Vector2.up * jumpPower);
    }
}
