﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    [Header("Game Balance")]
    [SerializeField]
    private float timeToTarget = 0.5f;
    [SerializeField]
    private float minSwipeDistance = 150;
    [SerializeField]
    private float shiningTime;

    [Header("Variables")]
    private Vector2Int currentCoor;
    public Vector2Int CurrentCoor
    {
        get
        {
            return currentCoor;
        }
        set
        {
            currentCoor = value;
            currentPos = mazeModel.GetPosition(currentCoor);
            targetPos = currentPos;
        }
    }
    [SerializeField]
    private Vector3 currentPos;
    [SerializeField]
    private Vector3 targetPos;
    private float tForLerp;
    private bool moving;

    [Header("References")]
    [SerializeField]
    private MazeController mazeController;
    [SerializeField]
    private MazeModel mazeModel;
    public Transform eyePos;
    //private GridSystem gridSystem;

    private Vector2 m_StartPos;
    
    private int maxRelAngle;
    private int[] relAngle;
    //private GridSystem.Direction direction;

    // Use this for initialization
    void Start()
    {
        m_StartPos = Vector2.zero;
        relAngle = new int[4];
        tForLerp = 0;
        moving = false;
    }

    //public Text debugText1;
    //public Text debugText2;
    //public GameObject target;

    private void Update()
    {
        if (!moving)
        {
            GetComponent<Animator>().SetInteger("Do", 0);
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                m_StartPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 swipeVector = new Vector3(Input.mousePosition.x - m_StartPos.x, 0, Input.mousePosition.y - m_StartPos.y);

                //debugText1.text = swipeVector + "";

                if (swipeVector.magnitude < minSwipeDistance)
                    return;

                swipeVector = Camera.main.transform.TransformDirection(swipeVector.normalized);
                //print(swipeVector);
                swipeVector = Vector3.ProjectOnPlane(swipeVector, transform.parent.up).normalized;
                //print(swipeVector);

                //debugText2.text = swipeVector + "";
                relAngle[0] = (int)Vector3.Angle(swipeVector, transform.parent.forward);
                //print(relAngle[(int)MazeModel.Direction.Forward]);
                relAngle[1] = (int)Vector3.Angle(swipeVector, -transform.parent.forward);
                //print(relAngle[(int)MazeModel.Direction.Back]);
                relAngle[2] = (int)Vector3.Angle(swipeVector, -transform.parent.right);
                //print(relAngle[(int)MazeModel.Direction.Left]);
                relAngle[3] = (int)Vector3.Angle(swipeVector, transform.parent.right);
                //print(relAngle[(int)MazeModel.Direction.Right]);

                maxRelAngle = Mathf.Min(relAngle[0], relAngle[1], relAngle[2], relAngle[3]);

                if (maxRelAngle == relAngle[0])
                {
                    moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                }
                else if (maxRelAngle == relAngle[1])
                    moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                else if (maxRelAngle == relAngle[2])
                    moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                else if (maxRelAngle == relAngle[3])
                    moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                else
                    return;

                tForLerp = 0;

                //for (int i = 0; i < relAngle.Length; i++)
                //{
                //    bool bigger = false;
                //    for (int j = 0; j < relAngle.Length; j++)
                //    {
                //        if (relAngle[i] > relAngle[j])
                //            bigger = true;
                //    }
                //    if (!bigger)
                //    {
                //        switch (i)
                //        {
                //            case (int)MazeModel.Direction.Forward:
                //                moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Forward, ref targetPos);
                //                break;
                //            case (int)GridSystem.Direction.Back:
                //                moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Back, ref targetPos);
                //                break;
                //            case (int)GridSystem.Direction.Right:
                //                moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Right, ref targetPos);
                //                break;
                //            case (int)GridSystem.Direction.Left:
                //                moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Left, ref targetPos);
                //                break;
                //        }
                //        tForLerp = 0;
                //        return;
                //    }
                //}
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
                        m_StartPos = touch.position;
                        break;
                    case TouchPhase.Ended:
                        Vector3 swipeVector = new Vector3(touch.position.x - m_StartPos.x, touch.position.y - m_StartPos.y, 0);
                        //debugText1.text = swipeVector + "";
                        if (swipeVector.magnitude < minSwipeDistance)
                            return;
                        swipeVector = Camera.main.transform.TransformDirection(swipeVector.normalized);
                        //debugText1.text = swipeVector + "";

                        swipeVector = Vector3.ProjectOnPlane(swipeVector, transform.parent.up).normalized;
                        //debugText2.text = swipeVector + "";
                        relAngle[0] = (int)Vector3.Angle(swipeVector, transform.parent.forward);
                        //print(relAngle[(int)MazeModel.Direction.Forward]);
                        relAngle[1] = (int)Vector3.Angle(swipeVector, -transform.parent.forward);
                        //print(relAngle[(int)MazeModel.Direction.Back]);
                        relAngle[2] = (int)Vector3.Angle(swipeVector, -transform.parent.right);
                        //print(relAngle[(int)MazeModel.Direction.Left]);
                        relAngle[3] = (int)Vector3.Angle(swipeVector, transform.parent.right);
                        //print(relAngle[(int)MazeModel.Direction.Right]);

                        maxRelAngle = Mathf.Min(relAngle[0], relAngle[1], relAngle[2], relAngle[3]);

                        if (maxRelAngle == relAngle[0])
                            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                        else if (maxRelAngle == relAngle[1])
                            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                        else if (maxRelAngle == relAngle[2])
                            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                        else if (maxRelAngle == relAngle[3])
                            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                        else
                            return;

                        tForLerp = 0;

                        //for (int i = 0; i < relAngle.Length; i++)
                        //{
                        //    bool bigger = false;
                        //    for (int j = 0; j < relAngle.Length; j++)
                        //    {
                        //        if (relAngle[i] > relAngle[j])
                        //            bigger = true;
                        //    }
                        //    if (!bigger)
                        //    {
                        //        switch (i)
                        //        {
                        //            case (int)GridSystem.Direction.Forward:
                        //                mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Forward, ref targetPos);
                        //                break;
                        //            case (int)GridSystem.Direction.Back:
                        //                mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Back, ref targetPos);
                        //                break;
                        //            case (int)GridSystem.Direction.Right:
                        //                mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Right, ref targetPos);
                        //                break;
                        //            case (int)GridSystem.Direction.Left:
                        //                mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Left, ref targetPos);
                        //                break;
                        //        }
                        //        moving = true;
                        //        tForLerp = 0;
                        //        return;
                        //    }
                        //}
                        break;
                }
            }
#endif
        }
        else 
        {
            GetComponent<Animator>().SetInteger("Do", 1);

            tForLerp += Time.deltaTime / timeToTarget;
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
            if(tForLerp >=1)
            {
                moving = false;
                currentPos = targetPos;
                print("reach target");
            }
        }
    }
    public void UseShine()
    {
        StartCoroutine(Shining());
    }

    private IEnumerator Shining()
    {
        print("Shine!");
        transform.GetChild(0).GetComponent<Light>().range *= 4;
        yield return new WaitForSeconds(shiningTime);
        transform.GetChild(0).GetComponent<Light>().range /= 4;
    }
}


