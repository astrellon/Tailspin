using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlightComputerController : MonoBehaviour {

    [System.Flags]
    public enum Controls 
    {
        NONE =      0x000,

        FORWARD =   0x001,
        BACKWARD =  0x002,
        LEFT =      0x004,
        RIGHT =     0x008,
        UP =        0x010,
        DOWN =      0x020,

        ROTXPOS =   0x040,
        ROTXNEG =   0x080,
        ROTYPOS =   0x100,
        ROTYNEG =   0x200,
        ROTZPOS =   0x400,
        ROTZNEG =   0x800
    }
    public class EngineEntity
    {
        public EngineController Engine;
        public float Distance;

        public EngineEntity(EngineController engine, float distance)
        {
            Engine = engine;
            Distance = distance;
        }
    }

    public Dictionary<Controls, List<EngineEntity>> Engines {get; protected set;}
    public ShipController Ship = null;
    private static float Half = Mathf.Sqrt(2) * 0.5f;
    private static float Zero = 0.01f;

    void Start()
    {
        Engines = new Dictionary<Controls, List<EngineEntity>>();
    }

    void Update()
    {

    }

    public void CalculateControl(EngineController engine, out Controls controls)
    {
        controls = Controls.NONE;
        if (engine == null)
        {
            return;
        }
    }
    /*
    public Vector3 CalculateTorqueToFace(GameObject target)
    {
        Vector3 toTarget = target.transform.position - transform.position;
        Vector3 normToTarget = toTarget.normalized;
        normToTarget = transform.worldToLocalMatrix.MultiplyVector(normToTarget);

        Quaternion angles = Quaternion.FromToRotation(Vector3.forward, normToTarget);
        Vector3 eulerAngles = angles.eulerAngles;
        if (eulerAngles.x > 180.0f) { eulerAngles.x -= 360.0f; }
        if (eulerAngles.y > 180.0f) { eulerAngles.y -= 360.0f; }
        if (eulerAngles.z > 180.0f) { eulerAngles.z -= 360.0f; }
        eulerAngles = eulerAngles * Mathf.Deg2Rad;

        Vector3 totalTimeVec = eulerAngles / Ship.RotateTorque;
        float totalTime = Mathf.Max(totalTimeVec.x, totalTimeVec.y, totalTimeVec.z);


        return eulerAngles;
    }
    */
    public static Vector3 MaxVelocity()
    {


        return new Vector3();
    }

    private void AddEngine(EngineController engine, float distance, Controls control)
    {
        if (!Engines.ContainsKey(control))
        {
            Engines[control] = new List<EngineEntity>();
        }
        Engines[control].Add(new EngineEntity(engine, distance));
    }
    public void AddEngine(EngineController engine)
    {
        if (engine == null)
        {
            Debug.Log("Cannot add null engine.");
            return;
        }
        if (Ship == null)
        {
            Debug.Log("Cannot add engine without a parent controller");
            return;
        }

        Vector3 localPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(engine.transform.position);

        Vector3 toCOM = Ship.CenterOfMass - localPosition;
        Vector3 normToCOM = toCOM.normalized;
        float distance = toCOM.magnitude;

        float goForward = Vector3.Dot(normToCOM, Vector3.forward);
        float goRight = Vector3.Dot(normToCOM, Vector3.right);
        float goUp = Vector3.Dot(normToCOM, Vector3.up);
        Vector3 angle = Vector3.Cross(transform.worldToLocalMatrix.MultiplyVector(engine.transform.forward), normToCOM);

        // Check if going forward
        if (goForward > Half)
        {
            AddEngine(engine, distance, Controls.FORWARD);
        }
        // Check if going backward
        if (goForward < -Half)
        {
            AddEngine(engine, distance, Controls.BACKWARD);
        }

        // Check if going right
        if (goRight > Half)
        {
            AddEngine(engine, distance, Controls.RIGHT);
        }
        // Check if going left
        if (goRight < -Half)
        {
            AddEngine(engine, distance, Controls.LEFT);
        }

        // Check if going up
        if (goUp > Half)
        {
            AddEngine(engine, distance, Controls.UP);
        }
        // Check if going down
        if (goUp < -Half)
        {
            AddEngine(engine, distance, Controls.DOWN);
        }

        // Check if torque is being applied about the x axis
        if (angle.x < -Zero)
        {
            AddEngine(engine, distance, Controls.ROTXPOS);
        }
        if (angle.x > Zero)
        {
            AddEngine(engine, distance, Controls.ROTXNEG);
        }

        // Check if torque is being applied about the y axis
        if (angle.y < -Zero)
        {
            AddEngine(engine, distance, Controls.ROTYPOS);
        }
        if (angle.y > Zero)
        {
            AddEngine(engine, distance, Controls.ROTYNEG);
        }

        // Check if torque is being applied about the z axis
        if (angle.z < -Zero)
        {
            AddEngine(engine, distance, Controls.ROTZPOS);
        }
        if (angle.z > Zero)
        {
            AddEngine(engine, distance, Controls.ROTZNEG);
        }
    }
}
