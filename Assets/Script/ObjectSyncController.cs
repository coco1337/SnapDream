using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectSyncController : MonoBehaviour
{
	[SerializeField] private GameObject[] eachCut;
	[SerializeField] private CutManager cutManager;
	[SerializeField] private GameManager gameManager;

	private int objectIdCounter = 0;

	private delegate bool SyncAction(int objId);
	private SyncAction[] syncActions = new SyncAction[(int)CInteractableObject.HitBoundaryLocation.MAX];

	private Dictionary<int, List<CInteractableObject>> objectDictionary =
		new Dictionary<int, List<CInteractableObject>>();

	public float spawnYPos;
	public float cameraLength;

	public int GetObjectId() => objectIdCounter++;
	public void SetCutManager(CutManager c) => cutManager = c;

	public bool SyncOtherObjects(int id, CInteractableObject.HitBoundaryLocation loc) => syncActions[(int) loc](id);

	private void Start()
	{
		gameManager = GameManager.GetInstance();
		objectIdCounter = 0;
		AddDelegates();

		// TODO : Init 함수에서 받아오는게 나음. 일단 임시
		StartCoroutine(GetGameManager());
	}

	private void AddDelegates()
	{
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ELEFT_BOUNDARY] = HitLeftBoundary;
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ERIGHT_BOUNDARY] = HitRightBoundary;
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ECEIL_BOUNDARY] = HitCeilBoundary;
	}

	private IEnumerator GetGameManager()
	{
		yield return new WaitForFixedUpdate();
		if (gameManager == null)
		{
			gameManager = GameManager.GetInstance();
		}
	}

	public void AddObject(int id, CInteractableObject interactableObj)
	{
		// 해당 아이디에 리스트 없으면 생성
		if (!objectDictionary.ContainsKey(id))
		{
			objectDictionary.Add(id, new List<CInteractableObject>());
		}

		// Dictionary에서 주어진 id에 해당하는 리스트 뽑아와서 처리
		if (objectDictionary.TryGetValue(id, out var objs))
		{
			objs.Add(interactableObj);
		}
		else
		{
			Debug.LogError("Add interactable Object error");
		}
	}

	// 왼쪽 충돌이면 이전 컷부터 변화
	// 오른쪽 충돌이면 이후 컷부터 변화 - 없음
	
	// TODO : 삭제 예정
	public void InstantiateObjects(GameObject obj, Vector2 vel)
	{
		var interactableObj = obj.GetComponent<InteractableObject>();
		// 오른쪽 충돌시
		if (obj.transform.localPosition.x > 0)
		{
		}
		// 왼쪽 충돌시
		else
		{
			// 제일 왼쪽 컷들은 물건 왼쪽으로 넘길수 없음
			if (gameManager.GetCurrentCutNum() == 0 || gameManager.GetCurrentCutNum() == cutManager.MaxCutCount / 2)
			{
				return;
			}

			// 현재 카메라 x좌표
			var cameraX = eachCut[gameManager.GetCurrentCutNum()].transform.Find("Camera(Clone)").transform
				.localPosition.x;

			// 생성될 컷의 카메라 x 좌표(바로 직전 컷의 카메라 좌표)
			var targetCameraX = eachCut[gameManager.GetCurrentCutNum() - 1].transform.Find("Camera(Clone)").transform
				.localPosition.x;

			// 아마도 물건 길이 + 1
			var result = targetCameraX + (cameraLength / 2) + 1;

			InteractableObject[] childPair = new InteractableObject[6];

			// currentNum 전 칸 부터 스폰
			for (var i = gameManager.GetCurrentCutNum() - 1; i < 6; ++i)
			{
				var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
				childPair[i] = spawnedObject.GetComponent<InteractableObject>();
				childPair[i].Init(i);
				spawnedObject.transform.localPosition = new Vector2(result, obj.gameObject.transform.localPosition.y);
			}

			interactableObj.childObjectPair = childPair;
			interactableObj.SyncNeeded(true);
		}
	}

	public void Thrown(GameObject obj, Vector2 vel)
	{
		if (gameManager.GetCurrentCutNum() < 3)
		{
			return;
		}

		// 현재 카메라 x좌표
		var cameraX = eachCut[gameManager.GetCurrentCutNum()].transform.Find("Camera(Clone)").transform.localPosition.x;

		// 생성될 컷의 카메라 x 좌표
		var targetCameraX = eachCut[gameManager.GetCurrentCutNum() - 3].transform.Find("Camera(Clone)").transform
			.localPosition.x;

		// 생성될 오브젝트의 좌표
		var result = (obj.transform.localPosition.x - cameraX) + targetCameraX;

		for (var i = gameManager.GetCurrentCutNum() - 3; i < 6; ++i)
		{
			var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
			spawnedObject.layer = 31;

			spawnedObject.transform.localPosition = new Vector2(result, spawnYPos);
			spawnedObject.GetComponent<Rigidbody2D>().velocity = vel;
		}
	}

	public void ExitCollider(InteractableObject obj)
	{
		if (obj.IsInstantiated)
		{
			obj.Instantiated(false);
		}
	}


	public void SyncObject(InteractableObject[] objectPair, Vector2 vel)
	{
		for (var i = 0; i < 6; ++i)
		{
			objectPair[i]?.ChangeVelocity(vel);
		}
	}

	public void MoveNextCut(int movedCutNum)
	{
		foreach (var dictionary in objectDictionary)
		{
			var val = dictionary.Value;
			for (int i = 0; i < val.Count; ++i)
			{
				if (val[i].WhichCutNum == movedCutNum)
				{
					// TODO : 다음 컷 이동, 지금은 위치만 (나중에 constraint 사용 여부 확실하게 정하기)
					val[i].transform.localPosition = val[i - 1].transform.localPosition;
				}
			}
		}
	}

	/// <summary>
	/// 카메라의 왼쪽 경계에 CInteractableObject가 충돌 판정 있으면
	/// </summary>
	/// <param name="objId">ID of CInteractableObject</param>
	/// <param name="pos">y position of hit point</param>
	/// <returns></returns>
	private bool HitLeftBoundary(int objId)
	{
		// 컷이 2개일때와 2개 초과일 때로 분류
		if (cutManager.MaxCutCount > 2)
		{
			// 제일 왼쪽 컷들은 물건을 왼쪽으로 넘길 수 없음
			if (gameManager.GetCurrentCutNum() == 0 || gameManager.GetCurrentCutNum() == cutManager.MaxCutCount / 2)
			{
				return false;
			}

			var currentCam = cutManager.GetCamera(gameManager.GetCurrentCutNum());
			
			// 싱크 필요한 오브젝트
			CInteractableObject interactedObject = default;
			if (objectDictionary.TryGetValue(objId, out var objList))
			{
				foreach (var obj in objList)
				{
					if (obj.WhichCutNum == GameManager.GetInstance().GetCurrentCutNum())
					{
						interactedObject = obj;
						break;
					}
					
					Debug.LogError("Can't find target interactable object");
					return false;
				}
			}
			else
			{
				Debug.LogError("Can't find CInteractable Object in Dictionary");
				return false;
			}
			
			// TODO:해당 ID에 맞는 오브젝트들을 싱크 필요한 위치로 이동시키거나 없으면 생성해야 됨.
			if (objectDictionary.TryGetValue(objId, out var objList2))
			{
				foreach (var obj in objList2)
				{
					if (obj.WhichCutNum >= GameManager.GetInstance().GetCurrentCutNum() - 1)
					{
						if (obj.WhichCutNum == GameManager.GetInstance().GetCurrentCutNum())
						{
							// TODO 현재 컷과 해당 ID의 오브젝트 컷이 같을때는 일단 중복되어야 됨
						}
						else
						{
							
						}
					}
				}
			}
			
			// interactedObject.MovingDirection
			// interactedObject.TranslateAfterHitBoundary();
			interactedObject.TranslateAfterHitBoundary(interactedObject.MovingDirection);
			
			// 영향을 주는 컷은 바로 전 컷 부터
			// 전컷 카메라
			var targetCam = cutManager.GetCamera(gameManager.GetCurrentCutNum() - 1);
			
			// CInteractableObject의 MoveDirection 가져와서 계산
			
		}
		else
		{
			
		}

		return true;
	}

	private bool HitRightBoundary(int objId)
	{
		return true;
	}

	private bool HitCeilBoundary(int objId)
	{
		return true;
	}
}