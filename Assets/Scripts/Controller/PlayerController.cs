using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : ShipController {
    void Start () 
    {
        AttachmentStart();
    }
    void Update ()
    {
        ShipUpdate();

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
            Rotate(0, 0, 0.5f);
        }
        if (Input.GetKey (KeyCode.E))
        {
            Rotate(0, 0, -0.5f);
        }
        if (Input.GetKey (KeyCode.Alpha1))
        {
            CurrentGunGroup = "laser1";
        }
        if (Input.GetKey (KeyCode.Alpha2))
        {
            CurrentGunGroup = "laser2";
        }

        if (Input.GetKey (KeyCode.G))
        {
            foreach (HardpointController hardpoint in Hardpoints)
            {
                Detach(hardpoint, true);
            }
            DiscoverConnected(true);
        }

        if (Input.GetMouseButton(0)) 
        {
            CallAttachmentInterfaceByGroup(CurrentGunGroup, "Fire");
            //FireGun();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CallAttachmentInterfaceByGroup("engine", "SetThrust", "100%");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CallAttachmentInterfaceByGroup("engine", "SetThrust", "0%");
        }

        // TODO!
        // Figure out why clicking the mouse somehow results in a rotation.
        // Hopefully it is a Unity in Wine issue rather than a real one.
        float xspeed = Input.GetAxis("Mouse X");
        float yspeed = Input.GetAxis("Mouse Y") * 1.0f;
        Rotate(yspeed, xspeed, 0);
	}
}
