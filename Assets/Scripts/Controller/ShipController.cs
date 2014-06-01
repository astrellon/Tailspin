using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour {

    private float NextFire = 0.0f;
    private Dictionary<HardpointController.Type, List<HardpointController>> Hardpoints = new Dictionary<HardpointController.Type, List<HardpointController>>();
    public GunController CurrentGun;
    public List<GunController> Guns;
    public GameObject ShipDeath;
    public float FlightSpeed = 15.0f;
    public float RotateTorque = 1.0f;
    public float Hull = 10.0f;

    protected Dictionary<int, Transform> ConnectedTransforms = new Dictionary<int, Transform>();
    protected Dictionary<int, HardpointController> ConnectedHardpoints = new Dictionary<int, HardpointController>();

	// Use this for initialization
	void Start () {
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
	}

    public void FireGun(GunController gun, params List<HardpointController>[] hardpoints)
    {
        if (gun == null || NextFire > Time.time)
        {
            return;
        }
        gun.Fire(hardpoints);
        NextFire = Time.time + gun.Cooldown;
    }
    public void FireGun()
    {
        if (CurrentGun != null)
        {
            List<HardpointController> hardpoints = Hardpoints[HardpointController.Type.LASER];
            if (CurrentGun.GunType == "Laser2")
            {
                FireGun(CurrentGun, hardpoints, Hardpoints[HardpointController.Type.LASER_2]);
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

    public void DiscoverConnected(bool clearCurrent = false)
    {
        if (clearCurrent)
        {
            ConnectedTransforms.Clear();
        }
        DiscoverConnected(transform);
    }
    protected void DiscoverConnected(Transform obj)
    {
        if (ConnectedTransforms.ContainsKey(obj.GetInstanceID()))
        {
            return;
        }
        ConnectedTransforms[obj.GetInstanceID()] = obj;

        HardpointController[] points = obj.GetComponentsInChildren<HardpointController>();
        for (int i = 0; i < points.Length; i++)
        {
            ConnectedHardpoints[points[i].GetInstanceID()] = points[i];
        }

        Joint[] connected = obj.GetComponentsInChildren<Joint>();
        for (int i = 0; i < connected.Length; i++)
        {
            Rigidbody other = connected[i].connectedBody;
            if (other != null)
            {
                DiscoverConnected(other.transform);
            }
        }
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
