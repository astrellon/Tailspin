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
                newBullet.rigidbody.velocity = obj.transform.forward * BulletSpeed;
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
