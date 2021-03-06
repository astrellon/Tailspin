using System;
using UnityEngine;
using System.Collections.Generic;

public class ShipGUI : MonoBehaviour {

    public ShipController Ship = null;
    protected GUIText Hull = null;
    protected GUIText Hardpoints = null;
    protected GUIText Nearby = null;

    protected GUIText FindGUIText(string child)
    {
        Transform trans = transform.Find(child);
        if (trans != null)
        {
            return trans.GetComponent<GUIText>();
        }
        return null;
    }

    void Start() 
    {
        Hull = FindGUIText("Hull");
        Hardpoints = FindGUIText("Hardpoints");
        Nearby = FindGUIText("Nearby");
    }

    void OnGUI() {
        if (Ship == null)
        {
            return;
        }

        if (Hull != null)
        {
            Hull.text = "Hull: " + Ship.Hull;
            if (Ship.MaxShields > 0.0f || Ship.Shields > 0.0f)
            {
                Hull.text += "\nShields: " + Ship.Shields;
            }
        }
        if (Hardpoints != null)
        {
            string points = "Hardpoints:\n";
            foreach (HardpointController hardpoint in Ship.Hardpoints)
            {
                if (!hardpoint.Enabled)
                {
                    continue;
                }
                points += hardpoint.name + ": ";
                if (hardpoint.Attached == null)
                {
                    points += "<Empty>";
                }
                else
                {
                    points += hardpoint.Attached.name;
                }
                points += "\n";
            }
            Hardpoints.text = points;
        }

        SensorController sensors = Ship.GetComponent<SensorController>();
        if (sensors != null)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperCenter;

            Camera cam = Camera.main;
            string nearby = "Nearby:";

            IDictionary<int, SensorController.Entry> entries = sensors.GetEntries();
            foreach (KeyValuePair<int, SensorController.Entry> pair in entries)
            {
                if (pair.Value.GetAttachedTo() != null || pair.Value.Object == Ship)
                {
                    continue;
                }

                string colour = "white";
                if (!pair.Value.Visible)
                {
                    colour = "red";
                }

                float dist = Vector4.Distance(pair.Value.GetPosition(), Ship.transform.position);
                string entryText = pair.Value.GetName() + " (" + dist.ToString("N") + ")";
                string guiEntryText = "<color=" + colour + ">" + entryText + " </color>";
                string hudEntryText = "<color=" + colour + ">" + entryText + "\n" + pair.Value.GetHUDInfo() + "</color>";
                nearby += "\n" + guiEntryText;

                Vector3 screenPos = cam.WorldToScreenPoint(pair.Value.GetPosition());
                if (screenPos.z < 0)
                {
                    continue;
                }

                GUI.Label(new Rect(screenPos.x - 40, cam.pixelHeight - screenPos.y + 10, 80, 20), hudEntryText, style);
            }

            if (Nearby != null)
            {
                Nearby.text = nearby;
            }
        }
    }
}

