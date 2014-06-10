using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    public static SceneManager MainManager;

	// Use this for initialization
	void Start () {
        MainManager = this;
	
        Screen.lockCursor = true;

        /*
        GameObject player = GameObject.Find("PlayerShip");
        GameObject gun = GameObject.Find("TestGun");
        GunController gunController = gun.GetComponent<GunController>();
        gunController.DiscoverHardpoints();
        List<HardpointController> mountPoints = gunController.Hardpoints[HardpointController.Type.MOUNTING];

        GameObject hardpoint = GameObject.Find("HardpointRight");
        HardpointController hardpointMount = hardpoint.GetComponent<HardpointController>();

        ShipController playerShip = player.GetComponent<ShipController>();
        */
        //playerShip.PullAndAttach(hardpointMount, gunController, mountPoints[0]);

        //playerShip.Attach(hardpointMount, gunController, mountPoints[0]);
        //playerShip.DiscoverConnected();
	}
	
	// Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) 
        {
            Screen.lockCursor = true;
        }

        if (Input.GetKeyDown("escape"))
        {
            Screen.lockCursor = false;
        }
        
    }
}
