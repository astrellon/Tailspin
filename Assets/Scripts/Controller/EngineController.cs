using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineController : AttachmentController {
    //public ParticleSystem Trail = null;
    public GameObject TrailPrefab = null;
    protected List<GameObject> Trails = new List<GameObject>();

    public float MaxThrust = 1000.0f;
    public float Thrust = 0.0f;
    public float ThrustPowerUp = 1000.0f;
    public float ThrustPowerDown = 2000.0f;
    public float CurrentThrust {get; protected set;}

    void Start() 
    {
        AttachmentStart();
        IList<HardpointController> enginePoints =
            FindAllHardpointsWithType(HardpointController.Type.ENGINE);
        CreateEngines(enginePoints);
        SetThrust(Thrust);

        AttachInterface["SetThrust"] = SetThrustInterface;
        AttachInterface["GetThrust"] = GetThrustInterface;
        AttachInterface["ChangeThrust"] = ChangeThrustInterface;
    }

    public object ChangeThrustInterface(params object[] arg)
    {
        if (arg.Length > 0)
        {
            float deltaThrust = ParseThrust(arg[0]);
            if (deltaThrust < 0.0f)
            {
                return Thrust;
            }
            Thrust += deltaThrust;
        }
        return Thrust;
    }
    public object GetThrustInterface(params object[] arg)
    {
        if (arg.Length > 0)
        {
            if (arg[0] is bool && (bool)arg[0])
            {
                float percent = (100.0f * Thrust / MaxThrust);
                return percent;
            }
        }
        return Thrust;
    }
    public object SetThrustInterface(params object[] arg)
    {
        if (arg.Length > 0)
        {
            float thrust = ParseThrust(arg[0]);

            if (thrust < 0.0f)
            {
                return false;
            }
            Thrust = thrust;
            return true;
        }
        return false;
    }

    public float ParseThrust(object arg)
    {
        float thrust = 0.0f;
        if (arg is string)
        {
            thrust = ParseThrustString(arg as string);
        }
        else
        {
            thrust = (float)arg;
        }
        return thrust;
    }
    public float ParseThrustString(string str)
    {
        str = str.Trim();
        if (str.Length == 0)
        {
            return 0.0f;
        }
        if (str[str.Length - 1] == '%')
        {
            str = str.Substring(0, str.Length - 1);
            float percent = 0.0f;
            if (!float.TryParse(str, out percent))
            {
                return -1.0f;
            }
            return MaxThrust * percent * 0.01f;
        }
        float result = 0.0f;
        if (!float.TryParse(str, out result))
        {
            return -1.0f;
        }
        return result;
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

    void FixedUpdate ()
    {
        if (rigidbody != null)
        {
            foreach (GameObject trail in Trails)
            {
                Vector3 force = trail.transform.forward * -CurrentThrust;
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

    void Update ()
    {
        UpdateThrust();
    }

    void UpdateThrust()
    {
        if (CurrentThrust != Thrust)
        {
            if (CurrentThrust > Thrust)
            {
                float dt = ThrustPowerDown * Time.deltaTime;
                CurrentThrust -= dt;
                if (CurrentThrust < Thrust)
                {
                    CurrentThrust = Thrust;
                }
            }
            else
            {
                float dt = ThrustPowerUp * Time.deltaTime;
                CurrentThrust += dt;
                if (CurrentThrust > Thrust)
                {
                    CurrentThrust = Thrust;
                }
            }
            SetThrust(CurrentThrust);
        }
    }

    public void SetThrust(float thrust)
    {
        if (thrust < 0.0f)
        {
            thrust = 0.0f;
        }
        else if (thrust > MaxThrust)
        {
            thrust = MaxThrust;
        }

        float time = thrust / MaxThrust;
        foreach (GameObject trail in Trails)
        {
            if (trail.animation == null)
            {
                continue;
            }
            foreach (AnimationState state in trail.animation)
            {
                state.time = time;
                state.enabled = true;
                trail.animation.Sample();
                state.enabled = false;
            }
        }
    }
}
