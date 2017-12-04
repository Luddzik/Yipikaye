using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            transform.parent.GetComponent<EnemyAI>().OnHittedPlayerWithSword(other.gameObject);
    }
}
