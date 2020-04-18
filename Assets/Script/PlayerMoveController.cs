﻿using System.Collections;
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
                if (interectController.isInteracting && playerScript.GetPlayerState() == Player.PlayerState.Move)
                    interectController.MoveInteractObject(inputAxis);

                if (Input.GetKeyDown(KeyCode.Z) && playerScript.GetPlayerState() != Player.PlayerState.Jump)
                {
                    playerScript.moveNextCut();
                    if (playerScript.isMovable())
                        playerScript.PlayerJump();
                    else
                        playerScript.playerStop(); //이동 정지
                }

                // 상호작용
                if (Input.GetKeyDown(KeyCode.X))
                {
                    interectController.InteractObject();
                }

                if (interectController.ladderTarget != null && Input.GetKeyDown(KeyCode.UpArrow))
                {
                    interectController.getLadder();
                }

            }
            else
            {
                inputAxis = Input.GetAxisRaw("Vertical");
                playerScript.PlayerLadderMove(inputAxis);
            }
        }// end of if(Movable())
    }
}