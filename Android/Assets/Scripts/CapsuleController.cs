using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CapsuleController : MonoBehaviour {

    public float speed;
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
    private float[] relAngle;
    private Vector3 joystickVector;
    public Vector2Int currentCoor;

    // Use this for initialization
    void Start ()
    {
        relAngle = new float[4];
        tForLerp = 0;
        moving = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //transform.Translate(Vector3.ProjectOnPlane(Camera.main.transform.forward, transform.parent.up).normalized * CrossPlatformInputManager.GetAxis("Vertical") * Time.fixedDeltaTime * speed, Space.World);
        //transform.Translate(Vector3.ProjectOnPlane(Camera.main.transform.right, transform.parent.up).normalized * CrossPlatformInputManager.GetAxis("Horizontal") * Time.fixedDeltaTime * speed, Space.World);

    }
    void Update()
    {
        if (CrossPlatformInputManager.GetAxis("Vertical") != 0 || CrossPlatformInputManager.GetAxis("Horizontal") != 0)
        {
            joystickVector = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, CrossPlatformInputManager.GetAxis("Vertical"));
            relAngle[0] = Vector3.Angle(joystickVector, transform.parent.forward);
            relAngle[1] = Vector3.Angle(joystickVector, -transform.parent.forward);
            relAngle[2] = Vector3.Angle(joystickVector, transform.parent.right);
            relAngle[3] = Vector3.Angle(joystickVector, -transform.parent.right);

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
                            moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Forward, ref targetPos);
                            break;
                        case (int)MazeModel.Direction.Back:
                            moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Back, ref targetPos);
                            break;
                        case (int)MazeModel.Direction.Right:
                            moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Right, ref targetPos);
                            break;
                        case (int)MazeModel.Direction.Left:
                            moving = mazeController.MoveCharacter(ref currentCoor, MazeModel.Direction.Left, ref targetPos);
                            break;
                    }
                    tForLerp = 0;
                    return;
                }
            }
        }
        if (moving)
        {
            tForLerp += Time.deltaTime / timeToTarget;
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
            if (tForLerp >= 1)
            {
                moving = false;
                currentPos = targetPos;
                print("reach target");
            }
        }
    }
}
