using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HardpointController : MonoBehaviour {

    [System.Flags]
    public enum Type 
    {
        NONE =      0x00,
        LASER =     0x01, 
        VULCAN =    0x02,
        ENGINE =    0x04
    }

    [BitMask(typeof(Type))]
    public Type HardpointType = Type.LASER;

    public bool Enabled = true;

    // Any value greater than 0 indicates priority.
    public int MountingOrder = 0;
    public AttachmentController Attached = null;
    public HardpointController AttachedPoint = null;

    public bool Matches(Type matches, bool isAvailable = true)
    {
        if ((HardpointType & matches) == matches)
        {
            if (isAvailable)
            {
                PullObjectsController puller = transform.GetComponent<PullObjectsController>();
                if (puller != null && puller.PullSpecific != null)
                {
                    return false;
                }

                if (Attached != null)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    public void Detach()
    {
        if (Attached != null)
        {
            Attached.ParentAttachment = null;
            Attached = null;
        }
        if (AttachedPoint != null)
        {
            AttachedPoint.Attached = null;
            AttachedPoint.AttachedPoint = null;
            AttachedPoint = null;
        }
    }

    // Use this for initialization
    void Start () {
    }

    public class MountingComparer : IComparer<HardpointController>
    {
        public int Compare(HardpointController h1, HardpointController h2)
        {
            return h1.MountingOrder.CompareTo(h2.MountingOrder);
        }
    }
}

