using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisappear : MonoBehaviour {

    [SerializeField]
    private float lifeTime;

    private bool isDying;
    private Color origin;
    private Color after;
    private float startTime;

    private void Start()
    {
        StartCoroutine("DisappearAfter", lifeTime/2);
        origin = GetComponent<MeshRenderer>().material.color;
        after = new Color(origin.r, origin.g, origin.b, 0);
    }

    IEnumerator DisappearAfter(float t)
    {
        yield return new WaitForSeconds(t);
        isDying = true;
        startTime = Time.time;
    }

    private void Update()
    {
        if (isDying)
        {
            float t = (2 / lifeTime) * (Time.time - startTime);
            GetComponent<MeshRenderer>().material.color = Color.Lerp(origin, after, t);
            if (t >= 1)
            {
                isDying = false;
                GetComponent<MeshRenderer>().material.color = origin;
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
