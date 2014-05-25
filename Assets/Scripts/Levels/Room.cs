using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room : MonoBehaviour {

    public RoomConnection[] Connections = null;
	// Use this for initialization
	void Start () {
        Connections = GetComponents<RoomConnection>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
