using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPull : MonoBehaviour {

    private void FixedUpdate()
    {
        transform.GetChild(0).GetComponent<Rigidbody>().AddForce(-transform.up * transform.GetChild(0).GetComponent<Rigidbody>().mass, ForceMode.Force);
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).GetComponent<Rigidbody>())
                transform.GetChild(1).GetChild(i).GetComponent<Rigidbody>().AddForce(-transform.up * transform.GetChild(1).GetChild(i).GetComponent<Rigidbody>().mass, ForceMode.Force);
        }

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    if(transform.GetChild(i).GetComponent<Rigidbody>())
        //        transform.GetChild(i).GetComponent<Rigidbody>().AddForce(-transform.up * transform.GetChild(i).GetComponent<Rigidbody>().mass, ForceMode.Force);
        //}
    }
}
