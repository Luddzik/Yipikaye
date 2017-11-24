using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    [Header("Game Balance")]
    [SerializeField, Range(0, 100)]
    private float pickupChance;
    [SerializeField, Range(0, 100)]
    private float trapChance;
    [SerializeField, Range(0, 100)]
    private float EnemyChance;
    [SerializeField, Range(0, 100)]
    private float WallChance;
    [SerializeField]
    private PropOption options;

    [Header("Variables")]
    public float charHeight = 0.72f;
    [SerializeField]
    private float tileHeight = 0.01f;
    [SerializeField]
    private List<Vector2Int> pathwayToExit;
    private List<Vector2Int> unmanagedTileVectors;
    [SerializeField]
    Vector2Int start;
    [SerializeField]
    Vector2Int exit;
    private bool modifying;
    private Vector3 localPtZero;
    private float localsquareLengthX;
    private float localsquareLengthY;
    private List<LineRenderer> m_lineRenderers;
    private Vector3 tileScale;

    [Header("Prefab")]
    public GameObject floorTilePrefab;
    public GameObject[] pillarPrefabs;
    public GameObject[] indoorWallPrefabs;
    public GameObject[] outdoorWallPrefabs;
    public GameObject linePrefab;

    [Header("Reference")]
    public Transform tilesParent;
    public Transform pilarsParent;
    [SerializeField]
    private Swipe controller;
    [SerializeField]
    private MazeModel mazeModel;


    private void Awake()
    {
        pathwayToExit = new List<Vector2Int>();
        unmanagedTileVectors = new List<Vector2Int>();
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
        tileScale = (new Vector3(1f / mazeModel.Column, 1f / Mathf.Sqrt(mazeModel.Column * mazeModel.Row), 1f / mazeModel.Row)) / 4;
        ClearTiles();
        DeleteLineRenderers();
        mazeModel.CenterPos = new Vector3(0, charHeight, 0);
        mazeModel.grid = new Tile[mazeModel.Column, mazeModel.Row];
        Vector3 newScale = new Vector3(mazeModel.Size * mazeModel.Column, mazeModel.Size * Mathf.Sqrt(mazeModel.Row * mazeModel.Column), mazeModel.Size * mazeModel.Row);
        Vector3 scaleDiff = new Vector3(newScale.x / mazeModel.transform.localScale.x,
            newScale.y / mazeModel.transform.localScale.y,
            newScale.z / mazeModel.transform.localScale.z);

        mazeModel.transform.localScale = newScale;
        mazeModel.transform.GetChild(0).localScale = new Vector3(mazeModel.transform.GetChild(0).localScale.x * scaleDiff.x,
                mazeModel.transform.GetChild(0).localScale.y * scaleDiff.y,
                mazeModel.transform.GetChild(0).localScale.z * scaleDiff.z);
        //1f is the local length of the whole maze
        localsquareLengthX = 1f / mazeModel.Column;
        localsquareLengthY = 1f / mazeModel.Row;

        //Distribute Tiles
        DistributeTiles();

        InstantiatePillars();

        controller.transform.localPosition = mazeModel.grid[start.x, start.y].transform.localPosition + Vector3.up * controller.transform.localScale.y;
        controller.CurrentCoor = start;

        ChangeStartFloorColor();

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

        modifying = false;
    }

    void DistributeTiles()
    {
        //Randomize the Start and the Exit
        start = new Vector2Int(Random.Range(0, mazeModel.Column), Random.Range(0, mazeModel.Row));
        exit = new Vector2Int(Random.Range(0, mazeModel.Column), Random.Range(0, mazeModel.Row));
        if(mazeModel.Row + mazeModel.Column > 4)
        {
            while (Mathf.Abs(exit.magnitude - start.magnitude) <= 1)
            {
                exit = new Vector2Int(Random.Range(0, mazeModel.Column), Random.Range(0, mazeModel.Row));
            }
        }
        

        //Generate a path to exit
        GenerateExitPathway(start, exit);
        print("Pathway generated");

        //Distribute walls and Instantiate Tiles
        GameObject temp = null;
        bool isOuter;
        bool cantPassFwd;
        bool cantPassBk;
        bool cantPassLft;
        bool cantPassRht;
        int wallCount;
        List<float> listOfWallProba = new List<float>(); 
        for (int i = 0; i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                cantPassFwd = false; cantPassBk = false; cantPassLft = false; cantPassRht = false;
                isOuter = false;
                wallCount = 0;
                //Make sure there are outer walls on the boundary squares
                if (i == 0)
                {
                    cantPassLft = true;
                    isOuter = true;
                    wallCount++;
                }
                else if (i == mazeModel.Column - 1)
                {
                    cantPassRht = true;
                    isOuter = true;
                    wallCount++;
                }
                if (j == 0)
                {
                    cantPassBk = true;
                    isOuter = true;
                    wallCount++;
                }
                else if (j == mazeModel.Row - 1)
                {
                    cantPassFwd = true;
                    isOuter = true;
                    wallCount++;
                }

                //Randomize whether there are walls on the four sides
                if (!pathwayToExit.Contains(new Vector2Int(i, j)))
                {
                    float wallFwd = 0, wallBk=0, wallRht=0, wallLft=0;
                    if (!cantPassFwd) { wallFwd = Random.value; listOfWallProba.Add(wallFwd); }
                    if (!cantPassBk) { wallBk = Random.value; listOfWallProba.Add(wallBk); }
                    if (!cantPassLft) { wallRht = Random.value; listOfWallProba.Add(wallRht); }
                    if (!cantPassRht) { wallLft = Random.value; listOfWallProba.Add(wallLft); }
                    listOfWallProba.Sort();
                    for (int k = listOfWallProba.Count - 1; k >= 0; k--)
                    {
                        if (wallCount < 3 && listOfWallProba[k] > 0.5f)
                        {
                            if (listOfWallProba[k] == wallFwd) cantPassFwd = true;
                            else if (listOfWallProba[k] == wallBk) cantPassBk = true;
                            else if (listOfWallProba[k] == wallRht) cantPassLft = true;
                            else if (listOfWallProba[k] == wallLft) cantPassRht = true;
                        }
                        else k = -1;
                    }
                }

                //Instantiate tiles
                temp = Instantiate(floorTilePrefab, tilesParent);
                mazeModel.grid[i, j] = temp.GetComponent<Tile>();
                mazeModel.grid[i, j].Set(cantPassFwd, cantPassBk, cantPassLft, cantPassRht, isOuter);
                temp.transform.localPosition = new Vector3(localPtZero.x + i * localsquareLengthX + localsquareLengthX / 2,
                    tileHeight, localPtZero.z + j * localsquareLengthY + localsquareLengthY / 2);
                temp.transform.localRotation = Quaternion.identity;
                temp.transform.localScale = tileScale;
                temp.name = "Tile " + i + ", " + j;

                //Find center position of the maze
                //if ((mazeModel.CenterPos - mazeModel.grid[i, j].localPosition).magnitude <= Mathf.Min(localsquareLengthX, localsquareLengthY) / 2)
                //{
                //    mazeModel.CenterPos = mazeModel.grid[i, j].localPosition;
                //    controller.CurrentCoor = new Vector2Int(i, j);
                //}
            }
        }

        //Check if there are squares blocking from 4 sides
        ClearBlockingSquare();

        InstantiateWalls();

        print("Walls distributed");
    }

    void ChangeStartFloorColor()
    {
        mazeModel.grid[exit.x, exit.y].transform.GetChild(1).GetComponent<MeshRenderer>().material = controller.GetComponent<MeshRenderer>().material;
    }

    void ClearBlockingSquare()
    {
        int blockCount;
        int randomInt;
        bool wallCracked;
        for (int i = 0; i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                blockCount = 0;
                if (mazeModel.grid[i, j].impassable[0] || (j < mazeModel.Row - 1 && mazeModel.grid[i, j + 1].impassable[1]))
                    blockCount++;
                if (mazeModel.grid[i, j].impassable[1] || (j > 0 && mazeModel.grid[i, j - 1].impassable[0]))
                    blockCount++;
                if (mazeModel.grid[i, j].impassable[2] || (i > 0 && mazeModel.grid[i - 1, j].impassable[3]))
                    blockCount++;
                if (mazeModel.grid[i, j].impassable[3] || (i < mazeModel.Column - 1 && mazeModel.grid[i + 1, j].impassable[2]))
                    blockCount++;
                if (blockCount >= 4)
                {
                    wallCracked = false;
                    do
                    {
                        randomInt = Random.Range(0, 4);
                        if (randomInt == 0 && j < mazeModel.Row - 1) //break fwd
                        {
                            mazeModel.grid[i, j].impassable[0] = false;
                            mazeModel.grid[i, j + 1].impassable[1] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break fwd");
                        }
                        else if (randomInt == 1 && j > 0) //break back
                        {
                            mazeModel.grid[i, j].impassable[1] = false;
                            mazeModel.grid[i, j - 1].impassable[0] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break back");
                        }
                        else if (randomInt == 2 && i > 0) //break left
                        {
                            mazeModel.grid[i, j].impassable[2] = false;
                            mazeModel.grid[i - 1, j].impassable[3] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break left");
                        }
                        else if (randomInt == 3 && i > mazeModel.Column) //break right
                        {
                            mazeModel.grid[i, j].impassable[3] = false;
                            mazeModel.grid[i + 1, j].impassable[2] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break right");
                        }
                    } while (!wallCracked);
                    
                }
            }
        }
    }

    void GenerateExitPathway(Vector2Int start, Vector2Int exit)
    {
        for (int i = 0; i < mazeModel.Column; i++)
            for (int j = 0; j < mazeModel.Row; j++)
                unmanagedTileVectors.Add(new Vector2Int(i, j));
        //Add start point coordinate to the path
        pathwayToExit.Add(unmanagedTileVectors[mazeModel.Row * start.x + start.y]);
        //Remove start point coordinate from the unmanaged tiles list
        unmanagedTileVectors.RemoveAt(mazeModel.Row * start.x + start.y);

        Vector2Int backOff = Vector2Int.zero; //Make sure there is a step back, life will be easier
        Vector2Int current = new Vector2Int(start.x, start.y); //Current coordinate to find the next step 
        Vector2Int tryMove = Vector2Int.zero; //The step it try to go to
        Vector2Int tryDir = Vector2Int.zero; //The direction it try to go to
        Vector2 exitDir = Vector2.zero; //The direction from current coor. to the exit
        int randomInt; //0: fwd, 1:back, 2:left, 3:right
        int struckCount; //Record how many times it strucks
        bool toWall; //Is it crashing to the wall?
        //Path-random-finding
        while (!pathwayToExit[pathwayToExit.Count - 1].Equals(exit))
        {
            struckCount = 0;
            do {
                struckCount++;
                toWall = false;
                randomInt = Random.Range(0, 4);
                exitDir = exit - current;
                exitDir = exitDir.normalized;
                if (randomInt == 0)
                {
                    tryDir = Vector2Int.up;
                    if (current.x == 0 || current.x == mazeModel.Column - 1)
                        toWall = true;
                }
                else if (randomInt == 1)
                {
                    tryDir = Vector2Int.down;
                    if (current.x == 0 || current.x == mazeModel.Column - 1)
                        toWall = true;
                }
                else if (randomInt == 2)
                {
                    tryDir = Vector2Int.left;
                    if (current.y == 0 || current.y == mazeModel.Row - 1)
                        toWall = true;
                }
                else if (randomInt == 3)
                {
                    tryDir = Vector2Int.right;
                    if (current.y == 0 || current.y == mazeModel.Row - 1)
                        toWall = true;
                }

                if (toWall && tryDir * -1 == new Vector2Int((int)exitDir.x, (int)exitDir.y))
                    tryDir *= -1;

                tryMove = current + tryDir;

                if (struckCount > 5) //if struck, move back
                {
                    pathwayToExit.Remove(current);
                    if (!unmanagedTileVectors.Contains(current))
                    {
                        unmanagedTileVectors.Add(current);
                        struckCount = 0;
                    }
                    current.Set(backOff.x, backOff.y);
                    backOff = unmanagedTileVectors[unmanagedTileVectors.Count - 2];
                }
            }
            while (!unmanagedTileVectors.Contains(tryMove) || (current.x == 0 && randomInt == 2) || (current.y == 0 && randomInt == 1) || (current.x == mazeModel.Column - 1 && randomInt == 3)
                || (current.y == mazeModel.Row - 1 && randomInt == 0));

            backOff.Set(current.x, current.y);
            current.Set(tryMove.x, tryMove.y);
            unmanagedTileVectors.Remove(current);
            pathwayToExit.Add(new Vector2Int(current.x, current.y));
        }
    }

    void InstantiateWalls()
    {
        GameObject tempWall = null;
        int randomIndex;
        for (int i = 0;i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                if (mazeModel.grid[i, j].impassable[0]) //front side can't pass
                {
                    if (mazeModel.grid[i, j].isOuter && j == mazeModel.Row - 1)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        //tempWall.transform.localPosition = new Vector3()
                    }
                    else
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                        //print(i + ", " + j + " created fwd wall, impassable[0] = " + mazeModel.grid[i, j].impassable[0])
                    }
                }
                if (mazeModel.grid[i, j].impassable[1]) //back side can't pass
                {
                    if (mazeModel.grid[i, j].isOuter && j == 0)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                }
                if (mazeModel.grid[i, j].impassable[2]) //left side can't pass
                {
                    if (mazeModel.grid[i, j].isOuter && i == 0)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, -90, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, -90, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                }
                if (mazeModel.grid[i, j].impassable[3]) //right side can't pass
                {
                    if (mazeModel.grid[i, j].isOuter && i == mazeModel.Column - 1)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                        tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        tempWall.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
       
    }

    void InstantiatePillars()
    {
        GameObject temp = null;
        for (int i = 0; i <= mazeModel.Column; i++)
        {
            for (int j = 0; j <= mazeModel.Row; j++)
            {
                if (j == 0 && i == 0 || j == mazeModel.Row && i == mazeModel.Column || j == 0 && i == mazeModel.Column || i == 0 && j == mazeModel.Row)
                {
                    temp = Instantiate(pillarPrefabs[0], pilarsParent);
                }
                else if (j == 0 || i == 0 || j == mazeModel.Row || i == mazeModel.Column)
                {
                    temp = Instantiate(pillarPrefabs[1], pilarsParent);
                }
                else
                {
                    temp = Instantiate(pillarPrefabs[2], pilarsParent);
                }
                temp.transform.localScale = tileScale;
                temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i, tileHeight, localPtZero.z + 1f / mazeModel.Row * j);
                if (j == mazeModel.Row && i == mazeModel.Column)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i - 0.2f * tileScale.x, tileHeight, localPtZero.z + 1f / mazeModel.Row * j - 0.2f * tileScale.z);
                else if(i == mazeModel.Column)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i - 0.2f * tileScale.x, tileHeight, localPtZero.z + 1f / mazeModel.Row * j);
                else if (j == mazeModel.Row)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i, tileHeight, localPtZero.z + 1f / mazeModel.Row * j - 0.2f * tileScale.z);
                if ((i==0 || i == mazeModel.Column) && j > 0 && j < mazeModel.Row)
                {
                    temp.transform.localEulerAngles = new Vector3(-90, 90, 0);
                    //temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i - 0.2f * tileScale.x, tileHeight, localPtZero.z + 1f / mazeModel.Row * j - 0.2f * tileScale.z);
                }
            }
        }
    }

    private void DrawGrid()
    {
        for (int i = 0; i < mazeModel.Column + 1; i++)
        {
            m_lineRenderers[i].SetPosition(0, new Vector3(localPtZero.x + (1f / mazeModel.Column) * i, tileHeight, localPtZero.z));
            m_lineRenderers[i].SetPosition(1, new Vector3(localPtZero.x + (1f / mazeModel.Column) * i, tileHeight, localPtZero.z + 1));
        }
        for (int i = 0; i < mazeModel.Row + 1; i++)
        {
            m_lineRenderers[i + mazeModel.Column + 1].SetPosition(0, new Vector3(localPtZero.x, tileHeight, localPtZero.z + (1f / mazeModel.Row) * i));
            m_lineRenderers[i + mazeModel.Column + 1].SetPosition(1, new Vector3(localPtZero.x + 1, tileHeight, localPtZero.z + (1f / mazeModel.Row) * i));
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

[System.Serializable]
public class PropOption
{
    public bool Wall, Guard, Pickup, BallPillarFire, SpikeTrap, FireTrap, GasTrap, CrushingWall, Catapult;
}