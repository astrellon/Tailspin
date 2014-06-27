using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 /* Sensor class.
 */
public class SensorController : MonoBehaviour {

    public class Entry
    {
        public AttachmentController Object {get; set;}
        public Vector3 Position {get; set;}
        public bool Visible {get; set;}

        public Entry(AttachmentController obj, bool visible)
        {
            Object = obj;
            Position = obj.transform.position;
            Visible = visible;
        }

        public void Update(bool visible)
        {
            //Debug.Log("Updating sensor object: " + Object.name + " (" + visible + ")");
            Visible = visible;
            if (Visible)
            {
                Position = Object.transform.position;
            }
        }

        public Vector3 GetPosition()
        {
            if (Visible)
            {
                return Object.transform.position;
            }
            return Position;
        }
    }

    protected Dictionary<int, Entry> Entries = new Dictionary<int, Entry>();
    public float Radius = 50.0f;
    private float NextPing = 0;

    void Start()
    {

    }

    void Update()
    {
        if (Time.time > NextPing)
        {
            DiscoverNearby();
            NextPing = Time.time + 1;
        }
    }

    public virtual IDictionary<int, Entry> GetEntries()
    {
        return Entries;
    }
    public virtual void DiscoverNearby()
    {
        //List<AttachmentController> nearby = new List<AttachmentController>();
        foreach (KeyValuePair<int, Entry> entry in Entries)
        {
            entry.Value.Visible = false;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider collider in hitColliders)
        {
            AttachmentController attachment = collider.transform.GetComponent<AttachmentController>();
            if (attachment == null)
            {
                continue;
            }

            bool visible = false;
            RaycastHit hit;
            if (Physics.Linecast(transform.position, attachment.transform.position, out hit,1 << 10 | 1 << 9 | 1 << 8))
            {
                if (hit.collider == null || hit.collider == attachment.collider)
                {
                    visible = true;
                }
            }
            int id = attachment.GetInstanceID(); 
            if (Entries.ContainsKey(id))
            {
                Entries[id].Update(visible);
            }
            else
            {
                //Debug.Log("New sensor object found: " + attachment.name + " (" + visible + ")");
                Entries[id] = new Entry(attachment, visible);
            }
        }
    }
}
