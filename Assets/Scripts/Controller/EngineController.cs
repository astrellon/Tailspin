using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineController : AttachmentController {
    public ParticleSystem Trail = null;

    public float MaxThrust = 1000.0f;
    public float Thrust = 750.0f;

    void Start() 
    {
        SetThrust(750.0f);
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.U))
        {
            SetThrust(Thrust + 50);
        }
        if (Input.GetKey(KeyCode.J))
        {
            SetThrust(Thrust - 50);
        }
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

        if (Trail != null && Trail.animation != null)
        {
            foreach (AnimationState state in Trail.animation)
            {
                state.enabled = true;
                state.time = Thrust / MaxThrust;
                Trail.animation.Sample();
                state.enabled = false;
            }
        }
    }

}
