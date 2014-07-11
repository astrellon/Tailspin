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
	void Update () 
    {
        ShipUpdate();

        if (CurrentTarget != null)
        {
            Vector3 toTarget = CurrentTarget.transform.position - transform.position;
            Vector3 normToTarget = toTarget.normalized;
            normToTarget = transform.worldToLocalMatrix.MultiplyVector(normToTarget);

            Quaternion angles = Quaternion.FromToRotation(Vector3.forward, normToTarget);
            Vector3 eulerAngles = angles.eulerAngles;
            if (eulerAngles.x > 180.0f) { eulerAngles.x -= 360.0f; }
            if (eulerAngles.y > 180.0f) { eulerAngles.y -= 360.0f; }
            if (eulerAngles.z > 180.0f) { eulerAngles.z -= 360.0f; }

            Vector3 rotateBy = new Vector3();
            float factor = 1.0f;
            float zero = 0.01f;
            rotateBy.x = (eulerAngles.x >  zero ?  factor : 
                         (eulerAngles.x < -zero ? -factor : 0.0f));
            rotateBy.y = (eulerAngles.y >  zero ?  factor : 
                         (eulerAngles.y < -zero ? -factor : 0.0f));
            rotateBy.z = (eulerAngles.z >  zero ?  factor :
                         (eulerAngles.z < -zero ? -factor : 0.0f));
            Rotate(rotateBy.x, rotateBy.y, rotateBy.z);
        }
	}
}
