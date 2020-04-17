using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 기본적으로 밀기는 가능, 들수 있는지만 확인
    // InteractableObject 태그 달기
    public bool isHoldableObject;

    private Rigidbody2D rb;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        if (this.gameObject.tag != "InteractableObject")
        {
            this.gameObject.tag = "InteractableObject";
        }
    }

    // 물건 들어올리기
    public void Hold(Player player)
    {
        if (isHoldableObject)
        {
            player.isHoldingObject = true;
            this.transform.parent = player.transform;
            // 들었을때 holdingPosition으로 이동
            this.transform.localPosition = player.holdingPosition;
            this.rb.simulated = false;
        }
    }

    // 물건 내리기
    public void Release(Player player)
    {
        if (isHoldableObject)
        {
            Debug.Log("here");
            player.isHoldingObject = false;
            this.transform.parent = player.transform.parent;
            this.rb.simulated = true;
        }
    }

    // 물건 밀기
    public void Drag(float axis, float speed)
    {
        rb.velocity = new Vector2(speed * axis, rb.velocity.y);
    }
}
