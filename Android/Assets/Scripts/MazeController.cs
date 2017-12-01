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

    

    public bool MoveCharacter(Transform charTransform ,ref Vector2Int curCoor, MazeModel.Direction direction, ref Vector3 position, bool restrictInTheSquare)
    {
        switch (direction)
        {
            case MazeModel.Direction.Forward:
                //if (character is on the boundary) or (Forward wall in current square is impassible)
                //  don't move
                if (curCoor.y >= mazeModel.Row * 4 - 1 || (curCoor.y % 4 == 3 && mazeModel.outerGrid[curCoor.x/4, curCoor.y/4].impassable[0]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.y % 4 == 3)
                    return false;
                //if there is a block in the front
                if (mazeModel.outerGrid[curCoor.x / 4, (curCoor.y + 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y+1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.up;
                charTransform.localEulerAngles = Vector3.zero;
                //print("Move forward");
                break;
            case MazeModel.Direction.Back:
                //if (character is on the boundary) or (Back wall in current square is impassible)
                //  don't move
                if (curCoor.y <= 0 || (curCoor.y % 4 == 0 && mazeModel.outerGrid[curCoor.x / 4, curCoor.y / 4].impassable[1]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.y % 4 == 0)
                    return false;
                //if there is a block in the back
                if (mazeModel.outerGrid[curCoor.x / 4, (curCoor.y - 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y-1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.down;
                charTransform.localEulerAngles = Vector3.up * 180;
                //print("Move back");
                break;
            case MazeModel.Direction.Left:
                //if (character is on the boundary) or (Left wall in current square is impassible)
                //  don't move
                if (curCoor.x <= 0 || (curCoor.x % 4 == 0 && mazeModel.outerGrid[curCoor.x / 4, curCoor.y / 4].impassable[2]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.x % 4 == 0)
                    return false;
                //if there is a block on the left
                if (mazeModel.outerGrid[(curCoor.x-1) / 4, (curCoor.y) / 4].contents[((curCoor.x-1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.left;
                charTransform.localEulerAngles = Vector3.up * -90;
                //print("Move left");
                break;
            case MazeModel.Direction.Right:
                //if (character is on the boundary) or (Right wall in current square is impassible)
                //  don't move
                if (curCoor.x >= mazeModel.Column * 4 - 1 || (curCoor.x % 4 == 3 && mazeModel.outerGrid[curCoor.x / 4, curCoor.y / 4].impassable[3]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.x % 4 == 3)
                    return false;
                //if there is a block in the front
                if (mazeModel.outerGrid[(curCoor.x+1) / 4, (curCoor.y) / 4].contents[((curCoor.x+1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.right;
                charTransform.localEulerAngles = Vector3.up * 90;
                //print("Move right");
                break;
        }
        position = mazeModel.GetPosition(curCoor);
        position.y = charTransform.localPosition.y;
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