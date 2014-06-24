using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    public static SceneManager MainManager;

	// Use this for initialization
	void Start () {
        MainManager = this;
	
        Screen.lockCursor = true;
	}
	
	// Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(0)) 
        {
            Screen.lockCursor = true;
        }

        if (Input.GetKeyDown("escape"))
        {
            Screen.lockCursor = false;
        }
        
    }
}
