using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : AttachmentController {

    public GameObject ShipDeath;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;
    public float Hull = 10.0f;
    public float MaxHull = 10.0f;
    public float Shields = 10.0f;
    public float MaxShields = 10.0f;
    public float ShieldRegen = 1.0f;
    public float ShieldWaitTime = 3.0f;
    private float LastHitTime = 0.0f;
    protected List<string> GunGroups = new List<string>();
    public string CurrentGunGroup = null;
    protected Dictionary<int, AttachmentController> IgnoreAttachments = new Dictionary<int, AttachmentController>();
    public FlightComputerController FlightComputer = null;
	// Use this for initialization
	void Start () 
    {
        DiscoverConnected();
	}
    
    private int Counter = 0;

    public override void DiscoverConnected(bool clearCurrent = false)
    {
        base.DiscoverConnected(clearCurrent);
    }

    void Update()
    {
        ShipUpdate();
    }

    private static AttachmentController[] EmptyAttachments = new AttachmentController[0];

    public IList<AttachmentController> DiscoverNearbyAttachable(float distance)
    {
        SensorController sensors = GetComponent<SensorController>();
        if (!sensors)
        {
            return EmptyAttachments;
        }

        float dist2 = distance * distance;

        List<AttachmentController> result = new List<AttachmentController>();
        IDictionary<int, SensorController.Entry> sensorEntries = sensors.GetEntries();
        foreach (KeyValuePair<int, SensorController.Entry> pair in sensorEntries)
        {
            if (!pair.Value.Visible)
            {
                continue;
            }

            AttachmentController attachment = pair.Value.Object;
            Vector4 pos = attachment.transform.position;
            if (Vector4.Dot(pos, pos) > dist2)
            {
                continue;
            }

            result.Add(attachment);
        }
        return result;
    }

    public override bool Attach(HardpointController point, AttachmentController attachment, HardpointController attachmentPoint)
    {
        bool result = base.Attach(point, attachment, attachmentPoint);
        if (result)
        {
            if (FlightComputer != null)
            {
                EngineController engine = attachment as EngineController;
                if (engine != null)
                {
                    FlightComputer.AddEngine(engine);
                }
            }
        }
        return result;
    }

    protected void ShipUpdate()
    {
        if (Shields < MaxShields)
        {
            Debug.Log("Hit times: " + LastHitTime + ", " + ShieldWaitTime + " | " + Time.time);
            if (LastHitTime + ShieldWaitTime < Time.time)
            {
                Shields += ShieldRegen * Time.deltaTime;
                if (Shields > MaxShields)
                {
                    Shields = MaxShields;
                }
            }
        }
        Counter++;
        if (Counter > 10)
        {
            IList<AttachmentController> nearby = DiscoverNearbyAttachable();
            if (nearby.Count > 0)
            {
                foreach (AttachmentController attachment in nearby)
                {
                    if (IgnoreAttachments.ContainsKey(attachment.GetInstanceID()))
                    {
                        continue;
                    }

                    HardpointController attachMounting = attachment.FindMountingHardpoint();
                    if (attachMounting == null)
                    {
                        continue;
                    }

                    HardpointController mounting = FindMountingHardpoint(attachMounting.HardpointType, true);
                    if (mounting == null)
                    {
                        continue;
                    }

                    PullAndAttach(mounting, attachment, attachMounting);
                }
            }
            Counter = 0;
        }
    }

    public void FireGun()
    {
        if (CurrentGunGroup == null)
        {
            return;
        }

        if (AttachmentGroups.ContainsKey(CurrentGunGroup))
        {
            foreach (AttachmentController gun in AttachmentGroups[CurrentGunGroup])
            {
                gun.CallAttachInterface("Fire");
            }
        }
    }

    public virtual bool Detach(HardpointController point, bool ignore)
    {
        AttachmentController attachment = point.Attached;
        if (Detach(point))
        {
            if (ignore)
            {
                IgnoreAttachments[attachment.GetInstanceID()] = attachment;
            }
            return true;
        }
        return false;
    }

    private Vector4 MoveAccum = new Vector4();
    private Vector4 RotateAccum = new Vector4();
    public void MoveDirection(float x, float y, float z)
    {
        MoveAccum.x += x;
        MoveAccum.y += y;
        MoveAccum.z += z;
    }
    public void Rotate(float x, float y, float z)
    {
        RotateAccum.x += x;
        RotateAccum.y += y;
        RotateAccum.z += z;
    }

    public bool IsDead()
    {
        return Hull <= 0.0f;
    }
    public void DealDamage(float damage)
    {
        if (!IsDead())
        {
            if (Shields > 0.0f)
            {
                Shields -= damage;
            }
            else 
            {
                Hull -= damage;
            }
            LastHitTime = Time.time;

            if (IsDead())
            {
                if (ShipDeath != null)
                {
                    GameObject death = Instantiate(ShipDeath, transform.position, Quaternion.identity) as GameObject;
                    death.transform.parent = transform.parent;
                }
                DestroyAttachment();
                Destroy(transform.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        rigidbody.AddRelativeForce (MoveAccum.x * FlightSpeed, MoveAccum.y * FlightSpeed, MoveAccum.z * FlightSpeed);
        rigidbody.AddRelativeTorque(RotateAccum.x * RotateTorque, RotateAccum.y * RotateTorque, RotateAccum.z * RotateTorque);
        MoveAccum.x = MoveAccum.y = MoveAccum.z = 0.0f;
        RotateAccum.x = RotateAccum.y = RotateAccum.z = 0.0f;
    }
}
