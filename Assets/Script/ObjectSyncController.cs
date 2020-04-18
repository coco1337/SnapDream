using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncController : MonoBehaviour
{
    public GameObject[] eachCut;
    //public InteractableObject[] spawnedObjects = new InteractableObject[6];
    //public bool[] collisionCheck = new bool[6];
    //public bool[] instantiated = new bool[6];
    public float padding;
    public int currentCut;
    public float spawnYPos;

    public int currentCutNum;

    // 왼쪽 충돌이면 이전 컷부터 변화
    // 오른쪽 충돌이면 이후 컷부터 변화
    public void InstantiateObjects(int currentCutNum, GameObject obj, Vector2 vel)
    {
        // 오른쪽 충돌시
        if (obj.transform.localPosition.x > 0)
        {
            var InteractableObject = obj.GetComponent<InteractableObject>();
            Vector2 spawnPosition = this.GetSyncPosition(InteractableObject, true, new Vector2(25, 0));
            // 다음칸 부터 스폰
            for (int i = currentCutNum + 1; i < 6; ++i)
            {
                var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
                InteractableObject.childObjectPair[i] = spawnedObject.GetComponent<InteractableObject>();
                spawnedObject.transform.localPosition = spawnPosition;
            }
        }
        // 왼쪽 충돌시
        else
        {

        }
    }

    //public void HitCollider(InteractableObject obj, bool verticalBoundary, Vector2 colliderLocalPos)
    //{
    //    if (obj.IsInstantiated)
    //    {
    //        return;
    //    }

    //    currentCutNum = obj.CurrentCutNum;
    //    Debug.Log("dd");

    //    // 밀기
    //    if (obj.CutNum == obj.CurrentCutNum)
    //    {
    //        Vector2 spawnPosition = this.GetSyncPosition(obj, verticalBoundary, colliderLocalPos);
    //        // 특정 컷에 생성하게 예외 추가해야 함, 현재 컷에서 오른쪽으로 밀었을때
    //        for (int i = obj.CurrentCutNum + 1; i < 6; ++i)
    //        {
    //            var spawnedObject = Instantiate(obj.gameObject, eachCut[obj.CutNum].transform);
    //            obj.childObjectPair[i] = spawnedObject.GetComponent<InteractableObject>();
    //            spawnedObject.transform.localPosition = spawnPosition;
    //        }
    //    }
    //}

    public void Thrown(int currentCutNum, GameObject obj, Vector2 vel)
    {
        if (currentCutNum < 3)
        {
            return;
        }

        float xPos = obj.transform.localPosition.x;

        // 3번씬 -> 0~5번
        // 4번씬 -> 1~5번
        // 5번씬 -> 2~5번

        for (int i = currentCutNum - 3; i < 6; ++i)
        {
            var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
            spawnedObject.transform.localPosition = new Vector2(xPos, spawnYPos);
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


    public void SyncObject(InteractableObject[] objectPair)
    {

    }

    private Vector2 GetSyncPosition(InteractableObject obj, bool verticalBoundary, Vector2 colliderLocalPos)
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
}
