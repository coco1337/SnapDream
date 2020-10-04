using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ObjectId))]
public abstract class CInteractableObject : MonoBehaviour
{
	public enum HitBoundaryLocation
	{
		NONE = 0,
		ELEFT_BOUNDARY = 1,
		ERIGHT_BOUNDARY = 2,
		ECEIL_BOUNDARY = 3,
		EFLOOR_BOUNDARY = 4,
		MAX = EFLOOR_BOUNDARY + 1,
	}

	[SerializeField] protected bool isGround;

	[Header("Physics")] 
	[SerializeField] protected Vector2 movingDirection;
	[SerializeField] protected float gravityScale;
	[SerializeField] protected float boxCastThickness;
	[SerializeField] protected List<Collider2D> hitColliders;
	[SerializeField] protected float padding;
	[SerializeField] protected float rayMaxDistance;
	[SerializeField] protected float groundOffset;
	[SerializeField] protected float sideOffset;
	[SerializeField] protected float upOffset;
	[SerializeField] protected BoxCollider2D colliderSelf;
	private RaycastHit2D[] groundColliders;

	[Header("Sync")] 
	[SerializeField] protected ObjectId objectId;
	[SerializeField] protected int whichCutNum;
	[SerializeField] protected bool synced = true;
	[SerializeField] protected Vector2 activationPosition;

	private Vector2 TopBottomBoundSize => new Vector2(colliderSelf.size.x, boxCastThickness);
	private Vector2 LeftRightBoundSize => new Vector2(boxCastThickness, colliderSelf.size.y);

	private Vector2 BoxCastRightOrigin =>
		new Vector2(transform.position.x + (colliderSelf.size.x + boxCastThickness) / 2 + padding - sideOffset,
			transform.position.y + colliderSelf.offset.y);

	private Vector2 BoxCastLeftOrigin =>
		new Vector2(transform.position.x - (colliderSelf.size.x + boxCastThickness) / 2 - padding + sideOffset,
			transform.position.y + colliderSelf.offset.y);

	private Vector2 BoxCastUpOrigin =>
		new Vector2(transform.position.x,
			transform.position.y + (colliderSelf.size.y + boxCastThickness) / 2 + colliderSelf.offset.y + padding -
			upOffset);

	private Vector2 BoxCastDownOrigin =>
		new Vector2(transform.position.x,
			transform.position.y - (colliderSelf.size.y + boxCastThickness) / 2 + colliderSelf.offset.y - padding +
			groundOffset);

	public int WhichCutNum => whichCutNum;
	public Vector2 MovingDirection => movingDirection;
	public int GetId => objectId.GetId;
	public bool IsSynced => synced;
	public void SetActivationPosition(Vector2 v) => activationPosition = v;

	public virtual void Init(int cutNum, Vector2 moveDir = new Vector2())
	{
		// TODO : 여기서 싱크 이후의 MoveDirection을 받아야함
		hitColliders = new List<Collider2D>();
		objectId = this.GetComponent<ObjectId>();
		objectId.BindObjectId(this);
		whichCutNum = cutNum;
		movingDirection = moveDir;
	}

	/// <summary>
	/// 상하좌우 각각 boxcast hit 체크, 충돌하는게 있는지 체크
	/// </summary>
	protected void HitColliderCheck()
	{
		hitColliders.AddRange(Physics2D.OverlapBoxAll(BoxCastUpOrigin, TopBottomBoundSize, 0));
		hitColliders.AddRange(Physics2D.OverlapBoxAll(BoxCastDownOrigin, TopBottomBoundSize, 0));
		hitColliders.AddRange(Physics2D.OverlapBoxAll(BoxCastLeftOrigin, LeftRightBoundSize, 0));
		hitColliders.AddRange(Physics2D.OverlapBoxAll(BoxCastRightOrigin, LeftRightBoundSize, 0));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;

		var temp = Physics2D.BoxCastAll(BoxCastDownOrigin, TopBottomBoundSize,
			0, Vector2.down, rayMaxDistance /*, 1 << LayerMask.NameToLayer("Ground")*/);
		if (temp.Length > 0)
		{
			foreach (var t in temp)
			{
				Gizmos.DrawRay(BoxCastDownOrigin, Vector3.down * t.distance);
				Gizmos.DrawWireCube(BoxCastDownOrigin + Vector2.down * t.distance, TopBottomBoundSize);
			}
		}
		else
		{
			Gizmos.DrawRay(BoxCastDownOrigin, Vector3.down * rayMaxDistance);
		}

		Gizmos.DrawRay(BoxCastDownOrigin, Vector2.down * rayMaxDistance);

		Gizmos.color = Color.cyan;

		var temp2 = Physics2D.BoxCastAll(BoxCastUpOrigin, TopBottomBoundSize, 0, Vector2.up, rayMaxDistance);
		if (temp2.Length > 0)
		{
			foreach (var t in temp2)
			{
				Gizmos.DrawRay(BoxCastUpOrigin, Vector2.up * t.distance);
				Gizmos.DrawWireCube(BoxCastUpOrigin + Vector2.up * t.distance, TopBottomBoundSize);
			}
		}
		else
		{
			Gizmos.DrawRay(BoxCastUpOrigin, Vector2.up * rayMaxDistance);
		}

		Gizmos.DrawRay(BoxCastUpOrigin, Vector2.up * rayMaxDistance);

		Gizmos.color = Color.red;

		var temp3 = Physics2D.BoxCastAll(BoxCastRightOrigin, LeftRightBoundSize, 0, Vector2.right, rayMaxDistance);
		if (temp3.Length > 0)
		{
			foreach (var t in temp3)
			{
				Gizmos.DrawRay(BoxCastRightOrigin, Vector2.right * t.distance);
				Gizmos.DrawWireCube(BoxCastRightOrigin + Vector2.right * t.distance, LeftRightBoundSize);
			}
		}
		else
		{
			Gizmos.DrawRay(BoxCastRightOrigin, Vector2.right * rayMaxDistance);
		}

		Gizmos.color = Color.black;
		var temp4 = Physics2D.BoxCastAll(BoxCastLeftOrigin, LeftRightBoundSize, 0, Vector2.left, rayMaxDistance);
		if (temp4.Length > 0)
		{
			foreach (var t in temp4)
			{
				Gizmos.DrawRay(BoxCastLeftOrigin, Vector2.left * t.distance);
				Gizmos.DrawWireCube(BoxCastLeftOrigin + Vector2.left * t.distance, LeftRightBoundSize);
			}
		}
		else
		{
			Gizmos.DrawRay(BoxCastLeftOrigin, Vector2.left * rayMaxDistance);
		}
	}

	/// <summary>
	/// Ground Check
	/// </summary>
	/// <returns>true : ground, false : midair</returns>
	protected bool GroundCheck()
	{
		groundColliders = Physics2D.BoxCastAll(BoxCastDownOrigin, TopBottomBoundSize,
			0, Vector2.down, rayMaxDistance, 1 << LayerMask.NameToLayer("Ground"));

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
			// 래더 위로 올라가는경우 제외
			if (t.collider.CompareTag("Ladder") || t.collider.CompareTag("Ladder Exit"))
				continue;

			if (t.distance != 0)
			{
				if (minDist > t.distance)
					minDist = t.distance;
			}
		}

		return minDist;
	}

	protected bool IsHitRight() => CheckLadderAndSync(Physics2D.BoxCastAll(BoxCastRightOrigin, LeftRightBoundSize, 0,
			Vector2.right, rayMaxDistance,
			(1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Boundary"))),
		HitBoundaryLocation.ERIGHT_BOUNDARY);

	protected bool IsHitLeft() => CheckLadderAndSync(Physics2D.BoxCastAll(BoxCastLeftOrigin, LeftRightBoundSize, 0,
			Vector2.left, rayMaxDistance,
			(1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Boundary"))),
		HitBoundaryLocation.ELEFT_BOUNDARY);

	protected bool IsHitUp(out bool needSync) => CheckUpperBoundaryAndSync(Physics2D.BoxCastAll(BoxCastUpOrigin, TopBottomBoundSize, 0,
		Vector2.up, rayMaxDistance, 1 << LayerMask.NameToLayer("Boundary")), HitBoundaryLocation.ECEIL_BOUNDARY, out needSync);

	private bool CheckLadderAndSync(RaycastHit2D[] hits, HitBoundaryLocation loc)
	{
		if (hits.Length > 1)
		{
			// 사다리때문에 안 밀리는경우가 있었음
			// colliders[0]는 자기 자신
			foreach (var i in hits)
			{
				// 자기 자신은 넘어가고
				if (i.transform == this.gameObject.transform)
					continue;
				else if (i.collider.CompareTag("Ladder") || i.collider.CompareTag("Ladder Exit"))	// 래더도 무시
					continue;
				else if (i.transform.CompareTag("Ground"))	// 벽 박으면 true반환
					return true;
				else if (i.collider.CompareTag("BoundaryCollider"))	// 카메라 경계 콜라이더
				{
					bool dir = false;

					if (loc == HitBoundaryLocation.ELEFT_BOUNDARY)
					{
						dir = true;
					}
					else if (loc == HitBoundaryLocation.ERIGHT_BOUNDARY)
					{
						dir = false;
					}
					
					// 일단 좌우부터 구현
					StageManager.GetInstance().GetCutManager.GetObjectSyncController.SyncOtherObjects(objectId.GetId, loc, dir);
					return true;
				}
			}
		}

		return false;
	}


	private bool CheckUpperBoundaryAndSync(RaycastHit2D[] hits, HitBoundaryLocation loc, out bool needSync)
	{
		needSync = false;
		// TODO : 위에 닿았을 때 구현
		foreach (var hit in hits)
		{
			if (hit.collider.CompareTag("BoundaryCollider"))
			{
				if (!(StageManager.GetInstance().GetCurrentCutNum() < StageManager.GetInstance().GetCutManager.MaxCutCount / 2))
				{
					StageManager.GetInstance().GetCutManager.GetObjectSyncController.SyncOtherObjects(objectId.GetId, loc, true);
					needSync = true;
					return true;
				}
				else
				{
					needSync = false;
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 중력 계산해서 방향 적용
	/// Update()에서 사용
	/// </summary>
	protected void TranslateWithGravity()
	{
		if (!isGround)
		{
			movingDirection.y += Physics2D.gravity.y * Time.fixedDeltaTime * gravityScale;
		}
		else
		{
			movingDirection.y = 0;
		}

		transform.Translate(movingDirection);
	}

	public void DisconnectSync()
	{
		this.synced = false;
	}

	public void AfterSync(float time)
	{
		
	}

	public void Activate()
	{
		if (!(activationPosition.x == 0 && activationPosition.y == 0))
		{
			activationPosition = Vector2.zero;
			var currentCut = StageManager.GetInstance().GetCurrentCutNum();

			// TODO: 카메라기준으로 위치 다시 잡아주기
			
		}
	}
}