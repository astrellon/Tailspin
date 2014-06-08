using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HardpointController : MonoBehaviour {

    public enum Type {
        MOUNTING, LASER, LASER_2, VULCAN
    }
    public Type HardpointType = Type.LASER;

    public AttachmentController Attached = null;

    // Use this for initialization
    void Start () {
    }
}

