﻿using System.Collections;
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
        if (PlayerScript.isMovable())
        {
            PlayerScript.PlayerMove(Input.GetAxisRaw("Horizontal"));

            if (Input.GetKeyDown(KeyCode.Z) && PlayerScript.isGround)
            {
                PlayerScript.moveNextCut();
                if (PlayerScript.isMovable())
                    PlayerScript.PlayerJump();
                else
                    PlayerScript.PlayerMove(0); //이동 정지
            }
        }
    }
}