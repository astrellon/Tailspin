using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : ShipController {
    void Start () {
         
    }
	void Update () {
   //     base.Update();

        if (!Screen.lockCursor)
        {
            return;
        }
        if (Input.GetKey (KeyCode.A)) 
        {
            MoveDirection(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)) 
        {
            MoveDirection(1, 0, 0);
        }
        if (Input.GetKey (KeyCode.W)) 
        {
            MoveDirection(0, 0, 1);
        }
        if (Input.GetKey (KeyCode.S)) 
        {
            MoveDirection(0, 0, -1);
        }
        if (Input.GetKey (KeyCode.LeftShift))
        {
            MoveDirection(0, 1, 0);
        }
        if (Input.GetKey (KeyCode.LeftControl))
        {
            MoveDirection(0, -1, 0);
        }

        if (Input.GetKey (KeyCode.Q))
        {
            Rotate(0, 0, 1);
        }
        if (Input.GetKey (KeyCode.E))
        {
            Rotate(0, 0, -1);
        }
        if (Input.GetKey (KeyCode.Alpha1))
        {
            ChangeGun(Guns[0]);
        }
        if (Input.GetKey (KeyCode.Alpha2))
        {
            ChangeGun(Guns[1]);
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) 
        {
            FireGun();
        }

        float xspeed = Input.GetAxis("Mouse X");
        float yspeed = Input.GetAxis("Mouse Y") * 1.0f;
        Rotate(yspeed, xspeed, 0);
	}
}
