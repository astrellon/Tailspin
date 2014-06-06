using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : AttachmentController {

    public GameObject BulletPrefab;
    public AudioClip FireClip;
    public string GunType = "Laser";

    public float Cooldown = 0.4f;
    private float NextFire = 0.0f;

    //protected HardpointController[] Hardpoints = null; 

    void Start () {
        //Hardpoints = GetComponentsInChildren<HardpointController>();

        AttachInterface["Fire"] = FireGun;

        //DiscoverHardpoints();
    }
    public object FireGun(params object[] arg)
    {
        //Fire(Hardpoints);
        Debug.Log("Do fire gun");
        if (Hardpoints.ContainsKey(HardpointController.Type.LASER))
        {
            List<HardpointController> firePoints = Hardpoints[HardpointController.Type.LASER];
            Debug.Log("Fire gun: " + firePoints.Count);
            Fire(firePoints);
        }
        return true;
    }
    void Update () {
    }

    public void Fire(params IList<HardpointController>[] hardpoints) 
    {
        if (BulletPrefab == null || NextFire > Time.time)
        {
            return;
        }
        NextFire = Time.time + Cooldown;
        foreach (IList<HardpointController> list in hardpoints)
        {
            foreach (HardpointController obj in list)
            {
                GameObject newBullet = Instantiate(BulletPrefab, obj.transform.position, obj.transform.rotation) as GameObject;
                newBullet.rigidbody.velocity = obj.transform.forward * 30;
                if (collider != null)
                {
                    Physics.IgnoreCollision(newBullet.collider, collider);
                }
                AttachmentController parent = ParentAttachment;
                while (parent != null)
                {
                    if (parent.collider != null)
                    {
                        Physics.IgnoreCollision(newBullet.collider, parent.collider);
                    }
                    parent = parent.ParentAttachment;
                }
            }
        }
        if (FireClip != null)
        {
            audio.PlayOneShot(FireClip);
        }
    }
}
