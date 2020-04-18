using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    public bool isHoldableObject;

    private Rigidbody2D rb;
    public Player player;
    public bool instantiated;
    public bool needSync;
    public bool triggerEntered;
    public ObjectSyncController objectSyncController;
    public InteractableObject[] childObjectPair = new InteractableObject[6];
    public InteractableObject parentObject;

    public int CutNum => this.player.GetPlayerCutNumber();
    public int CurrentCutNum => this.player.GetCurrentCutNumber();
    public bool IsInstantiated => this.instantiated;
    public bool NeedSync => this.needSync;

    public bool stay;
    public bool exit;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
    }

    // 물건 들어올리기
    public void Hold()
    {
        if (isHoldableObject)
        {
            //this.player.isHoldingObject = true;
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
            if (player.transform.GetComponent<SpriteRenderer>().flipX)
            {
                this.transform.localPosition = -player.releasePosition;
            }
            else
            {
                this.transform.localPosition = player.releasePosition;
            }
            this.rb.velocity = Vector2.zero;
            this.transform.parent = player.transform.parent;
            this.rb.simulated = true;

            player = null;
        }
    }

    // 물건 밀기
    // 여기서 sync 할 물건 찾아서 같이 이동시켜주기

    public void Drag(float axis, float speed)
    {
        if (axis > 0 && this.transform.position.x > player.transform.position.x
            || axis < 0 && this.transform.position.x < player.transform.position.x)
        {
            rb.velocity = new Vector2(speed * axis, rb.velocity.y);
            Debug.Log("drag");

            if (this.needSync/*현재 컷인지도 같이 체크*/)
            {
                //objectSyncController.SyncObject(rb.velocity);
            }
        }
    }

    // 물건 든 상태에서 Flip
    public void Flip(bool flip)
    {
        this.GetComponent<SpriteRenderer>().flipX = flip;
    }

    public void Instantiated(bool flag)
    {
        this.instantiated = flag;
    }

    public void SyncNeeded(bool flag)
    {
        this.needSync = flag;
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
            this.rb.velocity = Vector2.zero;
            this.needSync = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // player = collision.gameObject.GetComponent<Player>();
            // 플레이어와 경계 트리거 동시 접촉
            if (this.needSync)
            {
                this.objectSyncController.SyncObject(childObjectPair);
            }
        }
    }
}
