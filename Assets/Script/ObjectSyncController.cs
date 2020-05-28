using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSyncController : MonoBehaviour
{
    [SerializeField] private GameObject[] eachCut;
    [SerializeField] private GameManager gameManager;
    
    public float spawnYPos;
    public float cameraLength;

    private void Start()
    {
        gameManager = GameManager.getInstance();
        
        // TODO : Init 함수에서 받아오는게 나음. 일단 임시
        StartCoroutine(GetGameManager());
    }

    IEnumerator GetGameManager()
    {
        yield return new WaitForFixedUpdate();
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }
    }

    // 왼쪽 충돌이면 이전 컷부터 변화
    // 오른쪽 충돌이면 이후 컷부터 변화 - 없음
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
            if (gameManager.GetCurrentCutNum() == 0 || gameManager.GetCurrentCutNum() == 3)
            {
                return;
            }

            // 현재 카메라 x좌표
            var cameraX = eachCut[gameManager.GetCurrentCutNum()].transform.Find("Camera(Clone)").transform.localPosition.x;

            // 생성될 컷의 카메라 x 좌표(바로 직전 컷의 카메라 좌표)
            var targetCameraX = eachCut[gameManager.GetCurrentCutNum() - 1].transform.Find("Camera(Clone)").transform.localPosition.x;

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
        var targetCameraX = eachCut[gameManager.GetCurrentCutNum() - 3].transform.Find("Camera(Clone)").transform.localPosition.x;

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
}
