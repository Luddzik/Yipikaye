using System.Collections;
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

    [SerializeField]
    private Transform eyePos;

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
    public Collider[] targetsInViewRadius;
    public string hitedName;
    void FindVisibleTarget()
    {
        visibleTargetTransform.Clear();
        targetsInViewRadius = Physics.OverlapSphere(eyePos.position, radius * transform.lossyScale.y, targetMask);
        RaycastHit hitted;
        if (Physics.Raycast(eyePos.position, transform.forward, out hitted, radius * transform.lossyScale.y, obstacleMask, QueryTriggerInteraction.Collide))
        {
            hitedName = hitted.transform.name;
        }
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 targetDirection = (target.position - transform.position).normalized;
            //print(Vector3.Angle(transform.forward, targetDirection));
            if(Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2)
            {
                float targetDistance = Vector3.Distance(transform.position, target.position);
                RaycastHit hit;
                if(!Physics.Raycast(eyePos.position, targetDirection, out hit, targetDistance, obstacleMask, QueryTriggerInteraction.Collide))
                {
                    visibleTargetTransform.Add(target);
                    GetComponent<EnemyAI>().state = EnemyAI.State.Pursuing;
                    if(target.GetComponent<PlayerController>() != null)
                        GetComponent<EnemyAI>().targetCoor = target.GetComponent<PlayerController>().CurrentCoor;
                    else if(target.GetComponent<Game3PlayerController>() != null)
                        GetComponent<EnemyAI>().targetCoor = target.GetComponent<Game3PlayerController>().CurrentCoor;

                    GetComponent<EnemyAI>().OnFoundPlayer();
                }
            }
        }
        if (GetComponent<EnemyAI>().state == EnemyAI.State.Pursuing && visibleTargetTransform.Count == 0)
        {
            cantFindTime++;
            if (cantFindTime > GetComponent<EnemyAI>().timeToGiveUpPursuingWhenHidden)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.Patrolling;
                cantFindTime = 0;
            }
        }
    }
    //FOR DEBUG
    //public float MangleDWithLocalY;
    //public Vector3 Mdirection;
    //public float PangleDWithLocalY;
    //public Vector3 Pdirection;

    public Vector3 DirectionFormAngle(float angleD)
    {
        //if (angleD < 0)
        //{
        //    MangleDWithLocalY = angleD + transform.localEulerAngles.y;
        //    Mdirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleD), 0, Mathf.Cos(Mathf.Deg2Rad * angleD));
        //return transform.TransformDirection(Mdirection);
        //}
        //else
        //{
        //    PangleDWithLocalY = angleD + transform.localEulerAngles.y;
        //    Pdirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleD), 0, Mathf.Cos(Mathf.Deg2Rad * angleD));
        //return transform.TransformDirection(Pdirection);
        //}
        return transform.TransformDirection(new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleD), 0, Mathf.Cos(Mathf.Deg2Rad * angleD)));
    }
}
