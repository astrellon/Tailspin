using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public GameObject SparkPrefab;
    public float Lifetime = 3.0f;
    public float Damage = 1.0f;
    public GameObject Owner = null;
    private Transform Remaining;
    private float Timeout = 0.0f;
	// Use this for initialization
	void Start ()
    {
        Remaining = transform.Find("Remaining");
        Timeout = Time.time + Lifetime;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time > Timeout)
        {
            DestroyBullet(transform.position, transform.forward);
            return;
        }
	}

    void FixedUpdate()
    {
        if (this == null)
        {
            return;
        }
        Vector3 newPosition = transform.position + rigidbody.velocity * Time.fixedDeltaTime;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, newPosition, out hit))
        {
            if (hit.collider.gameObject != Owner)
            {
                OnCollision(hit.collider, hit.point, hit.normal);
            }
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        OnCollision(collision.collider, collision.contacts[0].point, collision.contacts[0].normal);
    }

    void OnCollision(Collider coll, Vector3 point, Vector3 normal)
    {
        ShipController ship = coll.GetComponent<ShipController>();
        if (ship != null)
        {
            ship.DealDamage(Damage);
        }
        DestroyBullet(point, normal);
    
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
