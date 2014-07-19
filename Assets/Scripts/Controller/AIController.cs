using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : ShipController
{
    public GameObject CurrentTarget = null;
    void Start () 
    {
        AttachmentStart();
    }
    /**
     * Returns the smaller value, however it takes the sign of input into
     * account. So if input is negative then the value closer to zero out
     * of input and -max will be returned.
     */
    float Smallest(float max, float input)
    {
        if (input < 0.0f)
        {
            return input < -max ? -max : input;
        }
        return input > max ? max : input;
    }
	void Update () 
    {
        ShipUpdate();

        if (CurrentTarget != null)
        {
            Vector3 toTarget = CurrentTarget.transform.position - transform.position;
            Vector3 normToTarget = toTarget.normalized;
            normToTarget = transform.worldToLocalMatrix.MultiplyVector(normToTarget);

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normToTarget);
            Vector3 angles = rotation.eulerAngles * Mathf.Deg2Rad;
            if (angles.x > Mathf.PI) { angles.x -= Mathf.PI * 2.0f; }
            if (angles.y > Mathf.PI) { angles.y -= Mathf.PI * 2.0f; }
            if (angles.z > Mathf.PI) { angles.z -= Mathf.PI * 2.0f; }

            Vector3 factor = new Vector3();

            const float zero = 0.01f;

            // Currently using the magic number 4 to muliply the angles to rotate by.
            // The actual values will be clamped to [-1.0f, 1.0f] when Rotate is used.
            // Really what there needs to be is a curve that defines the amount of 
            // thrust required to turn by a given amount that would cap at the maximum
            // when it's fairly close to the target but quickly ramp down when it's 
            // very close to prevent it from constantly overshooting.
            // This curve would probably be defined by the angular velocity that
            // the ship can change by per second, which would be related to it's 
            // engine torque output and moment of inertia.
            factor.x = Mathf.Abs(angles.x) < zero ? 0.0f : Smallest(1.0f, angles.x * 4.0f);
            factor.y = Mathf.Abs(angles.y) < zero ? 0.0f : Smallest(1.0f, angles.y * 4.0f);
            factor.z = Mathf.Abs(angles.z) < zero ? 0.0f : Smallest(1.0f, angles.z * 4.0f);
            Rotate(factor.x, factor.y, factor.z);

            Vector3 move = new Vector3(1, 0, 0);
            if (toTarget.magnitude > 16)
            {
                move.z = 0.5f;
            }
            else if (toTarget.magnitude < 14)
            {
                move.z = -0.5f;
            }
            MoveDirection(move.x, move.y, move.z);
            CallAttachmentInterfaceByGroup(CurrentGunGroup, "Fire");
        }
	}
}
