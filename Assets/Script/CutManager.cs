using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutManager : MonoBehaviour
{
    [SerializeField] private Transform cutField;
    [SerializeField] private GameObject backGround;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cutCamera;
    [SerializeField] private RenderTexture[] cameraRawImage;
    [SerializeField] private float cameraBoundury = 20f;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private List<GameObject> camImage = new List<GameObject>();
    [SerializeField] private int currentCut = 0;

    [SerializeField] Vector2 spawnPosition;
    List<Player> playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        currentCut = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CutInit()
    {
        currentCut = 0;

        for (int i = 0; i < cutField.childCount; i++)
        {
            Transform cut = cutField.GetChild(i);
            GameObject tempBackGround = Instantiate(backGround, Vector3.zero, Quaternion.identity);
            GameObject tempPlayer = Instantiate(player, Vector3.zero, Quaternion.identity);
            GameObject tempCamera = Instantiate(cutCamera, Vector3.zero, Quaternion.identity);

            tempCamera.GetComponent<Camera>().targetTexture = cameraRawImage[i];

            //Player Setting
            tempPlayer.GetComponent<Player>().SetPlayerCutNumber(i);

            //Camera Setting
            CameraController tempCameraController = tempCamera.GetComponent<CameraController>();
            tempCameraController.basePoint = tempBackGround.transform;
            tempCameraController.player = tempPlayer.transform;
            tempCameraController.bounduryValue = cameraBoundury;

            tempBackGround.transform.parent = cut;
            tempPlayer.transform.parent = cut;
            tempCamera.transform.parent = cut;

            tempBackGround.transform.localPosition = Vector3.zero;
            tempPlayer.transform.localPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            tempCamera.transform.localPosition = Vector3.zero + new Vector3(0, 0, -9);

            foreach (InteractableObject obj in tempBackGround.GetComponentsInChildren<InteractableObject>())
            {
                obj.Init(i);
            }

            playerList.Add(tempPlayer.GetComponent<Player>());

        }

        for (int i = 0; i < canvas.childCount; i++)
        {
            camImage.Add(canvas.GetChild(i).gameObject);
            camImage[i].SetActive(false);
            camImage[i].GetComponent<RawImage>().texture = cameraRawImage[i];
        }
        camImage[0].SetActive(true);
    }

    public void MoveToNextCut()
    {
        currentCut++;
        camImage[currentCut].SetActive(true);
    }

    public int GetCurrentCutNum()
    {
        return currentCut;
    }

    public void StageClear()
    {
        foreach (var player in playerList)
        {
            player.StageClear();
        }
    }
}
