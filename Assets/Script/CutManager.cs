using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ObjectSyncController))]
public sealed class CutManager : MonoBehaviour
{
    [Header("Cut Information")]
    [SerializeField] private Transform cutField;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cutCamera;
    [SerializeField] private RenderTexture cameraRawImage;
    [SerializeField] private float cameraBoundary = 20f;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private List<GameObject> camImage = new List<GameObject>();
    [SerializeField] private int currentCut = 0;
    [SerializeField] private Vector2 spawnPosition;

    private List<Player> playerList = new List<Player>();
    private List<Camera> cutCameras = new List<Camera>();

    [Header("Object Sync")]
    private ObjectSyncController syncController;

    public int GetCurrentCutNum() => currentCut;
    public int MaxCutCount => cutField.childCount;
    public List<Player> GetPlayerList => playerList;
    public ObjectSyncController GetObjectSyncController => syncController;
    public Camera GetCamera(int t) => cutCameras[t];

    // Start is called before the first frame update
    private void Start()
    {
        currentCut = 0;
        syncController = this.GetComponent<ObjectSyncController>();
        syncController.SetCutManager(this);
    }

    public void CutInit()
    {
        currentCut = 0;

        for (int i = 0; i < cutField.childCount; i++)
        {
            var cut = cutField.GetChild(i);
            var tempBackGround = Instantiate(backGround, Vector3.zero, Quaternion.identity);
            var tempPlayer = Instantiate(player, Vector3.zero, Quaternion.identity);
            var tempCamera = Instantiate(cutCamera, Vector3.zero, Quaternion.identity);
            
            // 상호작용 가능한 오브젝트 연결
            // 0일때와 나머지 컷을 구분해서 id 배정해줘야 됨
            var interactableObjects = tempBackGround.GetComponentsInChildren<ObjectId>();
            for (int j = 0; j < interactableObjects.Length; ++j)
            {
                if (i == 0)
                {
                    var t = syncController.GetObjectId();
                    Debug.Log("Id check : GetObjectId() == " + t + ", j : " + j);
                }
                
                interactableObjects[j].SetId(j);
                var obj = interactableObjects[j].GetInteractableObject;
                obj.Init(i);
                syncController.AddObject(j, obj);
                //syncController.AddObject(j, interactableObjects[j]);
            }

            // 각 컷별로 카메라 연결
            var rawImage = Instantiate(cameraRawImage);
            tempCamera.GetComponent<Camera>().targetTexture = rawImage;
            camImage.Add(canvas.GetChild(i).gameObject);
            camImage[i].GetComponent<RawImage>().texture = rawImage;

            // Player Setting
            tempPlayer.GetComponent<Player>().SetPlayerCutNumber(i);

            // Camera Setting
            var tempCameraController = tempCamera.GetComponent<CameraController>();
            tempCameraController.basePoint = tempBackGround.transform;            // 삭제 예정
            tempCameraController.player = tempPlayer.transform;
            tempCameraController.bounduryValue = cameraBoundary;                  // 삭제 예정
            cutCameras.Add(tempCamera.GetComponent<Camera>());

            // 부모 설정
            tempBackGround.transform.parent = cut;
            tempPlayer.transform.parent = cut;
            tempCamera.transform.parent = cut;

            // 초기위치 설정
            tempBackGround.transform.localPosition = Vector3.zero;
            tempPlayer.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            tempCamera.transform.localPosition = Vector3.zero + new Vector3(0, 0, -9);
            
            camImage[i].SetActive(false);

            playerList.Add(tempPlayer.GetComponent<Player>());
        }

        // 생성 끝, 첫번째 컷 활성화
        camImage[0].SetActive(true);
    }

    public void MoveToNextCut()
    {
        currentCut++;
        camImage[currentCut].SetActive(true);
        syncController.MoveNextCut(currentCut);
    }

    /// <summary>
    /// 애니메이션 재생용
    /// </summary>
    public void StageClear()
    {
        foreach (var player in playerList)
        {
            player.StageClear();
        }
    }
}
