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

    public Dictionary<HardpointController.Type, List<HardpointController>> Hardpoints = new Dictionary<HardpointController.Type, List<HardpointController>>();

    public List<PullTogether> PullingTogether = new List<PullTogether>();

    void Start () {
        DiscoverHardpoints();
    }

    void Update () {
        Debug.Log("Togeth? " + PullingTogether.Count);
        foreach (PullTogether pulling in PullingTogether)
        {
            Vector3 toPoints = pulling.AttachmentPoint.transform.position - pulling.Point.transform.position; 
            Debug.Log("Distance: " + toPoints.magnitude);
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
            Debug.Log("Cannot pull and attach as there is no puller on the point.");
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
            Debug.Log("Cannot attach if it does not have a ridig body!");
            return;
        }

        Quaternion rotate = CalcAttach(point.transform, attachmentPoint.transform);
        attachment.transform.rotation = rotate * attachment.transform.rotation;

        Vector3 translate = point.transform.position - attachmentPoint.transform.position;
        attachment.transform.Translate(translate, Space.World);

        joint.connectedBody = attachment.rigidbody;
    }

    public virtual void DiscoverHardpoints()
    {
        Hardpoints.Clear();

        HardpointController []points = GetComponentsInChildren<HardpointController>();
        foreach (HardpointController point in points)
        {
            HardpointController.Type type = point.HardpointType;
            if (!Hardpoints.ContainsKey(type))
            {
                Hardpoints[type] = new List<HardpointController>();
            }
            Hardpoints[type].Add(point);
        }
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

    // Called when new attachments are found, returning false will prevent the 
    // attachment from being attached.
    protected virtual bool OnDiscoverNewAttachment(AttachmentController obj, bool isNewGroup)
    {
        return true;
    }
}
