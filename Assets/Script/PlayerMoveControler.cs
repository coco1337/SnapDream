using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveControler : MonoBehaviour
{
    Player PlayerScript;

    private void Start()
    {
        PlayerScript = transform.GetComponentInParent<Player>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart State
        }


        if (PlayerScript.isMovable())
        {
            PlayerScript.PlayerMove(Input.GetAxisRaw("Horizontal"));

            if (Input.GetKeyDown(KeyCode.Z) && PlayerScript.isGround)
            {
                PlayerScript.moveNextCut();
                if (PlayerScript.isMovable())
                    PlayerScript.PlayerJump();
                else
                    PlayerScript.playerStop(); //이동 정지
            }

            // 상호작용
            if (Input.GetKeyDown(KeyCode.X))
            {
                PlayerScript.InteractObject();
            }
        }
    }
}
