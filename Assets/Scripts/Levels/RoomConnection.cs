using UnityEngine;
using System.Collections;

public class RoomConnection : MonoBehaviour {


    public Room ParentRoom;
	// Use this for initialization
	void Start () {
        ParentRoom = transform.parent.GetComponent<Room>();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
