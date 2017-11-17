using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPull : MonoBehaviour {

    private void FixedUpdate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Rigidbody>())
                transform.GetChild(i).GetComponent<Rigidbody>().AddForce(-transform.up * transform.GetChild(i).GetComponent<Rigidbody>().mass, ForceMode.Force);
        }
    }
}
