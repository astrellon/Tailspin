using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

    public AudioClip OpenClip;
    public AudioClip CloseClip;
    public bool IsOpen = false;

    private bool CurrentlyOpen = false;
    private float OpenUntil = 0;
    private Vector3 OrigScale;
	// Use this for initialization
	void Start () {
        OrigScale = transform.localScale;
	}

    public void OpenDoor()
    {
        OpenUntil = Time.time + 5.0f;
        Vector3 newScale = OrigScale;
        newScale.y = 0.1f;
        transform.localScale = newScale;
        CurrentlyOpen = true;
        if (audio != null && OpenClip != null)
        {
            audio.PlayOneShot(OpenClip);
        }
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsOpen", true);
    }
    public void CloseDoor()
    {
        OpenUntil = 0.0f;
        transform.localScale = OrigScale;
        CurrentlyOpen = false;
        if (audio != null && CloseClip != null)
        {
            audio.PlayOneShot(CloseClip);
        }
        Animator animator = GetComponent<Animator>();
        animator.SetBool("IsOpen", false);
    }
	
	// Update is called once per frame
	void Update () {
        if (IsOpen != CurrentlyOpen)
        {
            if (IsOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
        if (IsOpen && Time.time > OpenUntil)
        {
            IsOpen = false;
        }
	}

    public void ChildCollision(Collision collision)
    {
        IsOpen = true;
    }
    void OnCollisionEnter(Collision collision) 
    {
        IsOpen = true;
    }
}
