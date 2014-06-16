using System;
using UnityEngine;

public class ShipGUI : MonoBehaviour {

    public ShipController Ship = null;
    protected GUIText Hull = null;

    void Start() {
        Transform HullTrans = transform.Find("Hull");
        if (HullTrans != null)
        {
            Hull = HullTrans.GetComponent<GUIText>();
        }
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
    }
}

