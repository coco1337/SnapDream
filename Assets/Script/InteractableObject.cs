using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    [SerializeField] private bool isHoldableObject;
    [SerializeField] private bool isGround;
    [SerializeField] private Vector3 rayPosition;
    private Vector3 calculatedRayPosRight;
    private Vector3 calculatedRayPosLeft;
    private Vector2 calRayRight;
    private Vector2 calRayLeft;
    private Vector2 calRayMiddle;
    private Rigidbody2D rb;
    private ObjectSoundController sfx;
    private GameManager gameManager;
    private int whichCutNum;
    private bool movingXMidAir;
    
    // public Player player;
    public bool instantiatedForDrag;
    public bool needSync;
    public bool stayUpperCollider;
    public ObjectSyncController objectSyncController;
    public InteractableObject[] childObjectPair = new InteractableObject[6];
    public InteractableObject parentObject;

    public bool IsInstantiated => this.instantiatedForDrag;

    public bool stay;
    public bool exit;
    
    private void SetRigidbodyFreezePositionX() => 
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    
    
    public void Instantiated(bool flag) => this.instantiatedForDrag = flag;
    public void SyncNeeded(bool flag) => this.needSync = flag;
    public int WhichCutNum => whichCutNum;

    public void Init(int cutNum)
    {
        whichCutNum = cutNum;
    }
    
    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        sfx = this.GetComponent<ObjectSoundController>();
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
        gameManager = GameManager.getInstance();
    }

    private void Update()
    {
        isGround = GroundCheck();
        
        if(!(rb.velocity.y > 0 || rb.transform.localPosition.y < -4.5) && !this.stayUpperCollider)
        {
            this.gameObject.layer = 8;
        }
        
        if (this.transform.localPosition.y < -7)
        {
            Destroy(this.gameObject);
        }

        // 콜라이더 충돌 체크 등 다른 방법 필요
        if (gameManager.GetCurrentCutNum() < 3)
        {
            if (this.transform.localPosition.y > 5.5)
            {
                this.transform.localPosition =
                    new Vector3(this.transform.localPosition.x, 5.5f, this.transform.localPosition.z);
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            if (this.transform.localPosition.y > 6)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private bool GroundCheck()
    {
        /* // debug
        calculatedRayPosRight = new Vector3(transform.position.x + rayPosition.x, 
            transform.position.y + rayPosition.y, transform.position.z + rayPosition.z);
        calculatedRayPosLeft = new Vector3(transform.position.x - rayPosition.x, 
            transform.position.y + rayPosition.y, transform.position.z + rayPosition.z);
        Debug.DrawRay(calculatedRayPosRight, Vector3.down * 0.1f, Color.cyan, 1);
        Debug.DrawRay(calculatedRayPosLeft, Vector3.down * 0.1f, Color.cyan, 1);
        */
        
        calRayRight = new Vector2(transform.position.x + rayPosition.x, transform.position.y + rayPosition.y);
        calRayLeft = new Vector2(transform.position.x - rayPosition.x, transform.position.y + rayPosition.y);
        calRayMiddle = new Vector2(transform.position.x, transform.position.y + rayPosition.y);

        var hitRight = Physics2D.Raycast(calRayRight, Vector2.down, 0.1f);
        var hitLeft = Physics2D.Raycast(calRayLeft, Vector2.down, 0.1f);
        var hitMiddle = Physics2D.Raycast(calRayMiddle, Vector2.down, 0.1f);

        var isHitLeft = false;
        var isHitMiddle = false;

        
        if (GroundColliderCheck(hitRight, out var isHitRight) || GroundColliderCheck(hitLeft, out isHitLeft) ||
            GroundColliderCheck(hitMiddle, out isHitMiddle))
        {
            if (isHitRight || isHitLeft || isHitMiddle)
            {
                movingXMidAir = false;
                return true;
            }
            else
            {
                if (!movingXMidAir)
                    SetRigidbodyFreezePositionX();
                return false;
            }
        }
        else
        {
            if (!movingXMidAir)
                SetRigidbodyFreezePositionX();
            return false;
        }
    }

    private bool GroundColliderCheck(RaycastHit2D hit, out bool isHitGround)
    {
        if (hit.collider == null)
        {
            isHitGround = false;
            return false;
        }

        if (hit.collider.CompareTag("Throw") || hit.collider.CompareTag("Player"))
        {
            isHitGround = false;
            return false;
        }
        
        isHitGround = hit.collider.IsTouchingLayers(1 << LayerMask.NameToLayer("Ground"));
        return true;
    }

    public bool Drag(float axis, float speed)
    {
        if (!isGround)
        {
            return false;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = new Vector2(speed * axis, rb.velocity.y);

        if (whichCutNum == gameManager.GetCurrentCutNum())
        {
            if (axis != 0)
                sfx.TurnOnSound();
            else
                sfx.TurnOffSound();

            if (this.needSync)
            {
                objectSyncController.SyncObject(this.childObjectPair, rb.velocity);
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
        
        rb.velocity = new Vector2(0, throwPower);
        this.gameObject.layer = 31;
        return true;
    }

    public void ChangeVelocity(Vector2 vel)
    {
        movingXMidAir = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        this.rb.velocity = vel;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
                rb.velocity = Vector2.zero;
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
                objectSyncController.Thrown(this.gameObject, rb.velocity);
            }
            // 밀기
            else
            {
                if (!this.instantiatedForDrag)
                {
                    objectSyncController.InstantiateObjects(this.gameObject, rb.velocity);
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
            if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
                rb.velocity = Vector2.zero;
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