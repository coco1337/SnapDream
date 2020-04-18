using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncController : MonoBehaviour
{
    public GameObject[] eachCut;
    public float padding;
    public int currentCut;
    public float spawnYPos;

    public int currentCutNum;
    public InteractableObject InteractableObj;

    // 왼쪽 충돌이면 이전 컷부터 변화
    // 오른쪽 충돌이면 이후 컷부터 변화 - 없음
    public void InstantiateObjects(int currentCutNum, GameObject obj, Vector2 vel)
    {
        var InteractableObj = obj.GetComponent<InteractableObject>();
        // 오른쪽 충돌시
        if (obj.transform.localPosition.x > 0)
        {
            //Vector2 spawnPosition = this.GetSyncPosition(InteractableObject, new Vector2(25, 0));
            //// 다음칸 부터 스폰
            //for (int i = currentCutNum + 1; i < 6; ++i)
            //{
            //    var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
            //    InteractableObject.childObjectPair[i] = spawnedObject.GetComponent<InteractableObject>();
            //    spawnedObject.transform.localPosition = spawnPosition;
            //}
        }
        // 왼쪽 충돌시
        else
        {
            if (currentCutNum == 0)
            {
                return;
            }

            //Vector2 spawnPosition = this.GetSyncPosition(InteractableObject, new Vector2(-25, 0));

            // 카메라 가로길이 12, 6

            // 현재 카메라 x좌표
            float cameraX = eachCut[currentCutNum].transform.Find("Camera(Clone)").transform.localPosition.x;

            // 생성될 컷의 카메라 x 좌표(바로 직전 컷의 카메라 좌표)
            float targetCameraX = eachCut[currentCutNum - 1].transform.Find("Camera(Clone)").transform.localPosition.x;

            // 아마도 물건 길이 + 1
            float result = targetCameraX + 6 + 1;

            InteractableObject[] childPair = new InteractableObject[6];

            // 전칸부터 스폰
            for (int i = currentCutNum - 1; i < 6; ++i)
            {
                var spawnedObject = Instantiate(obj.gameObject, eachCut[i].transform);
                childPair[i] = spawnedObject.GetComponent<InteractableObject>();
                spawnedObject.transform.localPosition = new Vector2(result, obj.gameObject.transform.localPosition.y);
            }

            InteractableObj.childObjectPair = childPair;
            InteractableObj.needSync = true;
        }
    }

    public void Thrown(int currentCutNum, GameObject obj, Vector2 vel)
    {
        if (currentCutNum < 3)
        {
            return;
        }

        // 현재 카메라 x좌표
        float cameraX = eachCut[currentCutNum].transform.Find("Camera(Clone)").transform.localPosition.x;

        // 생성될 컷의 카메라 x 좌표
        float targetCameraX = eachCut[currentCutNum - 3].transform.Find("Camera(Clone)").transform.localPosition.x;

        // 생성될 오브젝트의 좌표
        float result = (obj.transform.localPosition.x - cameraX) + targetCameraX;

        for (int i = currentCutNum - 3; i < 6; ++i)
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
        for (int i = 0; i < 6; ++i)
        {
            if (objectPair[i] != null)
            {
                objectPair[i].GetComponent<Rigidbody2D>().velocity = vel;
            }
        }
    }

    private Vector2 GetSyncPosition(InteractableObject obj, Vector2 colliderLocalPos)
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
