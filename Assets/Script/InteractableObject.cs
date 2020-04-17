using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    public bool isHoldableObject;

    private Rigidbody2D rb;
    private Player player;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        if (this.gameObject.tag != "InteractableObject")
        {
            this.gameObject.tag = "InteractableObject";
        }
    }

    // 물건 들어올리기
    public void Hold()
    {
        if (isHoldableObject)
        {
            this.player.isHoldingObject = true;
            this.transform.parent = player.transform;
            // 들었을때 holdingPosition으로 이동
            this.transform.localPosition = player.holdingPosition;
            this.rb.simulated = false;
        }
    }

    // 물건 내리기
    public void Release()
    {
        if (isHoldableObject)
        {
            this.rb.velocity = Vector2.zero;
            this.transform.localPosition = player.releasePosition;
            this.transform.parent = player.transform.parent;
            this.rb.simulated = true;
            player.isHoldingObject = false;
            player.holdingObject = null;
            player = null;
        }
    }

    // 물건 밀기
    public void Drag(float axis, float speed)
    {
        if (axis > 0 && this.transform.position.x > player.transform.position.x
            || axis < 0 && this.transform.position.x < player.transform.position.x)
        {
            rb.velocity = new Vector2(speed * axis, rb.velocity.y);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.GetComponent<Player>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("exit");
            this.rb.velocity = Vector2.zero;
        }
    }
}
