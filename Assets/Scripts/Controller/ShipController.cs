using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

    private float nextFire = 0.0f;
    private Dictionary<ShipHardpointController.Type, List<ShipHardpointController>> Hardpoints = new Dictionary<ShipHardpointController.Type, List<ShipHardpointController>>();
    public GunController CurrentGun;
    public List<GunController> Guns;
    public GameObject ShipDeath;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;
    public float Hull = 10.0f;

	// Use this for initialization
	void Start () {
        ShipHardpointController[] hardpoints = GetComponentsInChildren<ShipHardpointController>();
        foreach (ShipHardpointController controller in hardpoints)
        {
            ShipHardpointController.Type type = controller.HardpointType;
            if (!Hardpoints.ContainsKey(type))
            {
                Hardpoints[type] = new List<ShipHardpointController>();
            }
            Hardpoints[type].Add(controller);
        }
	}

    public void FireGun(GunController gun, params List<ShipHardpointController>[] hardpoints)
    {
        if (gun == null || nextFire > Time.time)
        {
            return;
        }
        gun.Fire(hardpoints);
        nextFire = Time.time + gun.Cooldown;
    }
    public void FireGun()
    {
        if (CurrentGun != null)
        {
            List<ShipHardpointController> hardpoints = Hardpoints[ShipHardpointController.Type.LASER];
            if (CurrentGun.GunType == "Laser2")
            {
                FireGun(CurrentGun, hardpoints, Hardpoints[ShipHardpointController.Type.LASER_2]);
            }
            else
            {
                FireGun(CurrentGun, hardpoints);
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
