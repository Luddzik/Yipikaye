using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CapsuleController : MonoBehaviour {

    public float speed;
    public float jumpForce;

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.parent.up).normalized * CrossPlatformInputManager.GetAxis("Vertical") * Time.fixedDeltaTime * speed, Space.World);
        transform.Translate(Vector3.ProjectOnPlane(Camera.main.transform.right, transform.parent.up).normalized * CrossPlatformInputManager.GetAxis("Horizontal") * Time.fixedDeltaTime * speed, Space.World);

    }
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
            GetComponent<Rigidbody>().velocity = transform.up * jumpForce;
    }
}
