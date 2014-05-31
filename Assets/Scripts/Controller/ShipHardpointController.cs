using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipHardpointController : MonoBehaviour {

    public enum Type {
        LASER, LASER_2, VULCAN
    }
    public Type HardpointType = Type.LASER;

    // Use this for initialization
    void Start () {
    }
}

