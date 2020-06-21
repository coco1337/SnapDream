using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInteractableObject : MonoBehaviour
{
    [SerializeField] protected bool isGround;

    [Header("Physics")] 
    [SerializeField] protected Vector2 moveDirection;
    [SerializeField] protected float gravityScale;
    [SerializeField] protected float boxCastThickness;
    [SerializeField] protected List<Collider2D> hitColliders;
    [SerializeField] protected float padding;
    [SerializeField] protected float rayMaxDistance;
    [SerializeField] protected float groundOffset;
    [SerializeField] protected float sideOffset;
    protected BoxCollider2D colliderSelf;
    private RaycastHit2D[] groundColliders;

    private Vector2 TopBottomBoundSize => new Vector2(colliderSelf.size.x, boxCastThickness);
    private Vector2 LeftRightBoundSize => new Vector2(boxCastThickness, colliderSelf.size.y);
    private Vector2 boxCastRightOrigin => 
        new Vector2(transform.position.x + (colliderSelf.size.x + boxCastThickness) / 2 + padding - sideOffset, 
            transform.position.y + colliderSelf.offset.y);
    private Vector2 boxCastLeftOrigin =>
        new Vector2(transform.position.x - (colliderSelf.size.x + boxCastThickness) / 2 - padding + sideOffset, 
            transform.position.y + colliderSelf.offset.y);
    private Vector2 boxCastUpOrigin =>
        new Vector2(transform.position.x, 
            transform.position.y + (colliderSelf.size.y + boxCastThickness) / 2 + colliderSelf.offset.y + padding);
    private Vector2 boxCastDownOrigin =>
        new Vector2(transform.position.x, 
            transform.position.y - (colliderSelf.size.y + boxCastThickness) / 2 + colliderSelf.offset.y - padding + groundOffset);
    
    protected virtual void Init()
    {
        colliderSelf = this.transform.GetComponent<BoxCollider2D>();
        hitColliders = new List<Collider2D>();
    }

    /// <summary>
    /// 상하좌우 각각 boxcast hit 체크, 충돌하는게 있는지 체크
    /// </summary>
    protected void HitColliderCheck()
    {
        hitColliders.AddRange(Physics2D.OverlapBoxAll(boxCastUpOrigin, TopBottomBoundSize, 0));
        hitColliders.AddRange(Physics2D.OverlapBoxAll(boxCastDownOrigin, TopBottomBoundSize, 0));
        hitColliders.AddRange(Physics2D.OverlapBoxAll(boxCastLeftOrigin, LeftRightBoundSize, 0));
        hitColliders.AddRange(Physics2D.OverlapBoxAll(boxCastRightOrigin, LeftRightBoundSize, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        
        var temp = Physics2D.BoxCastAll(boxCastDownOrigin, TopBottomBoundSize, 
            0, Vector2.down, rayMaxDistance,1 << LayerMask.NameToLayer("Ground"));
        if (temp.Length > 0)
        {
            foreach (var t in temp)
            {
                if (t.distance == 0)
                    continue;
                
                Gizmos.DrawRay(boxCastDownOrigin, Vector3.down * t.distance);
                Gizmos.DrawWireCube(boxCastDownOrigin + Vector2.down * t.distance, TopBottomBoundSize);
            }
        }
        else
        {
            Gizmos.DrawRay(boxCastDownOrigin, Vector3.down * rayMaxDistance);
        }
        Gizmos.DrawWireCube(boxCastUpOrigin, TopBottomBoundSize);
        // Gizmos.DrawWireCube(boxCastDownOrigin, TopBottomBoundSize);
        Gizmos.DrawWireCube(boxCastLeftOrigin, LeftRightBoundSize);
        Gizmos.DrawWireCube(boxCastRightOrigin, LeftRightBoundSize);
    }

    /// <summary>
    /// Ground Check
    /// </summary>
    /// <returns>true : ground, false : midair</returns>
    protected bool GroundCheck()
    {
        groundColliders = Physics2D.BoxCastAll(boxCastDownOrigin, TopBottomBoundSize, 
            0, Vector2.down, rayMaxDistance,1 << LayerMask.NameToLayer("Ground"));

        float minDistance = rayMaxDistance;
        if (groundColliders.Length > 1)
        {
            minDistance = GetMinDistance(groundColliders);

            this.transform.position = new Vector2(this.transform.position.x, 
                this.transform.position.y + (colliderSelf.size.y / 2 - minDistance));
            
            return true;
        }

        return false;
    }

    private float GetMinDistance(RaycastHit2D[] hit2Ds)
    {
        float minDist = rayMaxDistance;
        foreach (var t in hit2Ds)
        {
            if (t.distance != 0)
            {
                if (minDist > t.distance)
                    minDist = t.distance;
            }
        }

        return minDist;
    }

    protected bool IsHitRight()
    {
        var colliders = Physics2D.BoxCastAll(boxCastRightOrigin, LeftRightBoundSize, 0, 
            Vector2.right, rayMaxDistance, 1 << LayerMask.NameToLayer("Ground"));
        return colliders.Length > 1 ? true : false;
    }

    protected bool IsHitLeft()
    {
        var colliders = Physics2D.BoxCastAll(boxCastLeftOrigin, LeftRightBoundSize, 0,
            Vector2.left, rayMaxDistance, 1 << LayerMask.NameToLayer("Ground"));
        return colliders.Length > 1 ? true : false;
    }

    protected bool IsHitUp(out RaycastHit2D[] colliders)
    {
        colliders = Physics2D.BoxCastAll(boxCastUpOrigin, TopBottomBoundSize, 0,
            Vector2.up, rayMaxDistance);
        return colliders.Length > 0 ? true : false;
    }

    /// <summary>
    /// 중력 계산해서 방향 적용
    /// Update()에서 사용
    /// </summary>
    protected void TranslateWithGravity()
    {
        if (!isGround)
        {
            moveDirection.y += Physics.gravity.y * Time.fixedDeltaTime * gravityScale;
        }
        else
        {
            moveDirection.y = 0;
        }
        
        transform.Translate(moveDirection);
    }
}    
