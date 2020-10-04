using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ObjectSyncController : MonoBehaviour
{
  [Header("실제 컷의 카메카 크기, 컷의 모양, 컷의 순서가 맞는지 확인")] 
  [SerializeField] private GameObject dummyCutsParent;

  [SerializeField] private GameObject[] eachCut;
  [SerializeField] private CutManager cutManager;
  [SerializeField] private float rayDistance;

  private Transform[] dummyCuts;

  private int objectIdCounter = 0;

  private delegate bool SyncAction(int objId, bool dir);

  private SyncAction[] syncActions = new SyncAction[(int) CInteractableObject.HitBoundaryLocation.MAX];

  private Dictionary<int, List<CInteractableObject>> objectDictionary =
    new Dictionary<int, List<CInteractableObject>>();

  private List<CInteractableObject> effectedObjects = new List<CInteractableObject>();

  public float spawnYPos;
  public float cameraLength;

  public int GetObjectId() => objectIdCounter++;
  public void SetCutManager(CutManager c) => cutManager = c;

  public bool SyncOtherObjects(int id, CInteractableObject.HitBoundaryLocation loc, bool dir) => syncActions[(int) loc](id, dir);

  private void Start()
  {
    objectIdCounter = 0;
    dummyCuts = dummyCutsParent.GetComponentsInChildren<Transform>();
    AddDelegates();
  }

  private void AddDelegates()
  {
    syncActions[(int) CInteractableObject.HitBoundaryLocation.ELEFT_BOUNDARY] = HitLeftBoundary;
    syncActions[(int) CInteractableObject.HitBoundaryLocation.ERIGHT_BOUNDARY] = HitLeftBoundary;
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
	        if (val[i - 1] != null)
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
  private bool HitLeftBoundary(int objId, bool dir)
  {
    int currentCutNum = StageManager.GetInstance().GetCurrentCutNum();
	  var hitDir = new Vector3();
	  hitDir = dir ? Vector3.left : Vector3.right;

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
	  
    // TODO : 오브젝트 카메라 기준 포지션 가져오기 - 해당 더미컷의 로컬 포지션으로 변환 -> 레이 쏘기
    var cutCam = StageManager.GetInstance().GetCutManager.GetCamera(currentCutNum);
	  var hitCut = Physics2D.Raycast(interactedObject.transform.position, hitDir, rayDistance,
		  1 << LayerMask.NameToLayer("Boundary"));
	  var originPos = hitCut.point - (Vector2)cutCam.transform.position;
	  Vector2 dummyPos = dummyCuts[currentCutNum + 1].transform.position;
	  Debug.DrawRay(dummyPos + originPos, hitDir * rayDistance, Color.blue, 10f);
	  var hitDummy = Physics2D.RaycastAll(dummyPos + originPos, hitDir, rayDistance,
		  1 << LayerMask.NameToLayer("Dummy"));

	  if (hitDummy.Length != 2)
	  {
		  return false;
	  }
	  
	  int targetCutNum = -1;

	  for (int i = 1; i < dummyCuts.Length; ++i)
	  {
		  if (dummyCuts[i] == hitDummy[1].transform)
		  {
			  targetCutNum = i - 1;
			  break;
		  }
	  }

	  Debug.DrawRay((Vector2) dummyCuts[targetCutNum + 1].position,
		  hitDummy[1].point - ((Vector2) interactedObject.transform.position - hitCut.point) - (Vector2) dummyCuts[targetCutNum + 1].position, Color.green, 10f);
	  var spawnPosVector = (Vector2) dummyCuts[targetCutNum + 1].position -
	                        (hitDummy[1].point - ((Vector2) interactedObject.transform.position - hitCut.point));
	  // TODO : 타겟 컷 찾고(targetCutNum 이용), 그 컷의 카메라 기준으로 좌표 계산 후 스폰

	  var destPos = (Vector2) cutManager.GetCamera(targetCutNum).transform.position - spawnPosVector;
	  Debug.DrawRay(eachCut[targetCutNum].transform.position,
		  (Vector2) cutManager.GetCamera(targetCutNum).transform.position - spawnPosVector, Color.red, 10f);
	  
	  if (targetCutNum > currentCutNum)
	  {
		  // 미래 컷으로 보낸 경우
		  for (int i = currentCutNum; i <= targetCutNum; ++i)
		  {
			  // 현재 컷부터 i 까지 다 없애기
			  if (i < targetCutNum)
			  {
				  for (int j = 0; j < objList.Count; ++j)
				  {
					  if (objList[j] != null)
					  {
						  if (objList[j].WhichCutNum == i)
						  {
							  var temp = objList[j];
							  objList[j] = null;
							  Destroy(temp);
						  }
					  }
				  }
			  }
			  else if (i == targetCutNum)
			  {
				  // TODO : 일단 카메라 기준으로 오브젝트 생성 후, 해당 컷 활성화 될 때 오브젝트 활성화
				  var o = objList.FirstOrDefault(e => e.WhichCutNum == i);

				  if (o != default)
				  {
					  o.SetActivationPosition(destPos);
				  }
				  else
				  {
					  return false;
				  }
			  }
		  }
	  }
	  else
	  {
		  for (int i = targetCutNum; i < eachCut.Length; ++i)
		  {
			  // 미래 컷은 안 보여주기 때문에 i > currentCutNum보다 큰 경우는 구현할 필요 없음
			  if (i < currentCutNum)
			  {
				  var instantiated = Instantiate(interactedObject);
				  
				  // 싱크 교체 (위험할 수 있음)
				  var idx = objList.FindIndex(e => e.WhichCutNum == i);
				  instantiated.transform.parent = objList[idx].transform.parent;
				  objList[idx] = instantiated;
				  instantiated.Init(objList[idx].WhichCutNum);
				  instantiated.DisconnectSync();

				  instantiated.transform.localPosition = destPos;
			  }
			  else if (i == currentCutNum)
			  {
				  interactedObject.transform.localPosition = destPos;
			  }
			  else if (i > currentCutNum)
			  {
				  // 미래 컷인 경우 이후에 알아서 싱크 맞추기
			  }
		  }
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

  private bool HitRightBoundary(int objId, bool dir)
  {
    return true;
  }

  /// <summary>
  /// 카메라의 위쪽 경계에 CInteractableObject가 충돌 판정 있으면
  /// </summary>
  /// <param name="objId"></param>
  /// <returns></returns>
  private bool HitCeilBoundary(int objId, bool dir)
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