using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : MonoBehaviour {
    public GameObject BulletPrefab;
    public AudioClip FireClip;
    public string GunType = "Laser";

    public float Cooldown = 0.4f;

    public void Fire(IList<Transform> hardpoints) 
    {
        if (BulletPrefab == null)
        {
            return;
        }
        foreach (Transform trans in hardpoints)
        {
            GameObject newBullet = Instantiate(BulletPrefab, trans.position, trans.rotation) as GameObject;
            newBullet.rigidbody.velocity = transform.forward * 30;
            Physics.IgnoreCollision(newBullet.collider, collider);
        }
        if (FireClip != null)
        {
            audio.PlayOneShot(FireClip);
        }
    }
}
