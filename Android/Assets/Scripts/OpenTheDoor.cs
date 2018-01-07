using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTheDoor : MonoBehaviour {

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Vector3 rel = transform.InverseTransformPoint(other.transform.position);
            if(rel.x > 0)
                animator.SetTrigger("toLeft");
            else
                animator.SetTrigger("toRight");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            animator.SetTrigger("backToIdle");
        }
    }
}
