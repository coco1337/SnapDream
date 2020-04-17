using System.Collections;
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

    public bool isHoldingObject;
    private bool canInteractable;
    private bool isHoldable;
    private GameObject interactableObject;
    [SerializeField]
    private Vector2 holdingPosition;

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

        if (canInteractable && interactableObject != null)
        {
            this.DragObject(axis);
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

    // 물건 들어올리기
    public void HoldObject()
    {
        // 물건을 들 때
        if (!isHoldingObject)
        {
            if (canInteractable && interactableObject != null)
            {
                interactableObject.transform.parent = this.transform;
                // 들었을때 holdingPosition으로 이동
                interactableObject.transform.localPosition = holdingPosition;
            }
        }
        // 물건을 내려 놓을 때
        else
        {
            isHoldingObject = false;
            // TODO : 내려놓는 동작
        }
    }

    // 물건 밀기
    private void DragObject(float axis)
    {
        interactableObject.transform.GetComponent<Rigidbody2D>().velocity
            = new Vector2(speed * axis, RB.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "InteractableObject")
        {
            canInteractable = true;
            interactableObject = collision.gameObject;
            isHoldable = collision.gameObject.GetComponent<InteractableObject>().isHoldableObject;
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
