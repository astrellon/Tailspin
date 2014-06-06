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

            // Create a rotation Quaternion that will perform the rotation
            // required to make the two normals align.
            Quaternion rotate = Quaternion.FromToRotation(
                destConnection.transform.forward, sourceConnection.transform.forward * -1);
            // Apply the rotation to the room, this will make the normals of
            // the attachment points align even if the rooms own world rotation
            // is point in some weird direction.
            newRoom.transform.rotation = rotate * newRoom.transform.rotation;

            rotate = Quaternion.FromToRotation(
                sourceConnection.transform.up* -1, destConnection.transform.up);
            newRoom.transform.rotation = rotate * newRoom.transform.rotation;
            Vector3 translate = sourceConnection.transform.position - destConnection.transform.position;
            newRoom.transform.Translate(translate, Space.World);

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
