using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MazeModel : MonoBehaviour {
    public enum Direction { Forward, Back, Left, Right }

    //private Quaternion originalRot;
    private Vector3 centerPos;
    public Vector3 CenterPos
    {
        get { return centerPos; }
        set { centerPos = value; }
    }

    [SerializeField]
    [Range(2, 50)]
    private int row = 2;
    public int Row
    {
        get { return row; }
        set { row = value; }
    }
    [SerializeField]
    [Range(2, 50)]
    private int column = 2;
    public int Column
    {
        get { return column; }
        set { column = value; }
    }
    [SerializeField]
    [Range(0.1f, 10f)]
    private float size = 0.7f;
    public float Size
    {
        get { return size; }
        set { size = value; }
    }
    public Tile[,] grid;
    //public Tile[,] innerGrid;
    public bool[,] impassibles; //store the booleans of whether a wall is passible.
    //     _I___I___I_     3
    //    I R I R I R I    2   R: Room
    //    | I   I   I |    2   I: impassibles' element
    //    I R I R I R I    1
    //    | I   I   I |    1
    //    I R I R I R I    0
    //    |_I___I___I_|    0
    //                   Row
    //Col 0 1 2 3 4 5 6
    public int[] zoneEquivalencyArray;

    public float charHeight;
    public Direction mainEntranceFacing;

    //public Vector2Int currentCoor;
    //public Vector2Int CurrentCoor
    //{
    //    get
    //    {
    //        return currentCoor;
    //    }
    //    set
    //    {
    //        currentCoor = value;
    //    }
    //}

    //[SerializeField]
    private PlayerController controller;

    // Use this for initialization
    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<PlayerController>();
        Assert.IsNull(controller, "MazeModel: Cannot reference the controller!");
    }

    public Vector3 GetPosition(Vector2Int coord)
    {
        return transform.InverseTransformPoint(grid[coord.x / 4, coord.y / 4].transform.TransformPoint(Tile.contentPositions[(coord.x % 4) + (coord.y % 4) * 4]));
    }
}
