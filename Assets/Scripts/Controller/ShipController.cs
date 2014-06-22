using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : AttachmentController {

    public GameObject ShipDeath;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;
    public float Hull = 10.0f;
    protected List<string> GunGroups = new List<string>();
    public string CurrentGunGroup = null;
    protected Dictionary<int, AttachmentController> IgnoreAttachments = new Dictionary<int, AttachmentController>();
	// Use this for initialization
	void Start () {
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

    protected void ShipUpdate()
    {
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

                    Debug.Log("Attach mounting: " + attachMounting.HardpointType);
                    HardpointController mounting = FindMountingHardpoint(attachMounting.HardpointType);
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

    public void MoveDirection(float x, float y, float z)
    {
        rigidbody.AddRelativeForce (x * FlightSpeed, y * FlightSpeed, z * FlightSpeed);
    }
    public void Rotate(float x, float y, float z)
    {
        rigidbody.AddRelativeTorque(x * RotateTorque, y * RotateTorque, z * RotateTorque);
    }

    public bool IsDead()
    {
        return Hull <= 0.0f;
    }
    public void DealDamage(float damage)
    {
        if (!IsDead())
        {
            Hull -= damage;
            Debug.Log("Hull now at: " + Hull);

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
}
