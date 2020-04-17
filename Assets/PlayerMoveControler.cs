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
        PlayerScript.PlayerMove(Input.GetAxisRaw("Horizontal"));

        if(Input.GetKeyDown(KeyCode.UpArrow) && PlayerScript.isGround)
        {
            PlayerScript.PlayerJump();
        }
    }
}
