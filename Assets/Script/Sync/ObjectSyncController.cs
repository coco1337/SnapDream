using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ObjectSyncController : MonoBehaviour
{
  [Header("실제 컷의 카메카 크기, 컷의 모양, 컷의 순서가 맞는지 확인")] [SerializeField]
  private GameObject dummyCutsParent;

  [SerializeField] private GameObject[] eachCut;
  [SerializeField] private CutManager cutManager;
  [SerializeField] private float rayDistance;

  private Transform[] dummyCuts;

  private int objectIdCounter = 0;

  private delegate bool SyncAction(int objId);

  private SyncAction[] syncActions = new SyncAction[(int) CInteractableObject.HitBoundaryLocation.MAX];

  private Dictionary<int, List<CInteractableObject>> objectDictionary =
    new Dictionary<int, List<CInteractableObject>>();

  private List<CInteractableObject> effectedObjects = new List<CInteractableObject>();

  public float spawnYPos;
  public float cameraLength;

  public int GetObjectId() => objectIdCounter++;
  public void SetCutManager(CutManager c) => cutManager = c;

  public bool SyncOtherObjects(int id, CInteractableObject.HitBoundaryLocation loc) => syncActions[(int) loc](id);

  private void Start()
  {
    objectIdCounter = 0;
    dummyCuts = dummyCutsParent.GetComponentsInChildren<Transform>();
    AddDelegates();
  }

  private void AddDelegates()
  {
    syncActions[(int) CInteractableObject.HitBoundaryLocation.ELEFT_BOUNDARY] = HitLeftBoundary;
    syncActions[(int) CInteractableObject.HitBoundaryLocation.ERIGHT_BOUNDARY] = HitRightBoundary;
    syncActions[(int) CInteractableObject.HitBoundaryLocation.ECEIL_BOUNDARY] = HitCeilBoundary;
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

  public void ExitCollider(InteractableObject obj)
  {
    if (obj.IsInstantiated)
    {
      obj.Instantiated(false);
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
  /// <returns></returns>
  private bool HitLeftBoundary(int objId)
  {
    int currentCutNum = StageManager.GetInstance().GetCurrentCutNum();

    // 싱크 필요한 오브젝트 찾기, foreach 밑에랑 합쳐서 한 개로 만들 수 있을듯
    CInteractableObject interactedObject = default;
    if (objectDictionary.TryGetValue(objId, out var objList))
    {
      interactedObject = objList.FirstOrDefault(e => e.WhichCutNum == currentCutNum);
    }
    else
    {
      Debug.LogError("Can't find CInteractable Object in Dictionary");
      return false;
    }

    if (interactedObject == default)
    {
      Debug.LogError("interactable object is default");
      return false;
    }

    if (!interactedObject.IsSynced)
      return false;

    if (Physics2D.Raycast(interactedObject.transform.position, Vector2.left, rayDistance,
      1 << LayerMask.NameToLayer("Boundary")))
    {
      // TODO : 오브젝트 로컬 포지션 가져오기 - 해당 더미컷의 로컬 포지션으로 변환 -> 레이 쏘기
      var originPos = interactedObject.transform.localPosition;
      var dummyPos = originPos + dummyCuts[currentCutNum].transform.position;
    }
    // // 오브젝트 생성될 좌표 파악하기
    // var camPos = cutManager.GetCamera(interactedObject.WhichCutNum).transform.localPosition;
    // var instantiatePosY = interactedObject.transform.localPosition.y - camPos.y;
    // var previousCam = cutManager.GetCamera(interactedObject.WhichCutNum - 1).transform;
    // effectedObjects.Clear();
    //
    // // 해당 ID에 맞는 오브젝트들을 싱크 필요한 위치로 이동시키거나 없으면 생성해야 됨.
    // for (int i = 0; i < objList.Count; ++i)
    // {
    // 	if ((objList[i].WhichCutNum == currentCutNum || objList[i].WhichCutNum == currentCutNum - 1) && objList[i].IsSynced)
    // 	{
    // 		// 현재 컷과 이전 컷에 오브젝트 생성 (어차피 미래 컷은 안보임)
    // 		var instantiated = Instantiate(objList[i], objList[i].transform.parent);
    // 		
    // 		// 오브젝트 싱크 끊기
    // 		objList[i].DisconnectSync();
    //
    // 		// 싱크 교체, 위험
    // 		objList[i] = instantiated;
    // 		instantiated.Init(objList[i].WhichCutNum/*, interactedObject.MovingDirection*/);
    // 		effectedObjects.Add(instantiated);
    //
    // 		instantiated.transform.localPosition =
    // 			new Vector2(previousCam.transform.localPosition.x - cutManager.GetCameraBoundaryWidth,
    // 				previousCam.transform.localPosition.y - instantiatePosY);
    // 	}
    // 	else if (objList[i].WhichCutNum > currentCutNum)
    // 	{
    // 		// TODO : 미래컷에 영향 주는것을 보여줘야 하면 여기서
    // 	}
    // }
    //
    // // TODO : 오브젝트 삭제 전에 보여줄 행동
    // // interactedObject.TranslateAfterHitBoundary(interactedObject.MovingDirection);
    // interactedObject.DisconnectSync();
    // Destroy(interactedObject.gameObject);
    // // StartCoroutine(AfterSyncHorizontalMove(2, interactedObject));

    return true;
  }

  /// <summary>
  /// 싱크 맞춰준 후 원래 오브젝트는 잠깐동안 이동연산 하고 삭제
  /// </summary>
  /// <param name="time"></param>
  /// <returns></returns>
  private IEnumerator AfterSyncHorizontalMove(float time, CInteractableObject interactedObject)
  {
    while (time > 0)
    {
      interactedObject.transform.Translate(interactedObject.MovingDirection);
      time -= Time.deltaTime;
      yield return null;
    }

    Destroy(interactedObject.gameObject);
  }

  private bool HitRightBoundary(int objId)
  {
    return true;
  }

  /// <summary>
  /// 카메라의 위쪽 경계에 CInteractableObject가 충돌 판정 있으면
  /// </summary>
  /// <param name="objId"></param>
  /// <returns></returns>
  private bool HitCeilBoundary(int objId)
  {
    if (cutManager.MaxCutCount > 2)
    {
      CInteractableObject interactedObject = default;
      int currentCutNum = StageManager.GetInstance().GetCurrentCutNum();

      if (objectDictionary.TryGetValue(objId, out var objList))
      {
        interactedObject = objList.FirstOrDefault(e => e.WhichCutNum == currentCutNum);
      }
      else
      {
        Debug.LogError("Can't find CInteractable Object in Dictionary");
        return false;
      }

      if (interactedObject == default)
      {
        Debug.LogError("interactable object is default");
        return false;
      }

      var camPos = cutManager.GetCamera(interactedObject.WhichCutNum).transform.localPosition;
      var instantiatePosX = interactedObject.transform.localPosition.x - camPos.x;
      var aboveCam = cutManager.GetCamera(interactedObject.WhichCutNum - cutManager.MaxCutCount / 2).transform;
      // effectedObjects.Clear();

      for (int i = 0; i < objList.Count; ++i)
      {
        /*
        var a = objList[i].WhichCutNum;
        var b = currentCutNum;
        var c = GameManager.GetInstance().GetCutManager.MaxCutCount / 2;
        var d = objList[i].IsSynced;
        */
        // TODO : 3가지로 나눠서 해결, 현재컷보다 과거컷이면 추가생성 후 ID 연결, 현재 컷이면 생성후 기존오브젝트 파괴, 현재보다 미래 컷이면 이동
        if ((objList[i].WhichCutNum >= currentCutNum - cutManager.MaxCutCount / 2 &&
             objList[i].WhichCutNum < currentCutNum)
            || objList[i].WhichCutNum == currentCutNum)
        {
          var instantiated = Instantiate(objList[i], objList[i].transform.parent);

          // 오브젝트 싱크 끊기
          objList[i].DisconnectSync();

          // 리스트 항목 교체, 다른 방법 찾기
          objList[i] = instantiated;
          instantiated.Init(objList[i].WhichCutNum, interactedObject.MovingDirection);
          // effectedObjects.Add(instantiated);

          // TODO : 생성 위치 다시 잡아줘야됨
          instantiated.transform.localPosition = new Vector2(aboveCam.transform.localPosition.x + instantiatePosX,
            aboveCam.transform.localPosition.y + cutManager.GetCameraBoundaryHeight);
        }
        else if (objList[i].WhichCutNum > currentCutNum)
        {
          objList[i].Init(objList[i].WhichCutNum, interactedObject.MovingDirection);
          // 위치만 잡아주기
          objList[i].transform.localPosition = new Vector2(aboveCam.transform.localPosition.x + instantiatePosX,
            aboveCam.transform.localPosition.y + cutManager.GetCameraBoundaryHeight);
        }
      }

      // TODO : 오브젝트 삭제 전에 보여줄 행동
      interactedObject.DisconnectSync();
      Destroy(interactedObject.gameObject);
    }

    return true;
  }
}

/* backup
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ObjectSyncController : MonoBehaviour
{
	[SerializeField] private GameObject[] eachCut;
	[SerializeField] private CutManager cutManager;

	private int objectIdCounter = 0;

	private delegate bool SyncAction(int objId);
	private SyncAction[] syncActions = new SyncAction[(int)CInteractableObject.HitBoundaryLocation.MAX];

	private Dictionary<int, List<CInteractableObject>> objectDictionary =
		new Dictionary<int, List<CInteractableObject>>();
	
	private List<CInteractableObject> effectedObjects = new List<CInteractableObject>();

	public float spawnYPos;
	public float cameraLength;

	public int GetObjectId() => objectIdCounter++;
	public void SetCutManager(CutManager c) => cutManager = c;

	public bool SyncOtherObjects(int id, CInteractableObject.HitBoundaryLocation loc) => syncActions[(int) loc](id);

	private void Start()
	{
		objectIdCounter = 0;
		AddDelegates();
	}

	private void AddDelegates()
	{
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ELEFT_BOUNDARY] = HitLeftBoundary;
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ERIGHT_BOUNDARY] = HitRightBoundary;
		syncActions[(int) CInteractableObject.HitBoundaryLocation.ECEIL_BOUNDARY] = HitCeilBoundary;
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

	public void ExitCollider(InteractableObject obj)
	{
		if (obj.IsInstantiated)
		{
			obj.Instantiated(false);
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
					// 다음 컷 이동, 지금은 위치만 (나중에 constraint 사용 여부 확실하게 정하기)
					val[i].transform.localPosition = val[i - 1].transform.localPosition;
				}
			}
		}
	}

	/// <summary>
	/// 카메라의 왼쪽 경계에 CInteractableObject가 충돌 판정 있으면
	/// </summary>
	/// <param name="objId">ID of CInteractableObject</param>
	/// <returns></returns>
	private bool HitLeftBoundary(int objId)
	{
		// 컷이 2개일때와 2개 초과일 때로 분류
		if (cutManager.MaxCutCount > 2)
		{
			int currentCutNum = StageManager.GetInstance().GetCurrentCutNum();
			
			// 제일 왼쪽 컷들은 물건을 왼쪽으로 넘길 수 없음
			if (currentCutNum == 0 || currentCutNum == cutManager.MaxCutCount / 2)
			{
				return false;
			}

			// 싱크 필요한 오브젝트 찾기, foreach 밑에랑 합쳐서 한 개로 만들 수 있을듯
			CInteractableObject interactedObject = default;
			if (objectDictionary.TryGetValue(objId, out var objList))
			{
				interactedObject = objList.FirstOrDefault(e => e.WhichCutNum == currentCutNum);
			}
			else
			{
				Debug.LogError("Can't find CInteractable Object in Dictionary");
				return false;
			}

			if (interactedObject == default)
			{
				Debug.LogError("interactable object is default");
				return false;
			}

			if (!interactedObject.IsSynced)
				return false;
			
			// 오브젝트 생성될 좌표 파악하기
			var camPos = cutManager.GetCamera(interactedObject.WhichCutNum).transform.localPosition;
			var instantiatePosY = interactedObject.transform.localPosition.y - camPos.y;
			var previousCam = cutManager.GetCamera(interactedObject.WhichCutNum - 1).transform;
			effectedObjects.Clear();

			// 해당 ID에 맞는 오브젝트들을 싱크 필요한 위치로 이동시키거나 없으면 생성해야 됨.
			for (int i = 0; i < objList.Count; ++i)
			{
				if ((objList[i].WhichCutNum == currentCutNum || objList[i].WhichCutNum == currentCutNum - 1) && objList[i].IsSynced)
				{
					// 현재 컷과 이전 컷에 오브젝트 생성 (어차피 미래 컷은 안보임)
					var instantiated = Instantiate(objList[i], objList[i].transform.parent);
					
					// 오브젝트 싱크 끊기
					objList[i].DisconnectSync();

					// 싱크 교체, 위험
					objList[i] = instantiated;
					instantiated.Init(objList[i].WhichCutNum/*, interactedObject.MovingDirection*/ /*);
					effectedObjects.Add(instantiated);

					instantiated.transform.localPosition =
						new Vector2(previousCam.transform.localPosition.x - cutManager.GetCameraBoundaryWidth,
							previousCam.transform.localPosition.y - instantiatePosY);
				}
				else if (objList[i].WhichCutNum > currentCutNum)
				{
					// 미래컷에 영향 주는것을 보여줘야 하면 여기서
				}
			}

			// 오브젝트 삭제 전에 보여줄 행동
			// interactedObject.TranslateAfterHitBoundary(interactedObject.MovingDirection);
			interactedObject.DisconnectSync();
			Destroy(interactedObject.gameObject);
			// StartCoroutine(AfterSyncHorizontalMove(2, interactedObject));
		}
		
		return true;
	}
	
	/// <summary>
	/// 싱크 맞춰준 후 원래 오브젝트는 잠깐동안 이동연산 하고 삭제
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator AfterSyncHorizontalMove(float time, CInteractableObject interactedObject)
	{
		while (time > 0)
		{
			interactedObject.transform.Translate(interactedObject.MovingDirection);
			time -= Time.deltaTime;
			yield return null;
		}
		
		Destroy(interactedObject.gameObject);
	}

	private bool HitRightBoundary(int objId)
	{
		return true;
	}

	/// <summary>
	/// 카메라의 위쪽 경계에 CInteractableObject가 충돌 판정 있으면
	/// </summary>
	/// <param name="objId"></param>
	/// <returns></returns>
	private bool HitCeilBoundary(int objId)
	{
		if (cutManager.MaxCutCount > 2)
		{
			CInteractableObject interactedObject = default;
			int currentCutNum = StageManager.GetInstance().GetCurrentCutNum();

			if (objectDictionary.TryGetValue(objId, out var objList))
			{
				interactedObject = objList.FirstOrDefault(e => e.WhichCutNum == currentCutNum);
			}
			else
			{
				Debug.LogError("Can't find CInteractable Object in Dictionary");
				return false;
			}

			if (interactedObject == default)
			{
				Debug.LogError("interactable object is default");
				return false;
			}
			
			var camPos = cutManager.GetCamera(interactedObject.WhichCutNum).transform.localPosition;
			var instantiatePosX = interactedObject.transform.localPosition.x - camPos.x;
			var aboveCam = cutManager.GetCamera(interactedObject.WhichCutNum - cutManager.MaxCutCount / 2).transform;
			// effectedObjects.Clear();
			
			for (int i = 0; i < objList.Count; ++i)
			{
				/*
				var a = objList[i].WhichCutNum;
				var b = currentCutNum;
				var c = GameManager.GetInstance().GetCutManager.MaxCutCount / 2;
				var d = objList[i].IsSynced;
				*/ /*
				// 3가지로 나눠서 해결, 현재컷보다 과거컷이면 추가생성 후 ID 연결, 현재 컷이면 생성후 기존오브젝트 파괴, 현재보다 미래 컷이면 이동
				if ((objList[i].WhichCutNum >= currentCutNum - cutManager.MaxCutCount / 2 && objList[i].WhichCutNum < currentCutNum)
					|| objList[i].WhichCutNum == currentCutNum)
				{
					var instantiated = Instantiate(objList[i], objList[i].transform.parent);
					
					// 오브젝트 싱크 끊기
					objList[i].DisconnectSync();
					
					// 리스트 항목 교체, 다른 방법 찾기
					objList[i] = instantiated;
					instantiated.Init(objList[i].WhichCutNum, interactedObject.MovingDirection);
					// effectedObjects.Add(instantiated);

					// 생성 위치 다시 잡아줘야됨
					instantiated.transform.localPosition = new Vector2(aboveCam.transform.localPosition.x + instantiatePosX,
						aboveCam.transform.localPosition.y + cutManager.GetCameraBoundaryHeight);
				}
				else if (objList[i].WhichCutNum > currentCutNum)
				{
					objList[i].Init(objList[i].WhichCutNum, interactedObject.MovingDirection);
					// 위치만 잡아주기
					objList[i].transform.localPosition = new Vector2(aboveCam.transform.localPosition.x + instantiatePosX,
						aboveCam.transform.localPosition.y + cutManager.GetCameraBoundaryHeight);
				}
			}
			
			// 오브젝트 삭제 전에 보여줄 행동
			interactedObject.DisconnectSync();
			Destroy(interactedObject.gameObject);
		}
		return true;
	}
}
*/