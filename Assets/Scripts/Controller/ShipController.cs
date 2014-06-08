using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : AttachmentController {

    //private Dictionary<HardpointController.Type, List<HardpointController>> Hardpoints = new Dictionary<HardpointController.Type, List<HardpointController>>();
    public GunController CurrentGun;
    public List<GunController> Guns;
    public GameObject ShipDeath;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;
    public float Hull = 10.0f;
    protected List<string> GunGroups = new List<string>();
    public string CurrentGunGroup = null;
	// Use this for initialization
	void Start () {
        /*
        HardpointController[] hardpoints = GetComponentsInChildren<HardpointController>();
        foreach (HardpointController controller in hardpoints)
        {
            HardpointController.Type type = controller.HardpointType;
            if (!Hardpoints.ContainsKey(type))
            {
                Hardpoints[type] = new List<HardpointController>();
            }
            Hardpoints[type].Add(controller);
        }
        */
        DiscoverConnected();
	}

    public override void DiscoverConnected(bool clearCurrent = false)
    {
        base.DiscoverConnected(clearCurrent);
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

    public void MoveDirection(float x, float y, float z)
    {
        rigidbody.AddRelativeForce (x * FlightSpeed, y * FlightSpeed, z * FlightSpeed);
    }
    public void Rotate(float x, float y, float z)
    {
        rigidbody.AddRelativeTorque(x * RotateTorque, y * RotateTorque, z * RotateTorque);
    }

    public void ChangeGun(GunController gun)
    {
        CurrentGun = gun;
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

            if (IsDead() && ShipDeath != null)
            {
                GameObject death = Instantiate(ShipDeath, transform.position, Quaternion.identity) as GameObject;
                death.transform.parent = transform.parent;
                Destroy(transform.gameObject);
            }
        }
    }
}
