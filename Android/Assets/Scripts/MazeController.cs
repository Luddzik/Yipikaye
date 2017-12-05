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
    [SerializeField]
    private GameObject exitScreen;
    [SerializeField]
    private GameObject deadScreen;
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
                charTransform.localEulerAngles = Vector3.zero;
                //if (character is on the boundary) or (Forward wall in current square is impassible)
                //  don't move
                if (curCoor.y >= mazeModel.Row * 4 - 1 || (curCoor.y % 4 == 3 && mazeModel.grid[curCoor.x/4, curCoor.y/4].impassable[0]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.y % 4 == 3)
                    return false;
                //if there is a block in the front
                if (mazeModel.grid[curCoor.x / 4, (curCoor.y + 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y+1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.up;
                //print("Move forward");
                break;
            case MazeModel.Direction.Back:
                charTransform.localEulerAngles = Vector3.up * 180;
                //if (character is on the boundary) or (Back wall in current square is impassible)
                //  don't move
                if (curCoor.y <= 0 || (curCoor.y % 4 == 0 && mazeModel.grid[curCoor.x / 4, curCoor.y / 4].impassable[1]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.y % 4 == 0)
                    return false;
                //if there is a block in the back
                if (mazeModel.grid[curCoor.x / 4, (curCoor.y - 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y-1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.down;
                //print("Move back");
                break;
            case MazeModel.Direction.Left:
                charTransform.localEulerAngles = Vector3.up * -90;
                //if (character is on the boundary) or (Left wall in current square is impassible)
                //  don't move
                if (curCoor.x <= 0 || (curCoor.x % 4 == 0 && mazeModel.grid[curCoor.x / 4, curCoor.y / 4].impassable[2]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.x % 4 == 0)
                    return false;
                //if there is a block on the left
                if (mazeModel.grid[(curCoor.x-1) / 4, (curCoor.y) / 4].contents[((curCoor.x-1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.left;
                //print("Move left");
                break;
            case MazeModel.Direction.Right:
                charTransform.localEulerAngles = Vector3.up * 90;
                //if (character is on the boundary) or (Right wall in current square is impassible)
                //  don't move
                if (curCoor.x >= mazeModel.Column * 4 - 1 || (curCoor.x % 4 == 3 && mazeModel.grid[curCoor.x / 4, curCoor.y / 4].impassable[3]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.x % 4 == 3)
                    return false;
                //if there is a block in the front
                if (mazeModel.grid[(curCoor.x+1) / 4, (curCoor.y) / 4].contents[((curCoor.x+1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                curCoor += Vector2Int.right;
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

    public void ChangeSize(float change)
    {
        transform.localScale *= change;
        transform.GetChild(0).GetChild(0).GetComponent<Light>().range *= change;
        transform.GetChild(0).GetChild(1).GetComponent<Light>().range *= change;
    }

    public void OnExitEnter()
    {
        exitScreen.SetActive(true);
    }

    public void OnDeath()
    {
        deadScreen.SetActive(true);
    }
}