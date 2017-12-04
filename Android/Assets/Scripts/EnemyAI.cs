using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    public enum State { Patrolling, Pursuing, Attacking, Pointing};
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
	
    public float dis;

	// Update is called once per frame
	void Update () {
        if (!moving)
        {
            GetComponent<Animator>().SetInteger("Do", 0);
            if (state == State.Attacking && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Combo"))
                state = State.Patrolling;
            dis = (currentCoor - targetCoor).magnitude;
            
            switch (state)
            {
                case State.Patrolling:
                    moving = mazeController.MoveCharacter(transform, ref currentCoor, patrolPattern[curPatrolIndex], ref targetPos, true);
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPattern.Length;
                    tForLerp = 0;
                    break;
                case State.Pursuing:
                    if ((currentCoor - targetCoor).magnitude <= 1.2f)
                    {
                        state = State.Attacking;
                        GetComponent<Animator>().SetInteger("Do", 4);
                    }
                    else if (currentCoor.y < targetCoor.y)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                    else if (currentCoor.y > targetCoor.y)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                    else if (currentCoor.x > targetCoor.x)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                    else if (currentCoor.x < targetCoor.x)
                        moving = mazeController.MoveCharacter(transform, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                    tForLerp = 0;
                    break;
                case State.Attacking:
                    //GetComponent<Animator>().SetInteger("Do", 4);
                    break;
            }
        }
        else
        {
            GetComponent<Animator>().SetInteger("Do", 1);
            tForLerp += Time.deltaTime / timeToTarget;
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
            if (tForLerp >= 1)
            {
                moving = false;
                currentPos = targetPos;
            }
        }
    }

    public void OnHittedPlayerWithSword(GameObject player)
    {
        if (state == State.Attacking)
        {
            player.GetComponent<PlayerController>().Hitted();
        }
    }
}
