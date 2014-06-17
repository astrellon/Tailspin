using System;
using UnityEngine;

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

    void Start() {
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
    }
}

