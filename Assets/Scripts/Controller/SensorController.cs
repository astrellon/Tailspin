using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 /* Sensor class.
 */
public class SensorController : MonoBehaviour {

    public class Entry
    {
        public AttachmentController Object {get; set;}
        public AttachmentController AttachedTo {get; set;}
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
            Visible = visible;
            if (Visible)
            {
                Position = Object.transform.position;
                AttachedTo = Object.ParentAttachment;
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
        public AttachmentController GetAttachedTo()
        {
            if (Visible)
            {
                return Object.ParentAttachment;
            }
            return AttachedTo;
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
            // If we don't hit any terrain then we'll assume that it is within sight.
            if (!Physics.Linecast(transform.position, attachment.transform.position, out hit, 1 << 11))
            {
                visible = true;
            }
            int id = attachment.GetInstanceID(); 
            if (Entries.ContainsKey(id))
            {
                Entries[id].Update(visible);
            }
            else if (visible)
            {
                Entries[id] = new Entry(attachment, visible);
            }
        }
    }
}
