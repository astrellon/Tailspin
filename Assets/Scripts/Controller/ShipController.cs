using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

    private float nextFire = 0.0f;
    private List<Transform> HardpointsLaser = new List<Transform>();
    private List<Transform> HardpointsLaser2 = new List<Transform>();
    public GunController CurrentGun;
    public GunController Gun1;
    public GunController Gun2;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;

	// Use this for initialization
	void Start () {
        HardpointsLaser.Add(transform.Find("HardpointLeft"));
        HardpointsLaser.Add(transform.Find("HardpointRight"));

        HardpointsLaser2.AddRange(HardpointsLaser);
        HardpointsLaser2.Add(transform.Find("HardpointLeft2"));
        HardpointsLaser2.Add(transform.Find("HardpointRight2"));
	}

    public void FireGun(GunController gun, IList<Transform> hardpoints)
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
            IList<Transform> hardpoints = HardpointsLaser;
            if (CurrentGun.GunType == "Laser2")
            {
                hardpoints = HardpointsLaser2;
            }
            FireGun(CurrentGun, hardpoints);
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
}
