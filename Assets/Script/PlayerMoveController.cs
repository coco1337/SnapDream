using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveController : MonoBehaviour
{
    Player playerScript;
    PlayerInterectController interectController;

    private void Start()
    {
        playerScript = transform.GetComponentInParent<Player>();
        interectController = transform.GetComponentInParent<PlayerInterectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isMovable())
        {
            float inputAxis;
            if (playerScript.GetPlayerState() != Player.PlayerState.Interaction_Ladder)
            {
                inputAxis = Input.GetAxisRaw("Horizontal");
                playerScript.PlayerMove(inputAxis);

                //상호작용중이고 움직일때만 동작
                if (interectController.dragObject != null)
                {
                    playerScript.getDrag();
                    interectController.MoveInteractObject(inputAxis);
                }

                if (Input.GetKeyDown(KeyCode.Z) && playerScript.IsJumpable())
                {
                    playerScript.moveNextCut();
                    if (playerScript.isMovable())
                        playerScript.PlayerJump();
                    else
                        playerScript.playerStop(); //이동 정지
                }

                // 상호작용
                if (Input.GetKeyDown(KeyCode.X) && interectController.CanInterectable() && playerScript.GetPlayerState() == Player.PlayerState.Idle)
                {
                    interectController.Interacting();
                }

                if (interectController.ladderTarget != null && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && playerScript.IsJumpable())
                {
                    playerScript.getLadder();
                }

            }
            else
            {
                playerScript.transform.position = new Vector2(interectController.ladderTarget.transform.position.x, transform.position.y);
                inputAxis = Input.GetAxisRaw("Vertical");
                playerScript.PlayerLadderMove(inputAxis);
                if(inputAxis < 0 && interectController.ladderExit != null)
                {
                    playerScript.realeaseLadder();
                }
            }
        }// end of if(Movable())
    }
}
