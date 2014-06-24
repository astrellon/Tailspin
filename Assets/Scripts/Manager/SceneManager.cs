using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    public static SceneManager MainManager;

	// Use this for initialization
	void Start () {
        MainManager = this;
	
        Screen.lockCursor = true;

        GameObject obj1 = GameObject.Find("GameObject1");
        GameObject obj2 = GameObject.Find("GameObject2");
        GameObject obj3 = GameObject.Find("GameObject3");
        GameObject obj4 = GameObject.Find("GameObject4");

        Debug.Log("Rotation1: " + obj1.transform.localRotation);
        Debug.Log("Rotation2: " + obj2.transform.localRotation);
        Debug.Log("Rotation3: " + obj3.transform.localRotation);
        
        Quaternion test1 = Quaternion.Lerp(obj2.transform.rotation, obj1.transform.rotation, 1.0f) * obj3.transform.localRotation;
        Debug.Log("Test1: " + test1);

        obj4.transform.rotation = test1;
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
