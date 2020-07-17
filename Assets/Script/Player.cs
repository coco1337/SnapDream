using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, Damageabel
{
    public Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    [SerializeField] public float speed = 4;
    [SerializeField] public float dragSpeed = 4;
    [SerializeField] public float throwPower = 2;
    [SerializeField] private int playerCutNum;
    public bool isGround;
    Animator animator;
    [SerializeField] float PlayerHealth = 1f;
    [SerializeField] float throwAnimationTime = 1f;
    private PlayerInterectController playerInterectController;

    public enum PlayerState
    {
        Idle, Move, Jump, Interaction_Ladder, Interaction_Throw, Interaction_Drag, DIe, Stop, Damaged, Clear
    }
    [SerializeField] PlayerState playerState;


    public bool IsPlyerFlip => spriteRenderer.flipX;
    public void SetPlayerCutNumber(int i) => playerCutNum = i;
    public int GetPlayerCutNumber => playerCutNum;

    private void Start()
    {
        playerState = PlayerState.Idle;
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        playerInterectController = transform.GetComponent<PlayerInterectController>();
    }

    void Update()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -1.28f * transform.localScale.y), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
    }

    void OnDrawGizmosSelected()
    {

    }

    //Player 이동 처리 함수
    public void PlayerMove(float axis)
    {

        if(axis != 0) spriteRenderer.flipX = (axis < 0);

        if (IsLadder())
        {
            Debug.Log(axis);
            animator.SetFloat("ladderSpeed", Mathf.Abs(axis) + 0.2f);
            animator.speed = Mathf.Abs(axis);
            rigidbody.velocity = new Vector2(0, speed * axis);
        }
       
        else
        {
            if (playerInterectController.CanDrag())
            {
                playerState = PlayerState.Interaction_Drag;
                animator.SetBool("isGround", isGround);
                animator.SetFloat("dragSpeed", Mathf.Abs(axis));
                animator.SetFloat("moveSpeed", 0);
                rigidbody.velocity = new Vector2(dragSpeed * axis, rigidbody.velocity.y);
                playerInterectController.MoveInteractObject(axis);
            }
            else
            {
                if (isGround)
                    playerState = (axis == 0f) ? PlayerState.Idle : PlayerState.Move;
                else
                    playerState = PlayerState.Jump;
                animator.SetBool("isGround", isGround);
                animator.SetFloat("moveSpeed", Mathf.Abs(axis));
                animator.SetFloat("dragSpeed", 0);
                rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
                animator.SetFloat("palyerVerticalSpeed", rigidbody.velocity.y);
            }
        }
    }

    //Player Cut 이동 처리 함수
    public void MoveToNextCut()
    {
        GameManager.GetInstance().NextCut();
    }

    // Player 상태 확인 관련 함수들
    //이동 가능한지를 확인하는 함수
    public bool IsMovable() => (playerState != PlayerState.Stop && playerState != PlayerState.DIe && playerState != PlayerState.Interaction_Throw && playerState != PlayerState.Clear);

    public PlayerState GetPlayerState()
    {
        return playerState;    
    }

    //Cut Change 가능 여부 확인하는 함수
    public bool IsCutChangeable() => (playerState == PlayerState.Idle || playerState == PlayerState.Move);

    //Object를 던질 수 있는지 확인하는 함수
    public bool IsThrowable() => (playerInterectController.CanThrow() && (playerState == PlayerState.Idle || playerState == PlayerState.Move || playerState == PlayerState.Interaction_Drag));

    public bool IsLadder() => (playerState == PlayerState.Interaction_Ladder);
    public bool IsDraging() => (playerState == PlayerState.Interaction_Drag);

    public void StopPlayer()
    {
        playerState = PlayerState.Stop;
        animator.enabled = false;
        foreach (var collider in transform.GetComponents<BoxCollider2D>())
        {
            collider.enabled = false;
        }
        rigidbody.velocity = Vector2.zero;
        rigidbody.bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<Player>().enabled = false;
        gameObject.GetComponent<PlayerInterectController>().enabled = false;
    }

    

    public void GetLadder()
    {
        if (playerState == PlayerState.Idle || playerState == PlayerState.Move || playerState == PlayerState.Jump)
        {
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            playerState = PlayerState.Interaction_Ladder;
            transform.position = new Vector3(playerInterectController.ladderTarget.transform.position.x, transform.position.y, transform.position.z);
        }
    }

    public void RealeaseLadder()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        animator.speed = 1;
        if (playerState == PlayerState.Interaction_Ladder)
        {
            animator.SetFloat("ladderSpeed", 0);
            playerState = PlayerState.Idle;
        }
    }


    public void GetDrag()
    {
        if (playerState == PlayerState.Move)
        {
            playerState = PlayerState.Interaction_Drag;
        }
    }

    public void RealeaseDrag()
    {
        animator.SetFloat("dragSpeed", 0);
        if (playerState == PlayerState.Interaction_Drag)
        {
            playerState = PlayerState.Idle;
        }
    }

    public void GetThrow()
    {
        if (IsThrowable())
        {
            PlayerMove(0);
            animator.SetTrigger("throwObject");
            animator.SetFloat("moveSpeed", 0);
            animator.SetFloat("dragSpeed", 0);
            playerState = PlayerState.Interaction_Throw;
            StartCoroutine(EndThrowObject());
        }
    } 

    IEnumerator EndThrowObject()
    {
        yield return new WaitForSeconds(throwAnimationTime);
        if (playerState == PlayerState.Interaction_Throw)
        {
            playerState = PlayerState.Idle;
        }
    }



    //Player에게 데미지를 주는 함수
    public void Hit(float damage)
    {
        PlayerHealth -= damage;
        if(PlayerHealth <= 0)
            DieObject();
    }

    //Player에게 데미지를 주는 Object를 만났을 경우 동작
    public void DieObject()
    {
        StopPlayer();
        playerState = PlayerState.DIe;
        GameManager.GetInstance().StageRestart();
    }

    //Stage를 Clear 할 시 Player의 동작
    public void StageClear()
    {
        playerState = PlayerState.Clear;
        animator.SetBool("isGround", true);
        StartCoroutine("StageClearAction");
    }

    IEnumerable StageClearAction()
    {
        float startTime = Time.time + 5;
        while(startTime > Time.time)
        {
            ClearMove(1);
            yield return null;
        }
        Destroy(this);
    }

    void ClearMove(float axis)
    {
        animator.SetBool("isGround", isGround);
        animator.SetFloat("moveSpeed", Mathf.Abs(axis));
        rigidbody.velocity = new Vector2(speed * axis, rigidbody.velocity.y);
    }

}
