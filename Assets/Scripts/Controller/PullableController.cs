using UnityEngine;
using System;

public class PullableController : MonoBehaviour {
    public bool Enabled = true; 
    // Will be true when a pull object controller specifically is pulling this
    // object.
    // Otherwise multiple pull objects can pull at the same time.
    public PullObjectsController CapturedBy = null;
}
