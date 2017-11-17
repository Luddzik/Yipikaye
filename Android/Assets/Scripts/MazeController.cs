using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{

    private MazeModel mazeModel;
    
    private Vector3 localPtZero;
    //private float localsquareLengthX;
    //private float localsquareLengthY;
    

    [SerializeField]
    private MazeGenerator mazeGen;

    // Use this for initialization
    void Start()
    {
        mazeModel = GetComponent<MazeModel>();
    }

    private void Update()
    {
        //if (!modifying)
        //    DrawGrid();
    }

    

    public bool MoveCharacter(ref Vector2Int curCoor, MazeModel.Direction direction, ref Vector3 position)
    {
        switch (direction)
        {
            case MazeModel.Direction.Forward:
                if (curCoor.y >= mazeModel.Row - 1)
                    return false;
                curCoor += Vector2Int.up;
                print("Move forward");
                break;
            case MazeModel.Direction.Back:
                if (curCoor.y <= 0)
                    return false;
                curCoor += Vector2Int.down;
                print("Move back");
                break;
            case MazeModel.Direction.Left:
                if (curCoor.x <= 0)
                    return false;
                curCoor += Vector2Int.left;
                print("Move left");
                break;
            case MazeModel.Direction.Right:
                if (curCoor.x >= mazeModel.Column - 1)
                    return false;
                curCoor += Vector2Int.right;
                print("Move right");
                break;
        }
        position = mazeModel.GetPosition(curCoor);
        return true;
    }

    public void ChangeRow(float rowNum)
    {
        mazeModel.Row = (int)rowNum;
        mazeGen.SetupGrid();
    }

    public void ChangeColumn(float columnNum)
    {
        mazeModel.Column = (int)columnNum;
        mazeGen.SetupGrid();
    }

    public void ChangeSize(float size)
    {
        mazeModel.Size = size;

        Vector3 newScale = new Vector3(size * mazeModel.Column, transform.localScale.y, size * mazeModel.Row);
        Vector3 scaleDiff = new Vector3(newScale.x / transform.localScale.x,
            0,
            newScale.z / transform.localScale.z);

        transform.localScale = newScale;
        transform.GetChild(0).localScale = new Vector3(transform.GetChild(0).localScale.x / scaleDiff.x,
                transform.GetChild(0).localScale.y,
                transform.GetChild(0).localScale.z / scaleDiff.z);
    }
}