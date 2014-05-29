using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateRoom : MonoBehaviour {

    public List<Room> UseRooms = new List<Room>();
    public int MaxNumberRooms = 5;
    protected System.Random rander = new System.Random();
    protected Queue<RoomConnection> AvailableConnections = new Queue<RoomConnection>();
    protected int BuiltRooms = 0;
	// Use this for initialization
	void Start () {
        Generate();
        /*
        Vector3 v1 = new Vector3(1, 0, 0);
        Vector3 v2 = new Vector3(-1, 0, 0);
        Debug.Log("Angle: " + Vector3.Angle(v1,v2));
        Quaternion q = Quaternion.FromToRotation(v1, v2);
        Vector3 v3 = q * v1;
        Vector3 v4 = q * v2;
        Debug.Log("V3: " + v3);
        Debug.Log("V4: " + v4);
        */
	}

    public Room PickRoom()
    {
        int index = rander.Next(UseRooms.Count);
        return UseRooms[index];

    }

    public void Generate()
    {
        Room newRoom = CreateRoom();
        Debug.Log("Generated room connections: " + newRoom.Connections.Length);
        foreach (RoomConnection connection in newRoom.Connections)
        {
            AvailableConnections.Enqueue(connection);
        }
        ProcessConnections();
    }
    protected void ProcessConnections()
    {
        while (AvailableConnections.Count > 0 && BuiltRooms < MaxNumberRooms)
        {
            // Connection from the available connections.
            RoomConnection sourceConnection = AvailableConnections.Dequeue();
            Room newRoom = CreateRoom();
            // Connection from the room to use
            RoomConnection destConnection = PickConnection(newRoom);
            sourceConnection.OtherRoomConnection = destConnection;

            /*
            Vector3 origForward = destConnection.transform.forward;
            Vector3 origRight = destConnection.transform.right;
            Vector3 origUp = destConnection.transform.up;

            Quaternion rotateForward = Quaternion.FromToRotation(origForward, sourceConnection.transform.forward * -1);
            Quaternion rotateRight = Quaternion.FromToRotation(origRight, sourceConnection.transform.right * -1);
            Quaternion rotateUp = Quaternion.FromToRotation(origUp, sourceConnection.transform.up);
            newRoom.transform.forward = rotateForward * origForward;
            newRoom.transform.right = rotateRight * origForward;
            newRoom.transform.up = rotateUp * origUp;
            Debug.Log("After rotation: " + destConnection.transform.position);
            */
            //newRoom.transform.forward = rotate * newRoom.transform.forward;
            //newRoom.transform.up = new Vector3(0, 1, 0);
            /*
            float diffAngle = Vector3.Angle(destConnection.transform.forward, sourceConnection.transform.forward * -1);
            Debug.Log("Diff Angle: " + diffAngle);
            newRoom.transform.RotateAround(destConnection.transform.position, Vector3.up, -diffAngle);
            diffAngle = Vector3.Angle(destConnection.transform.forward, sourceConnection.transform.forward * -1);
            Debug.Log("After Diff Angle: " + diffAngle);
            */
            Debug.Log("Before source position: " + sourceConnection.transform.position);
            Debug.Log("Before position: " + destConnection.transform.position);
            Quaternion rotate = Quaternion.FromToRotation(destConnection.transform.forward, sourceConnection.transform.forward * -1);
            newRoom.transform.rotation = rotate * newRoom.transform.rotation;
            Debug.Log("After position: " + destConnection.transform.position);
            Vector3 sourcePoint = sourceConnection.transform.position;
            Vector3 translate = sourcePoint - destConnection.transform.position;
            newRoom.transform.Translate(translate, Space.World);
            Debug.Log("Translate: " + translate);
            Debug.Log("Compared: " + destConnection.transform.position + " | " + sourceConnection.transform.position);
            /*
            newRoom.transform.RotateAround(sourcePoint, sourceConnection.transform.up, 180);
            */
            //Debug.Log("Source location: " + sourceConnection.transform.position);
            //Debug.Log("Destination location: " + destConnection.transform.localPosition);

            // Add all connections from the new room into the queue of available
            // rooms except the connection that's about the conencted to.
            foreach (RoomConnection connection in newRoom.Connections)
            {
                if (destConnection == connection)
                {
                    continue;
                }
                AvailableConnections.Enqueue(connection);
            }

            
        }
        Complete();
    }
    protected Room CreateRoom()
    {
        Room room = PickRoom();
        BuiltRooms++;
        Room newRoom = Instantiate(room, transform.position, Quaternion.identity) as Room;
        newRoom.GetConnections();
        newRoom.transform.parent = transform;
        return newRoom;
    }
    protected RoomConnection PickConnection(Room room)
    {
        if (room.Connections.Length == 0)
        {
            return null;
        }
        int index = rander.Next(room.Connections.Length);
        return room.Connections[index];
    }

    protected void Complete()
    {

    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
