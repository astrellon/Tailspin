using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : ShipController {
    private int Counter = 0;
    void Start () 
    {
        AttachmentStart();
    }
	void Update () 
    {
        Counter++;
        if (Counter > 10)
        {
            IList<AttachmentController> nearby = DiscoverNearbyAttachable();
            if (nearby.Count > 0)
            {
                foreach (AttachmentController attachment in nearby)
                {
                    HardpointController mounting = FindMountingHardpoint();
                    if (mounting == null)
                    {
                        continue;
                    }

                    HardpointController attachMounting = attachment.FindMountingHardpoint();
                    if (attachMounting == null)
                    {
                        continue;
                    }

                    PullAndAttach(mounting, attachment, attachMounting);
                }
            }
            Counter = 0;
        }

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

        // TODO!
        // Figure out why clicking the mouse somehow results in a rotation.
        float xspeed = Input.GetAxis("Mouse X");
        float yspeed = Input.GetAxis("Mouse Y") * 1.0f;
        Debug.Log("Speeds: " + xspeed + " | " + yspeed);
        Rotate(yspeed, xspeed, 0);
	}
}
