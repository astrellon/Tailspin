using System;
using UnityEngine;
using System.Collections.Generic;

public class ShipGUI : MonoBehaviour {

    public ShipController Ship = null;
    protected GUIText Hull = null;
    protected GUIText Hardpoints = null;

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
    }

    void OnGUI() {
        if (Ship == null)
        {
            return;
        }

        if (Hull != null)
        {
            Hull.text = "Hull: " + Ship.Hull;
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

            IDictionary<int, SensorController.Entry> entries = sensors.GetEntries();
            foreach (KeyValuePair<int, SensorController.Entry> pair in entries)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(pair.Value.GetPosition());
                if (screenPos.z < 0) {
                    continue;
                }
                string colour = "white";
                if (!pair.Value.Visible) {
                    colour = "red";
                }
                string text = "<color=" + colour + ">" + pair.Value.Object.name + "</color>";
                GUI.Label(new Rect(screenPos.x - 40, cam.pixelHeight - screenPos.y + 10, 80, 20), text, style);
            }

        }
    }
}

