using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
    private Vector2 overlapSize;
    public Player player;
    public bool instantiatedForDrag;
    public bool needSync;
    public bool stayUpperCollider;
    public ObjectSyncController objectSyncController;
    public InteractableObject[] childObjectPair = new InteractableObject[6];
    public InteractableObject parentObject;


    public int CutNum => this.player.GetPlayerCutNumber();
    public int CurrentCutNum => this.player.GetCurrentCutNumber();
    public bool IsInstantiated => this.instantiatedForDrag;
    public bool NeedSync => this.needSync;

    public bool stay;
    public bool exit;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        sfx = this.GetComponent<ObjectSoundController>();
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
        overlapSize = new Vector2(2.253715f, 0.01f);
    }

    private void Update()
    {
        isGround = GroundCheck();
        
        if(!(rb.velocity.y > 0 || rb.transform.localPosition.y < -4.5) && !this.stayUpperCollider)
        {
            this.gameObject.layer = 8;
        }

        if (this.transform.localPosition.y < -7 || this.transform.localPosition.y > 6)
        {
            Destroy(this.gameObject);
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

        RaycastHit2D hitRight = Physics2D.Raycast(calRayRight, Vector2.down, 0.1f);
        RaycastHit2D hitLeft = Physics2D.Raycast(calRayLeft, Vector2.down, 0.1f);
        RaycastHit2D hitMiddle = Physics2D.Raycast(calRayMiddle, Vector2.down, 0.1f);

        bool isHitRight;
        bool isHitLeft = false;
        bool isHitMiddle = false;

        
        if (GroundColliderCheck(hitRight, out isHitRight) || GroundColliderCheck(hitLeft, out isHitLeft) ||
            GroundColliderCheck(hitMiddle, out isHitMiddle))
        {
            if (isHitRight || isHitLeft || isHitMiddle)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
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

    public void Drag(float axis, float speed)
    {
        if (!isGround)
        {
            return;
        }

        rb.velocity = new Vector2(speed * axis, rb.velocity.y);

        if (this.CutNum == this.CurrentCutNum)
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

    public void Instantiated(bool flag)
    {
        this.instantiatedForDrag = flag;
    }

    public void SyncNeeded(bool flag)
    {
        this.needSync = flag;
    }

    public void ChangeVelocity(Vector2 vel)
    {
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
        if (collision.gameObject.CompareTag("Player"))
        {
            //if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
            //    rb.velocity = Vector2.zero;
            player = collision.gameObject.GetComponent<Player>();
        }

        if (collision.gameObject.CompareTag("BoundaryCollider"))
        {
            if (player.GetCurrentCutNumber() != player.GetPlayerCutNumber())
            {
                return;
            }

            // 현재 벡터값 저장, 위에 스폰, 다른데 생성 (던지기)
            if (!collision.gameObject.GetComponent<BoundaryCollider>().verticalBoundary)
            {
                objectSyncController.Thrown(player.GetCurrentCutNumber(),
                    this.gameObject, rb.velocity);
            }
            // 밀기
            else
            {
                if (!this.instantiatedForDrag)
                {
                    objectSyncController.InstantiateObjects(player.GetCurrentCutNumber(),
                        this.gameObject, rb.velocity);
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