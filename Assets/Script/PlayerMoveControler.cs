using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveControler : MonoBehaviour
{
    Player playerScript;
    PlayerInterectControler interectControler;

    private void Start()
    {
        playerScript = transform.GetComponentInParent<Player>();
        interectControler = transform.GetComponentInParent<PlayerInterectControler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isMovable())
        {
            float inputAxis;
            if (playerScript.GetPlayerState() != Player.PlayerState.Interaction_Labber)
            {
                inputAxis = Input.GetAxisRaw("Horizontal");
                playerScript.PlayerMove(inputAxis);

                //상호작용중이고 움직일때만 동작
                if (interectControler.isInteracting && playerScript.GetPlayerState() == Player.PlayerState.Move)
                    interectControler.MoveInteractObject(inputAxis);

                if (Input.GetKeyDown(KeyCode.Z) && playerScript.isGround)
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
                    interectControler.InteractObject();
                }
            }
            else
            {
                inputAxis = Input.GetAxisRaw("Vertical");
                playerScript.PlayerLabberMove(inputAxis);

            }



        }// end of if(Movable())
    }
}
