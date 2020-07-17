using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CutManager : MonoBehaviour
{
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

    public int GetCurrentCutNum() => currentCut;
    public List<Player> GetPlayerList => playerList;

    // Start is called before the first frame update
    private void Start()
    {
        currentCut = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MoveNextCut();
        }
    }

    private void MoveNextCut()
    {
        
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

            // 부모 설정
            tempBackGround.transform.parent = cut;
            tempPlayer.transform.parent = cut;
            tempCamera.transform.parent = cut;

            // 초기위치 설정
            tempBackGround.transform.localPosition = Vector3.zero;
            tempPlayer.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            tempCamera.transform.localPosition = Vector3.zero + new Vector3(0, 0, -9);
            
            camImage[i].SetActive(false);

            foreach (var obj in tempBackGround.GetComponentsInChildren<InteractableObject>())
            {
                obj.Init(i);
            }

            playerList.Add(tempPlayer.GetComponent<Player>());
        }

        // 생성 끝, 첫번째 컷 활성화
        camImage[0].SetActive(true);
    }

    public void MoveToNextCut()
    {
        currentCut++;
        camImage[currentCut].SetActive(true);
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
