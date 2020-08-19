using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ObjectSyncController))]
public sealed class CutManager : MonoBehaviour
{
	[Header("Cut Information")] [SerializeField]
	private Transform cutField;

	[SerializeField] private GameObject backGround;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject cutCamera;
	[SerializeField] private RenderTexture cameraRawImage;
	[SerializeField] private float cameraBoundary = 20f;
	[SerializeField] private RectTransform canvas;
	[SerializeField] private List<GameObject> camImage = new List<GameObject>();
	[SerializeField] private int currentCut = 0;
	[SerializeField] private Vector2 spawnPosition;

	[SerializeField] private GameObject dummyCutParent;

	private List<Player> playerList = new List<Player>();
	private List<Camera> cutCameras = new List<Camera>();

	[Header("Object Sync")] [SerializeField]
	private EdgeCollider2D cameraBoundaryCollider;

	private ObjectSyncController syncController;
	private float cameraBoundaryWidth;
	private float cameraBoundaryHeight;

	public int GetCurrentCutNum() => currentCut;
	public int MaxCutCount => cutField.childCount; //TODO : 컷 상/하 구분에 사용되는데 바뀌면 다른 대안 찾기
	public List<Player> GetPlayerList => playerList;
	public ObjectSyncController GetObjectSyncController => syncController;
	public Camera GetCamera(int cut) => cutCameras[cut];
	public float GetCameraBoundaryWidth => cameraBoundaryWidth;
	public float GetCameraBoundaryHeight => cameraBoundaryHeight;
	public void SetBackGround(GameObject prefab) => backGround = prefab;

	// Start is called before the first frame update
//	private void Awake()
//	{
//		currentCut = 0;
//		syncController = this.GetComponent<ObjectSyncController>();
//		syncController.SetCutManager(this);
//	}

	private Vector2[] CalculateCameraBoundary(Camera cam)
	{
		var a = cam.transform.position.z;
		var fov = cam.fieldOfView * .5f;
		fov = fov * Mathf.Deg2Rad;
		var h = (Mathf.Tan(fov) * a);
		var w = (h / cam.pixelHeight) * cam.pixelWidth;
		cameraBoundaryWidth = w;
		cameraBoundaryHeight = h;

		var arr = new Vector2[5];
		arr[0] = new Vector2(-w, -h);
		arr[1] = new Vector2(w, -h);
		arr[2] = new Vector2(w, h);
		arr[3] = new Vector2(-w, h);
		arr[4] = new Vector2(-w, -h);

		return arr;
	}

	public void CutInit()
	{
		currentCut = 0;
		syncController = this.GetComponent<ObjectSyncController>();
		syncController.SetCutManager(this);
		
		currentCut = 0;
		
		var children = dummyCutParent.GetComponentsInChildren<Renderer>();

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
			tempCameraController.basePoint = tempBackGround.transform; // 삭제 예정
			tempCameraController.player = tempPlayer.transform;
			tempCameraController.bounduryValue = cameraBoundary; // 삭제 예정
			tempCameraController.Init();
			cutCameras.Add(tempCamera.GetComponent<Camera>());

			// 부모 설정
			tempBackGround.transform.parent = cut;
			tempPlayer.transform.parent = cut;
			tempCamera.transform.parent = cut;

			// 초기위치 설정
			tempBackGround.transform.localPosition = Vector3.zero;
			tempPlayer.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
			tempCamera.transform.localPosition = Vector3.zero + new Vector3(0, 0, -9);

			// 카메라 바운더리 설정 및 추가
			if (cameraBoundaryCollider != null)
			{
				var col = Instantiate(cameraBoundaryCollider, tempCamera.transform);
				col.transform.localPosition = new Vector3(tempCamera.transform.localPosition.x,
					tempCamera.transform.localPosition.y, -tempCamera.transform.position.z);
				col.points = CalculateCameraBoundary(tempCamera.GetComponent<Camera>());
			}
			else
			{
				Debug.LogError(nameof(CutManager) + " Error, pls check camera boundary collider");
				return;
			}

			children[i].material.mainTexture = rawImage;

			camImage[i].SetActive(false);

			playerList.Add(tempPlayer.GetComponent<Player>());
		}

		// 생성 끝, 첫번째 컷 활성화
		camImage[0].SetActive(true);
	}

	public void MoveToNextCut()
	{
		if (currentCut < MaxCutCount - 1)
		{
			currentCut++;
			camImage[currentCut].SetActive(true);
			syncController.MoveNextCut(currentCut);
		}
		else
		{
			// 컷 못넘김
		}
	}

	/// <summary>
	/// 애니메이션 재생용
	/// </summary>
	public void StageClear()
	{
		foreach (var p in playerList)
		{
			p.StageClear();
		}
	}
}