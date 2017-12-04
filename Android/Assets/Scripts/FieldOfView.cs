﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float radius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargetTransform = new List<Transform>();

    private int cantFindTime;

    private void Start()
    {
        cantFindTime = 0;
        StartCoroutine("FindTargetWithDelay", 0.2f);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTarget();
        }
    }

    void FindVisibleTarget()
    {
        visibleTargetTransform.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius * transform.lossyScale.x, targetMask);
        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 targetDirection = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2)
            {
                float targetDistance = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, targetDirection, targetDistance, obstacleMask))
                {
                    visibleTargetTransform.Add(target);
                    GetComponent<EnemyAI>().state = EnemyAI.State.Pursuing;
                    GetComponent<EnemyAI>().targetCoor = target.GetComponent<PlayerController>().CurrentCoor;
                }
            }
        }
        if (GetComponent<EnemyAI>().state == EnemyAI.State.Pursuing && visibleTargetTransform.Count == 0)
        {
            cantFindTime++;
            if (cantFindTime > 10)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.Patrolling;
                cantFindTime = 0;
            }
        }
    }

    public float angleY;

	public Vector3 DirectionFormAngle(float angleD)
    {
        angleD += transform.localEulerAngles.y;
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleD), 0, Mathf.Cos(Mathf.Deg2Rad * angleD));
    }
}
