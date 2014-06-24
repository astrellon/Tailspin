using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineController : AttachmentController {
    //public ParticleSystem Trail = null;
    public GameObject TrailPrefab = null;
    protected List<GameObject> Trails = new List<GameObject>();

    public float MaxThrust = 1000.0f;
    public float Thrust = 750.0f;

    void Start() 
    {
        AttachmentStart();
        SetThrust(Thrust);
        IList<HardpointController> enginePoints =
            FindAllHardpointsWithType(HardpointController.Type.ENGINE);
        CreateEngines(enginePoints);
    }

    protected virtual void CreateEngines(IList<HardpointController> hardpoints)
    {
        if (TrailPrefab == null)
        {
            return;
        }

        foreach (HardpointController hardpoint in hardpoints)
        {
            GameObject trail = Instantiate(TrailPrefab,
                    hardpoint.transform.position, hardpoint.transform.rotation) as GameObject;
            trail.transform.parent = hardpoint.transform;
            Trails.Add(trail);
        }
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.U))
        {
            SetThrust(Thrust + MaxThrust * 0.05f);
        }
        if (Input.GetKey(KeyCode.J))
        {
            SetThrust(Thrust - MaxThrust * 0.05f);
        }

        if (rigidbody != null)
        {
            foreach (GameObject trail in Trails)
            {
                Vector3 force = trail.transform.forward * -Thrust;
                if (ParentAttachment != null && ParentAttachment.rigidbody != null)
                {
                    ParentAttachment.rigidbody.AddForce(force);
                }
            }
        }
    }

    protected override bool OnDetach()
    {
        SetThrust(0);
        return true;
    }

    public void SetThrust(float thrust)
    {
        Thrust = thrust;
        if (Thrust < 0.0f)
        {
            Thrust = 0.0f;
        }
        else if (Thrust > MaxThrust)
        {
            Thrust = MaxThrust;
        }

        foreach (GameObject trail in Trails)
        {
            if (trail.animation == null)
            {
                continue;
            }
            foreach (AnimationState state in trail.animation)
            {
                state.time = Thrust / MaxThrust;
                state.enabled = true;
                trail.animation.Sample();
                state.enabled = false;
            }
        }
    }
}
