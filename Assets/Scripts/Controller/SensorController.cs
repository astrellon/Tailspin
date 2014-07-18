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
        public string Name {get; set;}

        public Entry(AttachmentController obj, bool visible)
        {
            Object = obj;
            Position = obj.transform.position;
            Visible = visible;
            Name = obj.name;
        }

        public void Update(bool visible)
        {
            Visible = visible;
            if (Visible && Object != null)
            {
                Position = Object.transform.position;
                AttachedTo = Object.ParentAttachment;
                Name = Object.name;
            }
        }

        public Vector3 GetPosition()
        {
            if (Visible && Object != null)
            {
                return Object.transform.position;
            }
            return Position;
        }
        public AttachmentController GetAttachedTo()
        {
            if (Visible && Object != null)
            {
                return Object.ParentAttachment;
            }
            return AttachedTo;
        }
        public string GetName()
        {
            if (Visible && Object != null)
            {
                return Object.name;
            }
            return Name;
        }
        public string GetHUDInfo()
        {
            if (Object == null)
            {
                return "<Destroyed>";
            }
            if (Visible)
            {
                ShipController isShip = Object as ShipController;
                if (isShip != null)
                {
                    return "Hull: " + isShip.Hull + "\nShields: " + isShip.Shields;
                }
                return "Type: " + Object.AttachmentType;
            }
            return "<Out of Range>";
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
            int id = attachment.gameObject.GetInstanceID(); 
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
