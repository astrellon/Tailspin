using UnityEngine;
using System.Collections;

public class AutoKillController : MonoBehaviour {

	// Use this for initialization
    public bool Enabled = false;
    public bool KillParticle = true;
    public bool KillAudio = true;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (!Enabled || (!KillAudio && !KillParticle))
        {
            return;
        }

        int count = 0;
        if (KillParticle && particleSystem != null) 
        {
            count++;
            count += !particleSystem.IsAlive() ? -1 : 0;
        }
        if (KillAudio && audio != null)
        {
            count++;
            count += !audio.isPlaying ? -1 : 0;
        }

        if (count <= 0)
        {
            Destroy(transform.gameObject);
        }
	}
}
