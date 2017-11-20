using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //[SerializeField]
    private Swipe controller;

    // Use this for initialization
    void Awake()
    {
        controller = transform.GetChild(0).GetComponent<Swipe>();
    }

    public Vector3 GetPosition(Vector2Int coordinate)
    {
        return (grid[coordinate.x, coordinate.y].localPosition + Vector3.up * controller.transform.localScale.y);
    }
}
