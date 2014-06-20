using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : ShipController {
    void Start () 
    {
        AttachmentStart();
    }
	void Update () 
    {
        ShipUpdate();

        Rotate(0, 1f, 0);
        MoveDirection(0, 0, 0.1f);
        FireGun();
	}
}
