using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private Player player = null;
    private PlayerInterectController interectController;

    public void SetPlayer(Player tempPlayer)
    {
        if(player != null)
        {
            tempPlayer.transform.localPosition = player.transform.localPosition;
            player.playerStop();
        }
        this.player = tempPlayer;
        interectController = player.transform.GetComponentInParent<PlayerInterectController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.isMovable() && !GameManager.GetInstance().isOptioning)
        {
            float inputAxis;
            if (player.GetPlayerState() != Player.PlayerState.Interaction_Ladder)
            {
                inputAxis = Input.GetAxisRaw("Horizontal");
                player.PlayerMove(inputAxis);

                //Drag
                if (interectController.dragObject != null)
                {
                    interectController.MoveInteractObject(inputAxis);
                }

                //점프
                if (Input.GetKeyDown(KeyCode.Z) && player.IsJumpable())
                {
                    GameManager.GetInstance().NextCut();
                }

                // 상호작용
                if (Input.GetKeyDown(KeyCode.X) && interectController.CanInterectable() && player.IsThrowable())
                {
                    interectController.Interacting();
                }

                //사다리
                if (interectController.ladderTarget != null && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) && player.IsJumpable())
                {
                    player.getLadder();
                }

            }
            else
            {
                player.transform.position = new Vector2(interectController.ladderTarget.transform.position.x, transform.position.y);
                inputAxis = Input.GetAxisRaw("Vertical");
                player.PlayerLadderMove(inputAxis);
                if(inputAxis < 0 && interectController.ladderExit != null)
                {
                    player.realeaseLadder();
                }
            }
        }// end of if(Movable())
    }
}
