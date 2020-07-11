using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InteractableObject : CInteractableObject
{
    [Header("Toybox")]
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    [SerializeField] private bool isHoldableObject;
    // private Rigidbody2D rb;
    private ObjectSoundController sfx;
    private GameManager gameManager;
    private int whichCutNum;
    private bool movingXMidAir;
    
    // public Player player;
    [Header("Checking - will be private")]
    public bool instantiatedForDrag;
    public bool needSync;
    public bool stayUpperCollider;
    public ObjectSyncController objectSyncController;
    public InteractableObject[] childObjectPair = new InteractableObject[6];
    public InteractableObject parentObject;

    // private void SetRigidbodyFreezePositionX() => 
    //    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

    [Header("Drag")] 
    [SerializeField] private float dragWeight;
    [SerializeField] private bool hitSideWall;

    [Header("Throw")] 
    [SerializeField] private float throwWeight;
    
    public bool IsInstantiated => this.instantiatedForDrag;
    public void Instantiated(bool flag) => this.instantiatedForDrag = flag;
    public void SyncNeeded(bool flag) => this.needSync = flag;
    public int WhichCutNum => whichCutNum;

    public void Init(int cutNum)
    {
        whichCutNum = cutNum;
        base.Init();
    }
    
    private void Start()
    {
        // rb = this.GetComponent<Rigidbody2D>();
        sfx = this.GetComponent<ObjectSoundController>();
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
        gameManager = GameManager.GetInstance();
    }

    private void FixedUpdate()
    {
        if (moveDirection.y > 0)
        {
            isGround = false;

            if (gameManager.GetCurrentCutNum() < 3)
            {
                // TODO : 나중에 CutManager 추가한 뒤 수정하기 (전체 컷의 반), 일단 하드코딩으로 구현
                if (transform.localPosition.y > 5.5)
                {
                    moveDirection = new Vector2(moveDirection.x, 0);
                }
            }
        }
        else
        {
            if (base.GroundCheck())
            {
                // ground
                if (!isGround)
                {
                    // 이전 프레임에서 midair 였을 때
                    moveDirection = new Vector2(0, moveDirection.y);
                }
                isGround = true;
            }
            else
            {
                // midair
                isGround = false;
            }

            if (this.transform.localPosition.y < -7)
            {
                Destroy(this.gameObject);
            }
        }

        base.TranslateWithGravity();
    }

    public bool Drag(float axis, float speed)
    {
        if (!isGround)
        {
            return false;
        }

        if (axis == 0)
        {
            moveDirection = new Vector2(0, moveDirection.y);
        }
        else
        {
            // 좌우에 벽 붙어 있으면 못 밀게
            if (base.IsHitLeft() || base.IsHitRight())
            {
                hitSideWall = true;
                moveDirection = new Vector2(0, moveDirection.y);
                return false;
            }
            else
            {
                hitSideWall = false;
                moveDirection = new Vector2(speed * axis / dragWeight, moveDirection.y);
            }
        }
        
        if (whichCutNum == gameManager.GetCurrentCutNum())
        {
            if (axis != 0)
                sfx.TurnOnSound();
            else
                sfx.TurnOffSound();

            if (this.needSync)
            {
                // objectSyncController.SyncObject(this.childObjectPair, rb.velocity);
            }
        }

        return true;
    }

    public bool Throw(float throwPower)
    {
        if (!isGround)
        {
            return false;
        }
        
        moveDirection = new Vector2(0, throwPower / throwWeight);
        return true;
    }

    public void ChangeVelocity(Vector2 vel)
    {
        movingXMidAir = true;
       // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // this.rb.velocity = vel;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
                // rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.gameObject.CompareTag("Player"))
        {
            //if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
            //    rb.velocity = Vector2.zero;
            player = collision.gameObject.GetComponent<Player>();
        }
        */

        if (collision.gameObject.CompareTag("BoundaryCollider"))
        {
            if (whichCutNum != gameManager.GetCurrentCutNum())
            {
                return;
            }

            // 현재 벡터값 저장, 위에 스폰, 다른데 생성 (던지기)
            if (!collision.gameObject.GetComponent<BoundaryCollider>().verticalBoundary)
            {
                // objectSyncController.Thrown(this.gameObject, rb.velocity);
            }
            // 밀기
            else
            {
                if (!this.instantiatedForDrag)
                {
                    // objectSyncController.InstantiateObjects(this.gameObject, rb.velocity);
                    this.instantiatedForDrag = true;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BoundaryCollider"))
        {
            this.stayUpperCollider = true;
        }


        if (collision.transform.CompareTag("Player"))
        {
           // if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
                // rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.stayUpperCollider)
        {
            this.stayUpperCollider = false;
        }

        this.sfx.TurnOffSound();
    }
}