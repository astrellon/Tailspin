using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public GameObject SparkPrefab;
    public float Lifetime = 3.0f;
    public float Damage = 1.0f;
    private Transform Remaining;
    private float Timeout = 0.0f;
	// Use this for initialization
	void Start () {
        Remaining = transform.Find("Remaining");
        Timeout = Time.time + Lifetime;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > Timeout)
        {
            DestroyBullet(transform.position, transform.forward);
        }
	}

    void OnCollisionEnter(Collision collision) 
    {
        ShipController ship = collision.collider.GetComponent<ShipController>();
        if (ship != null)
        {
            ship.DealDamage(Damage);
        }
        DestroyBullet(collision.contacts[0].point, collision.contacts[0].normal);
    }

    void DestroyBullet(Vector3 position, Vector3 direction)
    {
        if (SparkPrefab != null)
        {
            Instantiate(SparkPrefab, position, Quaternion.LookRotation(direction));
        }
        if (Remaining != null)
        {
            AutoKillController autoKill = Remaining.GetComponent<AutoKillController>();
            if (autoKill != null)
            {
                autoKill.Enabled = true;
            }
            Remaining.parent = null;
            Remaining.transform.position = transform.position;
            Remaining.audio.Play();
            Remaining = null;
        }
        Destroy(transform.gameObject);
    }
}
