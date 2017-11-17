using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public GameObject floorTilePrefab;
    public GameObject wallTilePrefab;
    public GameObject linePrefab;
    public Transform tilesParent;
    [SerializeField]
    private Swipe controller;
    [SerializeField]
    private MazeModel mazeModel;
    public float charHeight = 0.72f;
    [SerializeField]
    private float tileHeight = 0.0029f;
    [SerializeField]
    private List<Tile> managedTiles;
    [SerializeField]
    private List<Tile> unmanagedTiles;

    private bool modifying;
    private Vector3 localPtZero;
    private float localsquareLengthX;
    private float localsquareLengthY;
    

    private List<LineRenderer> m_lineRenderers;

    private void Awake()
    {
        managedTiles = new List<Tile>();
        unmanagedTiles = new List<Tile>();
    }

    private void Start()
    {
        localPtZero = new Vector3(-0.5f, charHeight, -0.5f);
        m_lineRenderers = new List<LineRenderer>();
        SetupGrid();
    }

    private void Update()
    {
        if (!modifying)
            DrawGrid();
    }

    public void SetupGrid()
    {
        modifying = true;
        ClearTiles();
        DeleteLineRenderers();
        mazeModel.CenterPos = new Vector3(0, charHeight, 0);
        mazeModel.grid = new Tile[mazeModel.Column, mazeModel.Row];
        Vector3 newScale = new Vector3(mazeModel.Size * mazeModel.Column, mazeModel.transform.localScale.y, mazeModel.Size * mazeModel.Row);
        Vector3 scaleDiff = new Vector3(newScale.x / mazeModel.transform.localScale.x,
            newScale.y / mazeModel.transform.localScale.y,
            newScale.z / mazeModel.transform.localScale.z);

        mazeModel.transform.localScale = newScale;
        mazeModel.transform.GetChild(0).localScale = new Vector3(mazeModel.transform.GetChild(0).localScale.x / scaleDiff.x,
                mazeModel.transform.GetChild(0).localScale.y / scaleDiff.y,
                mazeModel.transform.GetChild(0).localScale.z / scaleDiff.z);

        //1f is the local length of the whole maze
        localsquareLengthX = 1f / mazeModel.Column;
        localsquareLengthY = 1f / mazeModel.Row;

        //Position Tiles
        for (int i = 0; i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                mazeModel.grid[i, j] = new Tile(Tile.Type.Floor, new Vector3(localPtZero.x + i * localsquareLengthX + localsquareLengthX / 2,
                    localPtZero.y, localPtZero.z + j * localsquareLengthY + localsquareLengthY / 2));
                unmanagedTiles.Add(mazeModel.grid[i, j]);

                if ((mazeModel.CenterPos - mazeModel.grid[i, j].localPosition).magnitude <= Mathf.Min(localsquareLengthX, localsquareLengthY) / 2)
                {
                    mazeModel.CenterPos = mazeModel.grid[i, j].localPosition;
                    controller.CurrentCoor = new Vector2Int(i, j);
                }
            }
        }
        controller.transform.localPosition = mazeModel.CenterPos + Vector3.up * controller.transform.localScale.y;

        //Create LineRenderers
        for (int i = 0; i < mazeModel.Row + mazeModel.Column + 2; i++)
        {
            GameObject temp = Instantiate(linePrefab, mazeModel.transform);
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;
            temp.name = "Line " + i;
            temp.GetComponent<LineRenderer>().useWorldSpace = false;
            m_lineRenderers.Add(temp.GetComponent<LineRenderer>());
        }

        DistributeTileType();

        InstantiateTiles();

        modifying = false;
    }

    void DistributeTileType()
    {
        int columnsOfWalls = Mathf.CeilToInt(mazeModel.Column / 2f) - 2;
        int rowsOfWalls = Mathf.CeilToInt(mazeModel.Row / 2f) - 2;
        bool[] toRight;
        bool[] toFwd;

        for (int i = 0; i < columnsOfWalls; i++)
        {

        }

        for (int i = 0; i < mazeModel.Column; i++)
        {

            for (int j = 0; j < mazeModel.Row; j++)
            {
                //Distribute Bornder walls
                if (i == 0 || j == 0 || i == mazeModel.Column - 1 || j == mazeModel.Row - 1)
                {
                    mazeModel.grid[i, j].type = Tile.Type.OneSideWall;
                    unmanagedTiles.Remove(mazeModel.grid[i, j]);
                    managedTiles.Add(mazeModel.grid[i, j]);
                }

                //Distribute main walls
                

            }
        }
    }

    void InstantiateTiles()
    {
        Vector3 tileScale = new Vector3(1f / mazeModel.Column, 1, 1f / mazeModel.Row);
        for (int i = 0; i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                //Instantiate tiles
                GameObject temp = null;
                switch(mazeModel.grid[i, j].type)
                {
                    case Tile.Type.Floor:
                        temp = Instantiate(floorTilePrefab, tilesParent);
                        break;
                    case Tile.Type.OneSideWall:
                        temp = Instantiate(wallTilePrefab, tilesParent);
                        break;
                    default:
                        print("Please define tile type!!");
                        break;
                }
                temp.transform.localPosition = new Vector3(mazeModel.grid[i, j].localPosition.x, 
                    tileHeight, mazeModel.grid[i, j].localPosition.z);
                temp.transform.localRotation = Quaternion.identity;
                temp.transform.localScale = tileScale;
                temp.name = "Tile " + i + ", " + j;
            }
        }
    }

    private void DrawGrid()
    {
        for (int i = 0; i < mazeModel.Column + 1; i++)
        {
            m_lineRenderers[i].SetPosition(0, new Vector3(localPtZero.x + (1f / mazeModel.Column) * i, localPtZero.y, localPtZero.z));
            m_lineRenderers[i].SetPosition(1, new Vector3(localPtZero.x + (1f / mazeModel.Column) * i, localPtZero.y, localPtZero.z + 1));
        }
        for (int i = 0; i < mazeModel.Row + 1; i++)
        {
            m_lineRenderers[i + mazeModel.Column + 1].SetPosition(0, new Vector3(localPtZero.x, localPtZero.y, localPtZero.z + (1f / mazeModel.Row) * i));
            m_lineRenderers[i + mazeModel.Column + 1].SetPosition(1, new Vector3(localPtZero.x + 1, localPtZero.y, localPtZero.z + (1f / mazeModel.Row) * i));
        }
    }

    void ClearTiles()
    {
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            Destroy(tilesParent.GetChild(i).gameObject);
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
