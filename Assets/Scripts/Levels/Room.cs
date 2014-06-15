using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room : MonoBehaviour {

    public RoomConnection[] Connections = null;
    private bool CreatedConnections = false;
    public Collider RoomCollider = null;

	// Use this for initialization
	void Start () {
        GetConnections();
	}

    public RoomConnection[] GetConnections() {
        if (!CreatedConnections)
        {
            Connections = GetComponentsInChildren<RoomConnection>();
            CreatedConnections = true;
        }

        return Connections;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
