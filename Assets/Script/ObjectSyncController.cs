using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncController : MonoBehaviour
{
    public GameObject[] eachCut;
    public float padding;
    public int currentCut;
    public float spawnYPos;
    public float cameraLength;

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

        }
        // 왼쪽 충돌시
        else
        {
            // 제일 왼쪽 컷들은 물건 왼쪽으로 넘길수 없음
            if (currentCutNum == 0 || currentCutNum == 3)
            {
                return;
            }

            // 현재 카메라 x좌표
            float cameraX = eachCut[currentCutNum].transform.Find("Camera(Clone)").transform.localPosition.x;

            // 생성될 컷의 카메라 x 좌표(바로 직전 컷의 카메라 좌표)
            float targetCameraX = eachCut[currentCutNum - 1].transform.Find("Camera(Clone)").transform.localPosition.x;

            // 아마도 물건 길이 + 1
            float result = targetCameraX + (cameraLength / 2) + 1;

            InteractableObject[] childPair = new InteractableObject[6];

            // currentNum 전 칸 부터 스폰
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
}
