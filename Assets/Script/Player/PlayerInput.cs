using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    //외부 참조 변수
    [SerializeField] private Player player = null;
    private PlayerInterectController interectController;

    //Local 변수
    private float inputAxis;

    public void PlayerInputInit()
    {
        player = null;
        interectController = null;
    }

    public void SetPlayer(Player tempPlayer)
    {
        if(player != null)
        {
            tempPlayer.transform.localPosition = player.transform.localPosition;
            player.StopPlayer();
        }
        this.player = tempPlayer;
        interectController = player.transform.GetComponentInParent<PlayerInterectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!StageManager.GetInstance().isOptioning) {
            if (player != null && player.IsMovable())
            {
                if (!player.IsLadder())
                {
                    inputAxis = Input.GetAxisRaw("Horizontal");
                    player.PlayerMove(inputAxis);
                    //컷 이동
                    if (Input.GetKeyDown(KeyCode.Z) && player.IsCutChangeable())
                    {
                        player.PlayerZAction();
                    }

                    // 상호작용
                    if (Input.GetKeyDown(KeyCode.X) && player.IsThrowable())
                    {
                        interectController.Interacting();
                    }

                    //사다리
                    if (interectController.ladderTarget != null && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
                    {
                        player.GetLadder();
                    }

                }
                else
                {
                    //사다리 이동  관련 동작
                    inputAxis = Input.GetAxisRaw("Vertical");
                    player.PlayerMove(inputAxis);

                    //사다리를 놓아주는 동작
                    if (inputAxis < 0 && interectController.ladderExit != null)
                    {
                        player.RealeaseLadder();
                    }
                }
            }
        }// end of if(Movable())
    }
}
