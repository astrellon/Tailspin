using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : AttachmentController {

    public GameObject BulletPrefab;
    public AudioClip FireClip;
    
    [BitMask(typeof(HardpointController.Type))]
    public HardpointController.Type GunTypes;

    public float BulletSpeed = 30.0f;
    public float Cooldown = 0.4f;
    private float NextFire = 0.0f;

    void Start ()
    {
        AttachInterface["Fire"] = FireGun;

        AttachmentStart();
    }
    public object FireGun(params object[] arg)
    {
        IList<HardpointController> firePoints = FindAllHardpointsWithType(GunTypes);
        Fire(firePoints);
        return true;
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
                Vector3 position = obj.transform.position + obj.transform.forward * 1.0f;
                GameObject newBullet = Instantiate(BulletPrefab, position, obj.transform.rotation) as GameObject;
                // Bullets along the direction vector get a speed boost.
                Vector3 speedBoost = Vector3.Project(rigidbody.velocity, obj.transform.forward);

                Vector3 velo = obj.transform.forward * BulletSpeed + speedBoost;
                newBullet.rigidbody.velocity = velo;

                // Bullets cannot collide with the gun that fired it.
                if (collider != null)
                {
                    Physics.IgnoreCollision(newBullet.collider, collider);
                }

                // Bullets also do not collide with the attachment chain that fired it.
                // This could be changed to a distance or timer so that it can collide 
                // but not immediately after being fired.
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
