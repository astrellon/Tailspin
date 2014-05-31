using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : MonoBehaviour {
    public GameObject BulletPrefab;
    public AudioClip FireClip;
    public string GunType = "Laser";

    public float Cooldown = 0.4f;

    public void Fire(params List<ShipHardpointController>[] hardpoints) 
    {
        if (BulletPrefab == null)
        {
            return;
        }
        foreach (List<ShipHardpointController> list in hardpoints)
        {
            foreach (ShipHardpointController obj in list)
            {
                GameObject newBullet = Instantiate(BulletPrefab, obj.transform.position, obj.transform.rotation) as GameObject;
                newBullet.rigidbody.velocity = transform.forward * 30;
                Physics.IgnoreCollision(newBullet.collider, collider);
            }
        }
        if (FireClip != null)
        {
            audio.PlayOneShot(FireClip);
        }
    }
}
