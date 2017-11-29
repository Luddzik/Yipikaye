using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public enum State { Patrolling, Pursuing};
    public State state = State.Patrolling;
    [SerializeField]
    private MazeModel.Direction[] patrolPattern;
    private int curPatrolIndex;

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

    public Vector2Int targetCoor;

    [SerializeField]
    private Vector3 currentPos;
    [SerializeField]
    private Vector3 targetPos;
    [SerializeField]
    private float timeToTarget = 0.5f;
    [SerializeField]
    private float tForLerp;
    [SerializeField]
    private bool moving;
    public MazeController mazeController;
    public MazeModel mazeModel;

    // Use this for initialization
    void Start (){
        curPatrolIndex = 0;
        tForLerp = 0;
        moving = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!moving)
        {
            switch (state)
            {
                case State.Patrolling:
                    moving = mazeController.MoveCharacter(transform, ref currentCoor, patrolPattern[curPatrolIndex], ref targetPos, true);
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPattern.Length;
                    tForLerp = 0;
                    break;
                case State.Pursuing:
                    if(currentCoor.y < targetCoor.y)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                    else if (currentCoor.y > targetCoor.y)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                    else if (currentCoor.x > targetCoor.x)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                    else if (currentCoor.x < targetCoor.x)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                    tForLerp = 0;
                    break;
            }
        }
        else
        {
            tForLerp += Time.deltaTime / timeToTarget;
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
            if (tForLerp >= 1)
            {
                moving = false;
                currentPos = targetPos;
            }
        }
    }
}
