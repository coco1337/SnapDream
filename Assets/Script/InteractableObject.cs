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
    private GameManager gameManager;
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
    [SerializeField] private float toyBoxWeight;
    [SerializeField] private bool hitSideWall;

    [Header("Throw")] 
    [SerializeField] private float throwWeight;
    
    public bool IsInstantiated => this.instantiatedForDrag;
    public void Instantiated(bool flag) => this.instantiatedForDrag = flag;
    public void SyncNeeded(bool flag) => this.needSync = flag;

    /*
    public override void Init(int cutNum)
    {
        base.Init(cutNum);
    }
    */
    
    private void Start()
    {
        // rb = this.GetComponent<Rigidbody2D>();
        // objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
        gameManager = GameManager.GetInstance();
    }

    private void Update()
    {
        // TODO : 던질때를 알수있는 필터같은게 필요함 
        
        if (movingDirection.y > 0)
        {
            isGround = false;

            if (base.IsHitUp(out var needSync))
            {
                // 위에 닿았을 때
                if (needSync)
                {
                    
                }
                else
                {
                    movingDirection = new Vector2(movingDirection.x, 0);
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
                    movingDirection = new Vector2(0, movingDirection.y);
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
            movingDirection = new Vector2(0, movingDirection.y);
        }
        else
        {
            // 좌우에 벽 붙어 있으면 못 밀게
            if (base.IsHitLeft() || base.IsHitRight())
            {
                hitSideWall = true;
                movingDirection = new Vector2(0, movingDirection.y);
                return false;
            }
            else
            {
                hitSideWall = false;
                movingDirection = new Vector2(speed * axis / toyBoxWeight, movingDirection.y);
            }
        }
        
        if (whichCutNum == gameManager.GetCurrentCutNum())
        {
            if (axis != 0)
                gameManager.GetAudioManager.PlaySfx(AudioManager.SfxType.EDRAG);
            else
                gameManager.GetAudioManager.PlaySfx(AudioManager.SfxType.EEND_DRAG);

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
        
        movingDirection = new Vector2(0, throwPower / throwWeight);
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

        gameManager.GetAudioManager.PlaySfx(AudioManager.SfxType.EEND_DRAG);
    }
}