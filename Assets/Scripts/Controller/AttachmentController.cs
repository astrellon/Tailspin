using UnityEngine;
using System;
using System.Collections.Generic;

public class AttachmentController : MonoBehaviour {

    public class PullTogether {
        public HardpointController Point { get; private set; }
        public AttachmentController Attachment { get; private set; }
        public PullableController AttachmentPullable { get; private set; }
        public HardpointController AttachmentPoint { get; private set; }

        public PullTogether(HardpointController point, AttachmentController attachment, HardpointController attachmentPoint)
        {
            Point = point;
            Attachment = attachment;
            AttachmentPoint = attachmentPoint;
            AttachmentPullable = attachment.GetComponent<PullableController>();
        }
    }

    protected Dictionary<int, AttachmentController> ConnectedAttachments = new Dictionary<int, AttachmentController>();

    public delegate object AttachFunction(params object[] args);

    public AttachmentController ParentAttachment = null;

    public string AttachmentType = "none";
    public string AttachmentGroup = "group";

    public Dictionary<string, AttachFunction> AttachInterface = 
        new Dictionary<string, AttachFunction>();
    public Dictionary<string, List<AttachmentController>> AttachmentGroups =
        new Dictionary<string, List<AttachmentController>>();

    //public Dictionary<HardpointController.Type, List<HardpointController>> Hardpoints = new Dictionary<HardpointController.Type, List<HardpointController>>();
    //public List<HardpointController> Hardpoints = new List<HardpointController>();
    public HardpointController[] Hardpoints = new HardpointController[0];
    public List<HardpointController> MountingHardpoints = new List<HardpointController>();

    public List<PullTogether> PullingTogether = new List<PullTogether>();
    public float PullRadius = 10.0f;

    void Start ()
    {
        AttachmentStart();
    }
    void Update () 
    {
        AttachmentUpdate();
    }

    protected void AttachmentStart()
    {
        DiscoverHardpoints();
    }


    protected virtual void AttachmentUpdate () 
    {
        for (int i = PullingTogether.Count - 1; i >= 0; i--)
        {
            PullTogether pulling = PullingTogether[i];
            Vector3 toPoints = pulling.AttachmentPoint.transform.position - pulling.Point.transform.position; 
            if (toPoints.magnitude < 0.1f)
            {
                Attach(pulling.Point, pulling.Attachment, pulling.AttachmentPoint);
                PullingTogether.RemoveAt(i);
                DiscoverConnected();
            }
        }
    }

    public object CallAttachInterface(string funcName, params object[] args)
    {
        if (AttachInterface.ContainsKey(funcName))
        {
            return AttachInterface[funcName](args);
        }
        return false;
    }

    public Quaternion CalcAttach(Transform point, Transform attachment)
    {
        Quaternion rotateForward = Quaternion.FromToRotation(
                point.forward, attachment.forward * -1);

        Quaternion rotateUp = Quaternion.FromToRotation(
                attachment.up* -1, point.up);

        return rotateUp * rotateForward;
    }

    public virtual void PullAndAttach(HardpointController point, AttachmentController attachment, HardpointController attachmentPoint)
    {
        PullableController pullable = attachment.GetComponent<PullableController>();
        if (pullable == null)
        {
            Debug.Log("Cannot pull object that is not pullable.");
            return;
        }


        PullObjectsController puller = point.GetComponent<PullObjectsController>();
        if (puller == null)
        {
            //Debug.Log("Cannot pull and attach as there is no puller on the point.");
            Attach(point, attachment, attachmentPoint);
            return;
        }
        puller.PullSpecific = pullable;
        pullable.CapturedBy = puller;

        PullingTogether.Add(new PullTogether(point, attachment, attachmentPoint));
    }

    public virtual void Attach(HardpointController point, AttachmentController attachment, HardpointController attachmentPoint)
    {
        FixedJoint joint = gameObject.AddComponent("FixedJoint") as FixedJoint; 
        // Move into position then attach.
        if (attachment.rigidbody == null)
        {
            //Debug.Log("Cannot attach if it does not have a ridig body!");
            return;
        }

        Quaternion rotate = CalcAttach(point.transform, attachmentPoint.transform);
        attachment.transform.rotation = rotate * attachment.transform.rotation;

        Vector3 translate = point.transform.position - attachmentPoint.transform.position;
        attachment.transform.Translate(translate, Space.World);

        point.Attached = attachment;
        attachmentPoint.Attached = this;
        joint.connectedBody = attachment.rigidbody;
        attachment.ParentAttachment = this;
    }

    public virtual void DiscoverHardpoints()
    {
        Hardpoints = GetComponentsInChildren<HardpointController>();

        MountingHardpoints.Clear();
        foreach (HardpointController hardpoint in Hardpoints)
        {
            if (hardpoint.MountingOrder > 0)
            {
                MountingHardpoints.Add(hardpoint);
            }
        }

        HardpointController.MountingComparer compare = new HardpointController.MountingComparer();
        MountingHardpoints.Sort(compare);
        //MountingHardpoints = MountingHardpoints.OrderBy(h=>h.MountingOrder).ToList();
    }

    public virtual void DiscoverConnected(bool clearCurrent = false)
    {
        if (clearCurrent)
        {
            ConnectedAttachments.Clear();
            AttachmentGroups.Clear();
        }

        Joint[] connected = transform.GetComponentsInChildren<Joint>();
        for (int i = 0; i < connected.Length; i++)
        {
            Rigidbody other = connected[i].connectedBody;
            if (other != null)
            {
                //DiscoverConnected(other.transform);
                AttachmentController []attachments = other.transform.GetComponentsInChildren<AttachmentController>();
                for (int j = 0; j < attachments.Length; j++)
                {
                    AttachmentController attachment = attachments[j];
                    string group = attachment.AttachmentGroup;
                    bool newGroup = !AttachmentGroups.ContainsKey(group);
                    if (!OnDiscoverNewAttachment(attachment, newGroup))
                    {
                        continue;
                    }
                    ConnectedAttachments[attachment.GetInstanceID()] = attachment;
                    if (newGroup)
                    {
                        AttachmentGroups[group] = new List<AttachmentController>();
                    }
                    AttachmentGroups[group].Add(attachment);
                }
            }
        }
    }

    public virtual IList<AttachmentController> DiscoverNearbyAttachable()
    {
        List<AttachmentController> nearby = new List<AttachmentController>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, PullRadius);
        foreach (Collider collider in hitColliders)
        {
            PullableController pullable = collider.transform.GetComponent<PullableController>();
            if (pullable == null || pullable.CapturedBy != null)
            {
                //Debug.Log("Collider " + collider.name + " is not pullable");
                continue;
            }
            
            AttachmentController attachment = collider.transform.GetComponent<AttachmentController>();
            if (attachment == null)
            {
                //Debug.Log("Collider " + collider.name + " has no attachment");
                continue;
            }

            HardpointController mounting = attachment.FindMountingHardpoint();
            if (mounting == null)
            {
                //Debug.Log("Attachment " + collider.name + " has not mounting point");
                continue;
            }

            if (mounting.Attached != null)
            {
                //Debug.Log("Attachment " + collider.name + " already mounted");
                continue;
            }

            nearby.Add(attachment);
        }
        return nearby;
    }

    public virtual HardpointController GetAvailablePoint(AttachmentController forAttachment)
    {
        /*
        if (Hardpoints.ContainsKey(HardpointController.Type.LASER))
        {
            List<HardpointController> list = Hardpoints[HardpointController.Type.LASER];
            if (list != null)
            {
                foreach (HardpointController hardpoint in list)
                {
                    PullObjectsController puller = hardpoint.GetComponent<PullObjectsController>();
                    if (puller != null && puller.PullSpecific != null)
                    {
                        continue;
                    }

                    if (hardpoint.Attached != null)
                    {
                        continue;
                    }

                    return hardpoint;
                }
            }
        }
        */
        return null;
    }

    public virtual HardpointController FindMountingHardpoint(HardpointController.Type matches = HardpointController.Type.NONE, bool isAvailable = true)
    {
        foreach (HardpointController point in MountingHardpoints)
        {
            if (point.Matches(matches, isAvailable))
            {
                return point; 
            }
        }
        return null;
    }
    public virtual HardpointController FindHardpointWithType(HardpointController.Type matches, bool isAvailable = true)
    {
        foreach (HardpointController point in Hardpoints)
        {
            if (point.Matches(matches, isAvailable))
            {
                return point;
            }
        }
        return null;
    }
    public virtual IList<HardpointController> FindAllHardpointsWithType(HardpointController.Type matches)
    {
        List<HardpointController> result = new List<HardpointController>();
        foreach (HardpointController point in Hardpoints)
        {
            if ((point.HardpointType & matches) == matches)
            {
                result.Add(point);
            }
        }
        return result;
    }

    // Called when new attachments are found, returning false will prevent the 
    // attachment from being attached.
    protected virtual bool OnDiscoverNewAttachment(AttachmentController obj, bool isNewGroup)
    {
        return true;
    }
}
