using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncController : MonoBehaviour
{
    public GameObject[] eachCut;
    public InteractableObject[] spawnedObjects = new InteractableObject[6];
    public bool[] collisionCheck = new bool[6];
    public bool[] instantiated = new bool[6];
    public float padding;
    public int currentCut;

    public int currentCutNum;

    // 왼쪽 충돌이면 이전 컷부터 변화
    // 오른쪽 충돌이면 이후 컷부터 변화
    // 위쪽 충돌이면 위쪽 컷부터 변화
    // 아래쪽 충돌이면 아래쪽 컷부터 변화

    public void HitCollider(InteractableObject obj, bool verticalBoundary, Vector2 colliderLocalPos)
    {
        if (obj.IsInstantiated)
        {
            return;
        }

        currentCutNum = obj.CurrentCutNum;
        Debug.Log("dd");

        // 밀기
        if (obj.CutNum == obj.CurrentCutNum)
        {
            Vector2 spawnPosition = this.GetSyncPosition(obj, verticalBoundary, colliderLocalPos);
            // 특정 컷에 생성하게 예외 추가해야 함, 현재 컷에서 오른쪽으로 밀었을때
            for (int i = obj.CurrentCutNum + 1; i < 6; ++i)
            {
                var spawnedObject = Instantiate(obj.gameObject, eachCut[obj.CutNum].transform);
                obj.childObjectPair[i] = spawnedObject.GetComponent<InteractableObject>();
                spawnedObject.transform.localPosition = spawnPosition;
            }
        }

        //if (obj.CutNum == obj.CurrentCutNum)
        //{
        //    Vector2 spawnPosition = this.GetSyncPosition(obj, verticalBoundary, colliderLocalPos);

        //    // 특정 컷에 생성하게 예외 추가해야 함, 현재 컷에서 오른쪽으로 밀었을때
        //    for (int i = obj.CurrentCutNum + 1; i < 6; ++i)
        //    {
        //        var spawnedObject = Instantiate(obj.gameObject, eachCut[obj.CutNum].transform);
        //        obj.childObjectPair[i] = spawnedObject.GetComponent<InteractableObject>();
        //    }
        //}

        // 어디다 생성할지 포지션 정해주기
        //Vector2 spawnPosition = this.GetSyncPosition(obj, verticalBoundary, colliderLocalPos);
        //this.collisionCheck[obj.CutNum] = true;

        // TODO : eachCut[obj.CutNum]을 통해 해당하는 컷에만 생성해야 함
        //var spawnedObject = Instantiate(obj.gameObject, eachCut[obj.CutNum].transform);
        //spawnedObjects[obj.CutNum] = spawnedObject.GetComponent<InteractableObject>();
        //spawnedObjects[obj.CutNum].Instantiated(true);
        //spawnedObject.transform.localPosition = spawnPosition;
        // obj.spawnedObjectPair[obj.CutNum] = spawnedObjects[obj.CutNum];
    }

    public void Thrown(int currentCutNum, GameObject obj, Vector2 vel)
    {
        // 3번씬 -> 0~5번
        // 4번씬 -> 1~5번
        // 5번씬 -> 2~5번

        for(int i = currentCutNum - 3; i < 6; ++i)
        {

        }
    }

    public void ExitCollider(InteractableObject obj)
    {
        if (obj.IsInstantiated)
        {
            obj.Instantiated(false);
        }
    }

    /// <summary>
    /// 들고있는 오브젝트를 기준으로 생성된 나머지 오브젝트 동기화
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="verticalBoundary"></param>
    /// <param name="colliderPosition"></param>
    public void SyncObject(InteractableObject[] objectPair)
    {
        //for (int i = 0; i < 6; ++i)
        //{
        //    if (objectPair[i] != null)
        //    {
        //        Debug.Log(i + "번째" );
        //        // 현재 컷 확인
        //        if (objectPair[i].CutNum == 0)
        //        {
                    
        //        }
        //    }
        //}
    }

    private Vector2 GetSyncPosition(InteractableObject obj, bool verticalBoundary, Vector2 colliderLocalPos)
    {
        if (verticalBoundary)
        {
            // 오른쪽으로 밀기
            if (obj.transform.localPosition.x > 0)
            {
                float rightPivot = colliderLocalPos.x - padding;
                float leftPivot = -colliderLocalPos.x + padding;

                return new Vector2(leftPivot + (obj.transform.localPosition.x - rightPivot), obj.transform.localPosition.y);
                // return new Vector2(-(obj.transform.localPosition.x) + padding, obj.transform.localPosition.y);
            }
            // 왼쪽으로 밀기
            else
            {
                float leftPivot = colliderLocalPos.x + padding;
                float rightPivot = -colliderLocalPos.x - padding;

                return new Vector2(rightPivot + (obj.transform.localPosition.x - leftPivot), obj.transform.localPosition.y);
                //return new Vector2(-(obj.transform.localPosition.x) - padding, obj.transform.localPosition.y);
            }
        }
        // 위아래로 충돌
        else
        {
            //return new Vector2(obj.transform.position.x, -(obj.transform.position.y - eachCut[obj.CutNum].transform.position.y));
            return Vector2.zero;
        }
    }
}
