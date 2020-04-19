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
    public bool instantiatedForDrag;
    public bool needSync;
    public bool triggerEntered;
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
        objectSyncController = GameObject.Find("ObjectSyncManager").GetComponent<ObjectSyncController>();
    }

    private void Update()
    {
        if(!(rb.velocity.y > 0 || rb.transform.localPosition.y < -4.5) && !this.stayUpperCollider)
        {
            this.gameObject.layer = 8;
        }

        if (this.transform.localPosition.y < -7 || this.transform.localPosition.y > 6)
        {
            Destroy(this.gameObject);
        }
    }

    // 물건 밀기
    // 여기서 sync 할 물건 찾아서 같이 이동시켜주기

    public void Drag(float axis, float speed)
    {
        rb.velocity = new Vector2(speed * axis, rb.velocity.y);

        if (this.needSync && (this.CutNum == this.CurrentCutNum))
        {
            objectSyncController.SyncObject(this.childObjectPair, rb.velocity);
        }
    }

    public void Throw(float throwPower)
    {
        rb.velocity = new Vector2(0, throwPower);
        this.gameObject.layer = 31;
    }

    public void Instantiated(bool flag)
    {
        this.instantiatedForDrag = flag;
    }

    public void SyncNeeded(bool flag)
    {
        this.needSync = flag;
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
        if (collision.gameObject.tag == "Player")
        {
            if (collision.transform.GetComponent<Player>().GetPlayerState() == Player.PlayerState.Jump)
                rb.velocity = Vector2.zero;
            player = collision.gameObject.GetComponent<Player>();
        }

        if (collision.gameObject.CompareTag("BoundaryCollider"))
        {
            if (!(player.GetCurrentCutNumber() == player.GetPlayerCutNumber()))
            {
                return;
            }

            // 현재 벡터값 저장, 위에 스폰, 다른데 생성 (던지기)
            if (!collision.gameObject.GetComponent<BoundaryCollider>().verticalBoundary)
            {
                Debug.Log("던지기");
                objectSyncController.Thrown(player.GetCurrentCutNumber(),
                    this.gameObject, rb.velocity);
            }
            // 밀기
            else
            {
                Debug.Log("밀기");
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
    }
}