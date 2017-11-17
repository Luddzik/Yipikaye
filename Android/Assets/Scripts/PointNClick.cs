using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PointNClick : MonoBehaviour {

    [SerializeField]
    private Vector3 currentPos;
    [SerializeField]
    private Vector3 targetPos;
    [SerializeField]
    private float timeToTarget = 0.5f;
    private float tForLerp;
    private bool moving;

    private NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        tForLerp = 0;
        moving = false;
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);
    }

    //public Text debugText1;
    //public Text debugText2;
    //public GameObject target;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                print(hit.transform.name + " Hitted at " + hit.point);
                agent.SetDestination(hit.point);
            }

        }
#endif
        //debugText1.text = Camera.main.transform.position+"";
        //debugText2.text = target.transform.position+"";
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    targetPos = touch.position;
                    break;
            }
        }
#endif
        //if (moving)
        //{
        //    tForLerp += Time.deltaTime / timeToTarget;
        //    transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
        //    if (tForLerp >= 1)
        //    {
        //        moving = false;
        //        currentPos = targetPos;
        //        print("reach target");
        //    }
        //}
    }
}
