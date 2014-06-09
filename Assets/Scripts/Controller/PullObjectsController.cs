using UnityEngine;
using System;

public class PullObjectsController : MonoBehaviour {
    public float Radius = 10.0f;
    public float Strength = 1.0f;
    public bool Enabled = true;
    public bool EnableGeneralPull = false;

    public PullableController PullSpecific = null;

    void Update () {
        if (!Enabled)
        {
            return;
        }
        if (PullSpecific != null)
        {
            Pull(PullSpecific);
        }
        else if (EnableGeneralPull)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider collider in hitColliders)
            {
                PullableController pullable = collider.transform.GetComponent<PullableController>();
                if (pullable == null || pullable.CapturedBy != null)
                {
                    continue;
                }
                Pull(pullable);
            }
        }
    }

    public void Pull(PullableController pullable)
    {
        Rigidbody pullRigidbody = pullable.rigidbody;
        if (pullRigidbody == null)
        {
            return;
        }
        Vector3 toTarget = pullable.transform.position - transform.position;
        float distance = toTarget.magnitude;
        // Circular falloff.
        //float pull = Mathf.Sqrt(Radius * Radius - distance * distance) / Radius * Strength;
        // Linear falloff.
        float pull = Strength - (Strength / Radius) * distance;

        float mass = pullRigidbody.mass;

        pullRigidbody.AddForce(toTarget * -pull);
    }
}