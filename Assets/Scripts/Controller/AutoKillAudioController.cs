using UnityEngine;
using System.Collections;

public class AutoKillAudioController : MonoBehaviour {

	// Use this for initialization
    public bool Enabled = false;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Enabled && audio != null && !audio.isPlaying)
        {
            Destroy(transform.gameObject);
        }
	}
}
