using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateRoom : MonoBehaviour {

    public List<Room> UseRooms = new List<Room>();
    public int MaxNumberRooms = 5;
    protected System.Random rander = new System.Random();
    protected Queue<RoomConnection> AvailableConnections = new Queue<RoomConnection>();
    protected List<Room> BuiltRooms = new List<Room>();
    protected List<Collider> BuiltColliders = new List<Collider>();
    protected int BuildAttempts = 0;
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
        while (AvailableConnections.Count > 0 && BuiltRooms.Count < MaxNumberRooms && BuildAttempts < 200)
        {
            BuildAttempts++;
            // Connection from the available connections.
            RoomConnection sourceConnection = AvailableConnections.Dequeue();
            Room newRoom = CreateRoom();
            // Connection from the room to use
            RoomConnection destConnection = PickConnection(newRoom);

            // Create a rotation Quaternion that will perform the rotation
            // required to make the two normals align.
            Quaternion rotate = Quaternion.FromToRotation(
                destConnection.transform.forward, sourceConnection.transform.forward * -1);
            // Apply the rotation to the room, this will make the normals of
            // the attachment points align even if the rooms own world rotation
            // is point in some weird direction.
            newRoom.transform.rotation = rotate * newRoom.transform.rotation;

            rotate = Quaternion.FromToRotation(
                sourceConnection.transform.up * -1, destConnection.transform.up);
            newRoom.transform.rotation = rotate * newRoom.transform.rotation;
            Vector3 translate = sourceConnection.transform.position - destConnection.transform.position;
            newRoom.transform.Translate(translate, Space.World);
        
            if (RoomCollides(newRoom))
            {
                AvailableConnections.Enqueue(sourceConnection);
                Destroy(newRoom.gameObject);
                continue;
            }
            sourceConnection.OtherRoomConnection = destConnection;
            BuiltRooms.Add(newRoom);
            if (newRoom.RoomCollider != null)
            {
                BuiltColliders.Add(newRoom.RoomCollider);
            }
            else if (newRoom.collider != null)
            {
                BuiltColliders.Add(newRoom.collider);
            }

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
    protected bool RoomCollides(Room checkRoom)
    {
        Collider checkCollider = checkRoom.RoomCollider;
        if (checkCollider == null)
        {
            checkCollider = checkRoom.collider;
        }
        if (checkCollider == null)
        {
            return false;
        }

        Bounds checkBounds = checkCollider.bounds;
        checkBounds.Expand(-0.2f);
        foreach (Collider collida in BuiltColliders)
        {
            Bounds bounds = collida.bounds;
            if (checkBounds.Intersects(bounds))
            {
                //Debug.Log(checkRoom.name + " intersects with " + collida.name);
                return true;
            }
        }
        return false;
    }

    protected Room CreateRoom()
    {
        Room room = PickRoom();
        Room newRoom = Instantiate(room, transform.position, Quaternion.identity) as Room;
        newRoom.GetConnections();
        newRoom.transform.parent = transform;
        newRoom.name += "_" + BuildAttempts;
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
