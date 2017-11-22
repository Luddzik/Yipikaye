using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
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
            //currentPos = gridSystem.GetPosition(currentCoor);
            currentPos = mazeModel.GetPosition(currentCoor);
            targetPos = currentPos;
        }
    }
    [SerializeField]
    private Vector3 currentPos;
    [SerializeField]
    private Vector3 targetPos;
    [SerializeField]
    private float timeToTarget = 0.5f;
    private float tForLerp;
    private bool moving;
    [SerializeField]
    private MazeController mazeController;
    [SerializeField]
    private MazeModel mazeModel;
    //private GridSystem gridSystem;

    private Vector2 m_StartPos;
    private float minSwipeDistance = 150;
    private float[] relAngle;
    private GridSystem.Direction direction;

    // Use this for initialization
    void Start()
    {
        m_StartPos = Vector2.zero;
        relAngle = new float[4];
        tForLerp = 0;
        moving = false;
    }

    //public Text debugText1;
    //public Text debugText2;
    //public GameObject target;

    private void Update()
    {
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
            print(swipeVector);

            swipeVector = Vector3.ProjectOnPlane(swipeVector, transform.parent.up).normalized;
            //debugText2.text = swipeVector + "";
            relAngle[(int)GridSystem.Direction.Forward] = Vector3.Angle(swipeVector, transform.parent.forward);
            relAngle[(int)GridSystem.Direction.Back] = Vector3.Angle(swipeVector, -transform.parent.forward);
            relAngle[(int)GridSystem.Direction.Right] = Vector3.Angle(swipeVector, transform.parent.right);
            relAngle[(int)GridSystem.Direction.Left] = Vector3.Angle(swipeVector, -transform.parent.right);

            for (int i = 0; i < relAngle.Length; i++)
            {
                bool bigger = false;
                for (int j = 0; j < relAngle.Length; j++)
                {
                    if (relAngle[i] > relAngle[j])
                        bigger = true;
                }
                if (!bigger)
                {
                    switch (i)
                    {
                        case (int)MazeModel.Direction.Forward:
                            mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Forward, ref targetPos);
                            break;
                        case (int)GridSystem.Direction.Back:
                            mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Back, ref targetPos);
                            break;
                        case (int)GridSystem.Direction.Right:
                            mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Right, ref targetPos);
                            break;
                        case (int)GridSystem.Direction.Left:
                            mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Left, ref targetPos);
                            break;
                    }
                    moving = true;
                    tForLerp = 0;
                    return;
                }
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
                    relAngle[(int)GridSystem.Direction.Forward] = Vector3.Angle(swipeVector, transform.parent.forward);
                    relAngle[(int)GridSystem.Direction.Back] = Vector3.Angle(swipeVector, -transform.parent.forward);
                    relAngle[(int)GridSystem.Direction.Right] = Vector3.Angle(swipeVector, transform.parent.right);
                    relAngle[(int)GridSystem.Direction.Left] = Vector3.Angle(swipeVector, -transform.parent.right);

                    for (int i = 0; i < relAngle.Length; i++)
                    {
                        bool bigger = false;
                        for (int j = 0; j < relAngle.Length; j++)
                        {
                            if (relAngle[i] > relAngle[j])
                                bigger = true;
                        }
                        if (!bigger)
                        {
                            switch (i)
                            {
                                case (int)GridSystem.Direction.Forward:
                                    mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Forward, ref targetPos);
                                    break;
                                case (int)GridSystem.Direction.Back:
                                    mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Back, ref targetPos);
                                    break;
                                case (int)GridSystem.Direction.Right:
                                    mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Right, ref targetPos);
                                    break;
                                case (int)GridSystem.Direction.Left:
                                    mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Left, ref targetPos);
                                    break;
                            }
                            moving = true;
                            tForLerp = 0;
                            return;
                        }
                    }
                    break;
            }
        }
#endif
        if (moving)
        {
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
}
