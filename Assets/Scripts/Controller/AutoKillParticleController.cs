using UnityEngine;
using System.Collections;

public class AutoKillParticleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (particleSystem != null && !particleSystem.IsAlive())
        {
            Destroy(transform.gameObject);
        }
	}
}
