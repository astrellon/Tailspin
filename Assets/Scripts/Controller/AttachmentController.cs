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
    public Dictionary<string, List<AttachmentController>> AttachmentTypes =
        new Dictionary<string, List<AttachmentController>>();
    public Dictionary<string, List<AttachmentController>> AttachmentGroups =
        new Dictionary<string, List<AttachmentController>>();

    public HardpointController[] Hardpoints {get; protected set;}
    protected List<HardpointController> MountingHardpoints = new List<HardpointController>();

    protected List<PullTogether> PullingTogether = new List<PullTogether>();
    public float PullRadius = 10.0f;

    void Start ()
    {
        AttachmentStart();
    }
    void LateUpdate () 
    {
        AttachmentUpdate();
    }

    protected void AttachmentStart()
    {
        Hardpoints = new HardpointController[0];
        DiscoverHardpoints();
    }


    protected virtual void AttachmentUpdate () 
    {
        bool updateConnected = false;
        for (int i = PullingTogether.Count - 1; i >= 0; i--)
        {
            PullTogether pulling = PullingTogether[i];
            Vector3 toPoints = pulling.AttachmentPoint.transform.position - pulling.Point.transform.position; 
            if (toPoints.magnitude < 1f)
            {
                PullObjectsController puller = pulling.Point.GetComponent<PullObjectsController>();
                if (puller != null)
                {
                    puller.PullSpecific = null;
                }
                Attach(pulling.Point, pulling.Attachment, pulling.AttachmentPoint);
                PullingTogether.RemoveAt(i);
                updateConnected = true;
            }
        }
        if (updateConnected)
        {
            DiscoverConnected();
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

    public void CallAttachmentInterfaceByType(string attachmentType, string funcName, params object[] args)
    {


    }
    public void CallAttachmentInterfaceByGroup(string attachmentGroup, string funcName, params object[] args)
    {
        if (attachmentGroup == null || funcName == null)
        {
            return;
        }

        if (AttachmentGroups.ContainsKey(attachmentGroup))
        {
            foreach (AttachmentController attachment in AttachmentGroups[attachmentGroup])
            {
                attachment.CallAttachInterface(funcName, args);
            }
        }
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
            DiscoverConnected();
            return;
        }
        puller.PullSpecific = pullable;
        pullable.CapturedBy = puller;

        PullingTogether.Add(new PullTogether(point, attachment, attachmentPoint));
    }

    public virtual void RotateConnect(Transform point, Transform attachment, Transform attachmentPoint)
    {
        Quaternion rotateForward = Quaternion.FromToRotation(
                attachmentPoint.forward, point.forward);
        attachment.rotation = rotateForward * attachment.rotation;

        Quaternion rotateUp = Quaternion.FromToRotation(
                point.up * -1, attachmentPoint.up);
        attachment.rotation = rotateUp * attachment.rotation;
    }

    public virtual bool Attach(HardpointController point, AttachmentController attachment, HardpointController attachmentPoint)
    {
        FixedJoint joint = gameObject.AddComponent("FixedJoint") as FixedJoint; 
        // Move into position then attach.
        if (attachment.rigidbody == null)
        {
            //Debug.Log("Cannot attach if it does not have a ridig body!");
            return false;
        }

        RotateConnect(point.transform, attachment.transform, attachmentPoint.transform);

        Vector3 translate = point.transform.position - attachmentPoint.transform.position;
        attachment.transform.Translate(translate, Space.World);

        point.Attached = attachment;
        point.AttachedPoint = attachmentPoint;
        attachmentPoint.Attached = this;
        attachmentPoint.AttachedPoint = point;
        joint.connectedBody = attachment.rigidbody;
        attachment.ParentAttachment = this;

        return true;
    }
    public virtual bool Detach(HardpointController point)
    {
        AttachmentController attachment = point.Attached;
        if (attachment == null)
        {
            return false;
        }
        FixedJoint[] joints = gameObject.GetComponents<FixedJoint>();
        foreach (FixedJoint joint in joints)
        {
            if (joint.connectedBody == attachment.rigidbody)
            {
                joint.connectedBody = null;
                Destroy(joint);
            }
        }

        point.Detach();
        attachment.rigidbody.AddForceAtPosition(point.transform.up * 10, point.transform.position);

        return true;
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
            AttachmentTypes.Clear();
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
                    string type = attachment.AttachmentType;
                    bool newGroup = !AttachmentGroups.ContainsKey(group);
                    bool newType = !AttachmentTypes.ContainsKey(type);
                    if (!OnDiscoverNewAttachment(attachment, newType, newGroup))
                    {
                        continue;
                    }
                    ConnectedAttachments[attachment.GetInstanceID()] = attachment;
                    if (newGroup)
                    {
                        AttachmentGroups[group] = new List<AttachmentController>();
                    }
                    if (newType)
                    {
                        AttachmentTypes[type] = new List<AttachmentController>();
                    }
                    AttachmentGroups[group].Add(attachment);
                    AttachmentTypes[type].Add(attachment);
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
            if (!point.Enabled)
            {
                continue;
            }
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
    protected virtual bool OnDiscoverNewAttachment(AttachmentController obj, bool isNewType, bool isNewGroup)
    {
        return true;
    }

    public virtual void DestroyAttachment()
    {
        ParentAttachment = null;
        foreach (HardpointController hardpoint in MountingHardpoints)
        {
            hardpoint.Detach();
        }
    }
}
