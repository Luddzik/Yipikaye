using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour {
    public enum Direction { Forward, Back, Right, Left }

    private Quaternion originalRot;
    private Vector3 centerPos;
    [SerializeField]
    [Range(2, 50)]
    private int row = 2;
    [SerializeField]
    [Range(2, 50)]
    private int column = 2;
    [SerializeField]
    [Range(0.1f, 10f)]
    private float size = 0.7f;
    public GameObject linePrefab;
    [SerializeField]
    private Vector3[,] grid;
    private List<LineRenderer> m_lineRenderers;
    private float lengthOfSquare;
    //[SerializeField]
    private Vector3 localPtZero;
    private float localsquareLengthX;
    private float localsquareLengthY;
    private Swipe controller;
    private bool modifying;

    // Use this for initialization
    void Start () {
        controller = transform.GetChild(0).GetComponent<Swipe>();
        originalRot = transform.rotation;
        m_lineRenderers = new List<LineRenderer>();
        centerPos = new Vector3(0, 0.72f, 0);
        localPtZero = new Vector3(-0.5f, 0.72f, -0.5f);
        modifying = false;
        SetupGrid();
    }

    private void Update()
    {
        if (!modifying)
            DrawGrid();
    }

    private void DrawGrid()
    {
        for (int i = 0; i < column + 1; i++)
        {
            m_lineRenderers[i].SetPosition(0, new Vector3(localPtZero.x + (1f/column) * i, localPtZero.y, localPtZero.z));
            m_lineRenderers[i].SetPosition(1, new Vector3(localPtZero.x + (1f / column) * i, localPtZero.y, localPtZero.z + 1));
        }
        for (int i = 0; i < row + 1; i++)
        {
            m_lineRenderers[i + column + 1].SetPosition(0, new Vector3(localPtZero.x, localPtZero.y, localPtZero.z + (1f / row) * i));
            m_lineRenderers[i + column + 1].SetPosition(1, new Vector3(localPtZero.x + 1, localPtZero.y, localPtZero.z + (1f / row) * i));
        }
    }

    public Vector3 GetPosition(Vector2Int coordinate)
    {
        return (grid[coordinate.x, coordinate.y] + Vector3.up * controller.transform.localScale.y);
    }

    public bool MoveCharacter(ref Vector2Int curCoor, Direction direction, ref Vector3 position)
    {
        switch (direction)
        {
            case Direction.Forward:
                if (curCoor.y >= row - 1)
                    return false;
                curCoor += Vector2Int.up;
                print("Move forward");
                break;
            case Direction.Back:
                if (curCoor.y <= 0)
                    return false;
                curCoor += Vector2Int.down;
                print("Move back");
                break;
            case Direction.Left:
                if (curCoor.x <= 0)
                    return false;
                curCoor += Vector2Int.left;
                print("Move left");
                break;
            case Direction.Right:
                if (curCoor.x >= column - 1)
                    return false;
                curCoor += Vector2Int.right;
                print("Move right");
                break;
        }
        position = GetPosition(curCoor);
        return true;
    }

    public void ChangeRow(float rowNum)
    {
        modifying = true;
        row = (int)rowNum;
        SetupGrid();
        modifying = false;
    }

    public void ChangeColumn(float columnNum)
    {
        modifying = true;
        column = (int)columnNum;
        SetupGrid();
        modifying = false;
    }

    public void ChangeSize(float size)
    {
        this.size = size;
        transform.localScale = new Vector3(size * column, transform.localScale.y, size * row);
    }

    void SetupGrid()
    {
        DeleteLineRenderers();

        grid = new Vector3[column, row];

        transform.localScale = new Vector3(size * column, transform.localScale.y, size * row);
        
        localsquareLengthX = 1f / column;
        localsquareLengthY = 1f / row;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                grid[i, j] = new Vector3(localPtZero.x + i * localsquareLengthX + localsquareLengthX / 2,
                    localPtZero.y, localPtZero.z + j * localsquareLengthY + localsquareLengthY / 2);

                //#region GridPosDebug
                //GameObject temp = new GameObject("Grid[" + i + ", " + j + "]");
                //temp.transform.SetParent(transform);
                //temp.transform.localPosition = grid[i, j];
                //#endregion

                //print("Grid[" + i + ", " + j + "]" + (centerPos - grid[i, j]).magnitude + ", " + Mathf.Max(localsquareLengthX, localsquareLengthY) / 2);
                if ((centerPos - grid[i, j]).magnitude <= Mathf.Max(localsquareLengthX, localsquareLengthY) / 2)
                {
                    centerPos = grid[i, j];
                    controller.CurrentCoor = new Vector2Int(i, j);
                }
            }
        }
        controller.transform.localPosition = centerPos + Vector3.up * controller.transform.localScale.y;

        for (int i = 0; i < row + column + 2; i++)
        {
            GameObject temp = Instantiate(linePrefab, transform);
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;
            temp.name = "Line " + i;
            temp.GetComponent<LineRenderer>().useWorldSpace = false;
            m_lineRenderers.Add(temp.GetComponent<LineRenderer>());
        }
    }

    void DeleteLineRenderers()
    {
        LineRenderer temp;
        int length = m_lineRenderers.Count;
        for (int i = 0; i < m_lineRenderers.Count; i++)
        {
            temp = m_lineRenderers[i];
            Destroy(temp.gameObject);
        }
        m_lineRenderers.Clear();
    }
}
