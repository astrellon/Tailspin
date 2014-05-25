using UnityEngine;
using System.Collections;

public class DoorChildController : MonoBehaviour {
    void OnCollisionEnter(Collision collision)
    {
        if (transform.parent == null)
        {
            return;
        }
        DoorController door = transform.parent.GetComponent<DoorController>();
        if (door != null)
        {
            door.ChildCollision(collision);
        }
    }
}
