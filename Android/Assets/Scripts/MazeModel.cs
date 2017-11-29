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
    public Tile[,] outerGrid;
    //public Tile[,] innerGrid;

    public float charHeight;
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
    private CapsuleController controller;

    // Use this for initialization
    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<CapsuleController>();
        Assert.IsNull(controller, "MazeModel: Cannot reference the controller!");
    }

    public Vector3 GetPosition(Vector2Int coord)
    {
        //print(outerGrid[coord.x / 4, coord.y / 4].localPosition.x);
        //print(Tile.contentPositions[(coord.x / 4) + (coord.y / 4) * 4].x);
        //return new Vector3((outerGrid[coord.x/4, coord.y/4].localPosition.x) + (Tile.contentPositions[(coord.x - coord.x / 4) + (coord.y - coord.y / 4) * 4].x),
        //    (outerGrid[coord.x / 4, coord.y / 4].localPosition.y) + (Tile.contentPositions[(coord.x - coord.x / 4) + (coord.y - coord.y / 4) * 4].y),
        //    (outerGrid[coord.x / 4, coord.y / 4].localPosition.z) + (Tile.contentPositions[(coord.x - coord.x / 4) + (coord.y - coord.y / 4) * 4].z));
        return transform.InverseTransformPoint(outerGrid[coord.x / 4, coord.y / 4].transform.TransformPoint(Tile.contentPositions[(coord.x % 4) + (coord.y % 4) * 4]));
    }
}
