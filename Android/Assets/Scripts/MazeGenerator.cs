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
    private float enemyChance;
    [SerializeField, Range(0, 100)]
    private float blockChance;
    [SerializeField]
    private PropOption options;

    [Header("Variables")]
    [SerializeField, Range(0, 100)]
    private float wallChance;
    [SerializeField]
    private float charHeight = 0.72f;
    [SerializeField]
    private float tileHeight = 0.01f;
    [SerializeField]
    private List<Vector2Int> pathwayToExit;
    private List<Vector2Int> unmanagedTileVectors;
    [SerializeField]
    Vector2Int start;
    //[SerializeField]
    //Vector3 startPosition;
    Vector2Int exit;
    private bool modifying;
    private Vector3 localPtZero;
    private float localsquareLengthX;
    private float localsquareLengthY;
    private List<LineRenderer> m_lineRenderers;
    private Vector3 tileScale;

    [Header("Prefab")]
    public GameObject guardPrefab;
    public GameObject pickUpPrefab;
    public GameObject blockPrefab;
    public GameObject startPtPrefab;
    public GameObject exitPrefab;
    public GameObject floorTilePrefab;
    public GameObject cornerPrefab;
    public GameObject[] pillarPrefabs;
    public GameObject[] indoorWallPrefabs;
    public GameObject[] outdoorWallPrefabs;
    public GameObject linePrefab;

    [Header("Reference")]
    public Transform tilesParent;
    public Transform pilarsParent;
    [SerializeField]
    private Swipe controller;
    //private CapsuleController controller;
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
        if (!options.Block) blockChance = 0;
        if (!options.Guard) enemyChance = 0;
        if (!options.Pickup) pickupChance = 0;

        for (int i = 0; i < Tile.contentPositions.Length; i++)
            Tile.contentPositions[i].y = charHeight;

        m_lineRenderers = new List<LineRenderer>();
        SetupGrid();
    }

    private void Update()
    {
        //if (!modifying)
        //    DrawGrid();
    }

    public void SetupGrid()
    {
        modifying = true;
        tileScale = (new Vector3(1f / mazeModel.Column, 1f / Mathf.Sqrt(mazeModel.Column * mazeModel.Row), 1f / mazeModel.Row)) / 4;
        ClearTiles();
        DeleteLineRenderers();
        mazeModel.CenterPos = new Vector3(0, charHeight, 0);
        mazeModel.outerGrid = new Tile[mazeModel.Column, mazeModel.Row];
        //mazeModel.innerGrid = new Tile[mazeModel.Column * 4, mazeModel.Row * 4];
        Vector3 newScale = new Vector3(mazeModel.Size * mazeModel.Column, mazeModel.Size * Mathf.Sqrt(mazeModel.Row * mazeModel.Column), mazeModel.Size * mazeModel.Row);
        //Vector3 scaleDiff = new Vector3(newScale.x / mazeModel.transform.localScale.x,
        //    newScale.y / mazeModel.transform.localScale.y,
        //    newScale.z / mazeModel.transform.localScale.z);

        mazeModel.transform.localScale = newScale;
        //Deal with scale of Capsule
        controller.transform.localScale = new Vector3(controller.transform.localScale.x * tileScale.x,
                controller.transform.localScale.y * tileScale.y,
                controller.transform.localScale.z * tileScale.z);
        controller.transform.GetChild(0).GetComponent<Light>().range *= tileScale.x;
        controller.transform.GetChild(1).GetComponent<Light>().range *= tileScale.x;

        //1f is the local length of the whole maze
        localsquareLengthX = 1f / mazeModel.Column;
        localsquareLengthY = 1f / mazeModel.Row;

        //Distribute Tiles
        DistributeTiles();

        InstantiatePillars();

        InstantiateOuterCorner();

        //controller.transform.position = startPosition;

        //ChangeStartFloorColor();

        //Create LineRenderers
        //for (int i = 0; i < mazeModel.Row + mazeModel.Column + 2; i++)
        //{
        //    GameObject tempLine = Instantiate(linePrefab, mazeModel.transform);
        //    tempLine.transform.localPosition = Vector3.zero;
        //    tempLine.transform.localRotation = Quaternion.identity;
        //    tempLine.name = "Line " + i;
        //    tempLine.GetComponent<LineRenderer>().useWorldSpace = false;
        //    m_lineRenderers.Add(tempLine.GetComponent<LineRenderer>());
        //}

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
        //GameObject tempTile = null;
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
                GameObject tempTile = Instantiate(floorTilePrefab, tilesParent);
                tempTile.transform.localPosition = new Vector3(localPtZero.x + i * localsquareLengthX + localsquareLengthX / 2,
                    tileHeight, localPtZero.z + j * localsquareLengthY + localsquareLengthY / 2);
                tempTile.transform.localRotation = Quaternion.identity;
                tempTile.transform.localScale = tileScale;
                mazeModel.outerGrid[i, j] = tempTile.GetComponent<Tile>();
                mazeModel.outerGrid[i, j].Set(cantPassFwd, cantPassBk, cantPassLft, cantPassRht, isOuter, tempTile.transform.localPosition);
                tempTile.name = "Tile " + i + ", " + j;

                InstantiateContents(i, j);

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

    void InstantiateContents(int i, int j)
    {
        //GameObject tempContent = null;
        bool hasStart = (start.x == i && start.y == j);
        bool hasExit = (exit.x == i && exit.y == j);

        mazeModel.outerGrid[i, j].CompleteRandomize(hasStart, hasExit, ref pickupChance, ref enemyChance, ref blockChance);
        for (int k = 0; k < Tile.contentPositions.Length; k++)
        {
            GameObject tempContent = null;
            switch (mazeModel.outerGrid[i, j].contents[k].content)
            {
                case Tile.Content.Guard:
                    tempContent = Instantiate(guardPrefab, mazeModel.transform.GetChild(1));
                    tempContent.GetComponent<EnemyAI>().mazeController = mazeModel.GetComponent<MazeController>();
                    tempContent.GetComponent<EnemyAI>().mazeModel = mazeModel;
                    tempContent.GetComponent<EnemyAI>().CurrentCoor = new Vector2Int(i * 4 + k % 4, j * 4 + k / 4);
                    tempContent.name = "Guard";
                    break;
                case Tile.Content.Block:
                    tempContent = Instantiate(blockPrefab, mazeModel.transform.GetChild(1)); //mazeModel.outerGrid[i, j].transform);
                    tempContent.name = "Block";
                    break;
                case Tile.Content.Pickup:
                    tempContent = Instantiate(pickUpPrefab, mazeModel.transform.GetChild(1));
                    tempContent.name = "Pickup";
                    break;
                case Tile.Content.Start:
                    tempContent = Instantiate(startPtPrefab, mazeModel.transform.GetChild(1));
                    tempContent.name = "Start";
                    controller.CurrentCoor = new Vector2Int(i * 4 + k % 4, j * 4 + k / 4);
                    break;
                case Tile.Content.Exit:
                    tempContent = Instantiate(exitPrefab, mazeModel.transform.GetChild(1));
                    tempContent.name = "Exit";
                    break;
            }
            if (tempContent != null)
            {
                tempContent.transform.localPosition = mazeModel.GetPosition(new Vector2Int(i * 4 + k % 4, j * 4 + k / 4)); //Tile.contentPositions[k]);
                tempContent.transform.localScale = new Vector3(tempContent.transform.localScale.x * tileScale.x,
                    tempContent.transform.localScale.y * tileScale.y,
                    tempContent.transform.localScale.z * tileScale.z);
            }
            if (mazeModel.outerGrid[i, j].contents[k].content == Tile.Content.Start)
                controller.transform.position = tempContent.transform.position;
        }
    }

    //void ChangeStartFloorColor()
    //{
    //    mazeModel.grid[exit.x, exit.y].transform.GetChild(1).GetComponent<MeshRenderer>().material = guardPrefab.GetComponent<MeshRenderer>().sharedMaterial;
    //}

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
                if (mazeModel.outerGrid[i, j].impassable[0] || (j < mazeModel.Row - 1 && mazeModel.outerGrid[i, j + 1].impassable[1]))
                    blockCount++;
                if (mazeModel.outerGrid[i, j].impassable[1] || (j > 0 && mazeModel.outerGrid[i, j - 1].impassable[0]))
                    blockCount++;
                if (mazeModel.outerGrid[i, j].impassable[2] || (i > 0 && mazeModel.outerGrid[i - 1, j].impassable[3]))
                    blockCount++;
                if (mazeModel.outerGrid[i, j].impassable[3] || (i < mazeModel.Column - 1 && mazeModel.outerGrid[i + 1, j].impassable[2]))
                    blockCount++;
                if (blockCount >= 4)
                {
                    wallCracked = false;
                    do
                    {
                        randomInt = Random.Range(0, 4);
                        if (randomInt == 0 && j < mazeModel.Row - 1) //break fwd
                        {
                            mazeModel.outerGrid[i, j].impassable[0] = false;
                            mazeModel.outerGrid[i, j + 1].impassable[1] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break fwd");
                        }
                        else if (randomInt == 1 && j > 0) //break back
                        {
                            mazeModel.outerGrid[i, j].impassable[1] = false;
                            mazeModel.outerGrid[i, j - 1].impassable[0] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break back");
                        }
                        else if (randomInt == 2 && i > 0) //break left
                        {
                            mazeModel.outerGrid[i, j].impassable[2] = false;
                            mazeModel.outerGrid[i - 1, j].impassable[3] = false;
                            wallCracked = true;
                            print(i + ", " + j + ": break left");
                        }
                        else if (randomInt == 3 && i > mazeModel.Column) //break right
                        {
                            mazeModel.outerGrid[i, j].impassable[3] = false;
                            mazeModel.outerGrid[i + 1, j].impassable[2] = false;
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
        Vector2 tryDir = Vector2.zero; //The direction it try to go to
        Vector2 exitDir = Vector2.zero; //The direction from current coor. to the exit
        int randomInt; //0: fwd, 1:back, 2:left, 3:right
        int struckCount; //Record how many times it strucks
        //bool toWall; //Is it near the wall and has a possibility to stuck?
        //Path-random-finding
        while (!pathwayToExit[pathwayToExit.Count - 1].Equals(exit))
        {
            struckCount = 0;
            do {
                struckCount++;
                //toWall = false;
                randomInt = Random.Range(0, 4); //radomize the direction
                exitDir = exit - current;
                exitDir = exitDir.normalized;

                //Determine if it is safe to follow that direction without stucking itself
                if (randomInt == 0)
                {
                    tryDir = Vector2Int.up;
                    if ((current.x == 0 || current.x == mazeModel.Column - 1) && exitDir.y < 0)
                        tryDir *= -1;
                }
                else if (randomInt == 1)
                {
                    tryDir = Vector2Int.down;
                    if ((current.x == 0 || current.x == mazeModel.Column - 1) && exitDir.y > 0)
                        tryDir *= -1;
                }
                else if (randomInt == 2)
                {
                    tryDir = Vector2Int.left;
                    if ((current.y == 0 || current.y == mazeModel.Row - 1) && exitDir.x > 0)
                        tryDir *= -1;
                }
                else if (randomInt == 3)
                {
                    tryDir = Vector2Int.right;
                    if ((current.y == 0 || current.y == mazeModel.Row - 1) && exitDir.x < 0)
                        tryDir *= -1;
                }
                //if (toWall) {
                //    if (randomInt == 0 && exitDir.y < 0)
                //        tryDir *= -1;
                //    else if (randomInt == 1 && exitDir.y > 0)
                //        tryDir *= -1;
                //    else if (randomInt == 2 && exitDir.x > 0)
                //        tryDir *= -1;
                //    else if (randomInt == 3 && exitDir.x < 0)
                //        tryDir *= -1;
                //}

                tryMove = current + new Vector2Int((int)tryDir.x, (int)tryDir.y);

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
                if (mazeModel.outerGrid[i, j].isOuter)
                {
                    //Forward walls
                    if (j == mazeModel.Row - 1) 
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.name = "Fwd Wall";
                    }
                    else
                    {
                        if (mazeModel.outerGrid[i, j].impassable[0])
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            tempWall.name = "Fwd Wall";
                        }
                        else if(Random.value>wallChance/100f)
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().SetBool("FwdOrRight", true);
                            tempWall.name = "Fwd Door";
                        }
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    tempWall = null;
                    //Backward walls
                    if (j == 0)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.name = "Back Wall";
                    }
                    else
                    {
                        if (mazeModel.outerGrid[i, j].impassable[1])
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            tempWall.name = "Back Wall";
                        }
                        else if (Random.value > wallChance / 100f)
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().SetBool("FwdOrRight", false);
                            tempWall.name = "Back Door";
                        }
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    tempWall = null;
                    //Left walls
                    if (i == 0)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.name = "Left Wall";
                    }
                    else
                    {
                        if (mazeModel.outerGrid[i, j].impassable[2])
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            tempWall.name = "Left Wall";
                        }
                        else if (Random.value > wallChance / 100f)
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().SetBool("FwdOrRight", false);
                            tempWall.name = "Left Door";
                        }
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    tempWall = null;
                    //Right walls
                    if (i == mazeModel.Column - 1)
                    {
                        randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                        tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.name = "Right Wall";
                    }
                    else
                    {
                        if (mazeModel.outerGrid[i, j].impassable[3])
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            tempWall.name = "Right Wall";
                        }
                        else if (Random.value > wallChance / 100f)
                        {
                            randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                            tempWall.GetComponent<Animator>().SetBool("FwdOrRight", true);
                            tempWall.name = "Right Door";
                        }
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    tempWall = null;
                }
                else
                {
                    if (mazeModel.outerGrid[i, j].impassable[0])
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().enabled = false;
                        tempWall.name = "Fwd Wall";
                    }
                    else if (Random.value > wallChance / 100f)
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().SetBool("FwdOrRight", true);
                        tempWall.name = "Fwd Door";
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    tempWall = null;
                    if (mazeModel.outerGrid[i, j].impassable[1])
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().enabled = false;
                        tempWall.name = "Back Wall";
                    }
                    else if (Random.value > wallChance / 100f)
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().SetBool(0, false);
                        tempWall.name = "Back Door";
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    tempWall = null;
                    if (mazeModel.outerGrid[i, j].impassable[2])
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().enabled = false;
                        tempWall.name = "Left Wall";
                    }
                    else if (Random.value > wallChance / 100f)
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().SetBool(0, false);
                        tempWall.name = "Left Door";
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    tempWall = null;

                    if (mazeModel.outerGrid[i, j].impassable[3])
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().enabled = false;
                        tempWall.name = "Right Wall";
                    }
                    else if (Random.value > wallChance / 100f)
                    {
                        randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                        tempWall = Instantiate(indoorWallPrefabs[randomIndex], mazeModel.outerGrid[i, j].transform);
                        tempWall.GetComponent<Animator>().SetBool(0, true);
                        tempWall.name = "Right Door";
                    }
                    if (tempWall != null)
                        tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    tempWall = null;
                }
            }
        }
       
    }

    void InstantiatePillars()
    {
        for (int i = 0; i <= mazeModel.Column; i++)
        {
            for (int j = 0; j <= mazeModel.Row; j++)
            {
                GameObject temp = null;
                if (j == 0 && i == 0 || j == mazeModel.Row && i == mazeModel.Column || j == 0 && i == mazeModel.Column || i == 0 && j == mazeModel.Row)
                    temp = Instantiate(pillarPrefabs[0], pilarsParent);
                else if (j == 0 || i == 0 || j == mazeModel.Row || i == mazeModel.Column)
                    temp = Instantiate(pillarPrefabs[1], pilarsParent);
                else
                    temp = Instantiate(pillarPrefabs[2], pilarsParent);
                temp.transform.localScale = new Vector3(tileScale.x, tileScale.y, tileScale.z);
                temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i, tileHeight, localPtZero.z + 1f / mazeModel.Row * j);
                if (j == mazeModel.Row && i == mazeModel.Column)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i - 0.2f * tileScale.x, tileHeight, localPtZero.z + 1f / mazeModel.Row * j - 0.2f * tileScale.z);
                else if(i == mazeModel.Column)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i - 0.2f * tileScale.x, tileHeight, localPtZero.z + 1f / mazeModel.Row * j);
                else if (j == mazeModel.Row)
                    temp.transform.localPosition = new Vector3(localPtZero.x + 1f / mazeModel.Column * i, tileHeight, localPtZero.z + 1f / mazeModel.Row * j - 0.2f * tileScale.z);
                if ((i == 0 || i == mazeModel.Column) && j > 0 && j < mazeModel.Row)
                {
                    temp.transform.localEulerAngles = new Vector3(0, 90, 0);
                    temp.transform.localScale = new Vector3(temp.transform.localScale.z, temp.transform.localScale.y, temp.transform.localScale.x);
                }
            }
        }
    }

    void InstantiateOuterCorner()
    {
        GameObject temp = null;
        temp = Instantiate(cornerPrefab, pilarsParent); //right-top corner
        temp.transform.localPosition = new Vector3(0.5f, tileHeight, 0.5f);
        temp.transform.localScale = tileScale;
        temp = Instantiate(cornerPrefab, pilarsParent);  //left-top corner
        temp.transform.localPosition = new Vector3(-0.5f, tileHeight, 0.5f);
        temp.transform.localEulerAngles = Vector3.up * -90;
        temp.transform.localScale = new Vector3(tileScale.z, tileScale.y, tileScale.x);
        temp = Instantiate(cornerPrefab, pilarsParent); //right-bottom corner
        temp.transform.localPosition = new Vector3(0.5f, tileHeight, -0.5f);
        temp.transform.localEulerAngles = Vector3.up * 90;
        temp.transform.localScale = new Vector3(tileScale.z, tileScale.y, tileScale.x);
        temp = Instantiate(cornerPrefab, pilarsParent);  //left-bottom corner
        temp.transform.localPosition = new Vector3(-0.5f, tileHeight, -0.5f);
        temp.transform.localEulerAngles = Vector3.up * 180;
        temp.transform.localScale = tileScale;

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
    public bool Block, Guard, Pickup, BallPillarFire, SpikeTrap, FireTrap, GasTrap, CrushingWall, Catapult;
}