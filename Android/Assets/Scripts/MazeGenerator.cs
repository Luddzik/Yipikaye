using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    [Header("Game Balance")]
    [SerializeField]
    private int minRow;
    [SerializeField]
    private int maxRow;
    [SerializeField]
    private int minColumn;
    [SerializeField]
    private int maxColumn;
    [SerializeField, Range(0, 100)]
    private float healthPickupChance;
    [SerializeField, Range(0, 100)]
    private float abilityPickupChance;
    [SerializeField, Range(0, 100)]
    private float deadEndTreasureBonus;
    [SerializeField, Range(0, 100)]
    private float trapChance;
    [SerializeField, Range(0, 100)]
    private float enemyChance;
    [SerializeField, Range(0, 100)]
    private float exitBonusEnemyChance;
    [SerializeField, Range(0, 100)]
    private float blockChance;
    [SerializeField, Range(0, 100)]
    private float collectableChance;
    [SerializeField]
    private PropOption options;
    [SerializeField]
    private int minDistBetweenStartNExit = 1;

    [Header("Variables")]
    public int gameMode;
    [SerializeField]
    private bool isLowest; //is it the lowest floor of castle?
    [SerializeField, Range(0, 100)]
    private float doorChance;
    [SerializeField]
    private float charHeight = 0.72f;
    [SerializeField]
    private float tileHeight = 0.01f;
    [SerializeField]
    private List<Vector2Int> pathwayToExit;
    [SerializeField]
    private List<Vector2Int> allTileVectors;
    private Vector2Int start;
    private Vector2Int exit;
    [SerializeField]
    private int mainEntranceCCWOrder;
    private bool modifying;
    private Vector3 localPtZero;
    private float localsquareLengthX;
    private float localsquareLengthY;
    private List<LineRenderer> m_lineRenderers;
    private Vector3 tileScale;
    private List<Vector2Int> deadEndsCoor;
    public bool mazeGenerated = false;

    [Header("Prefab")]
    public GameObject guardPrefab;
    public GameObject healthPickUpPrefab;
    public GameObject abilityPickUpPrefab;
    public GameObject blockPrefab;
    public GameObject startPtPrefab;
    public GameObject exitPrefab;
    public GameObject mainEntrancePrefab;
    public GameObject floorTilePrefab;
    public GameObject cornerPrefab;
    public GameObject[] pillarPrefabs;
    public GameObject[] indoorWallPrefabs;
    public GameObject[] outdoorWallPrefabs;
    public GameObject linePrefab;

    [Header("Reference")]
    [SerializeField] private Transform tilesParent;
    [SerializeField] private Transform pilarsParent;
    [SerializeField] private Transform interactablesParent;
    [SerializeField] private PlayerController controller;
    [SerializeField] private Game3PlayerController controller2;
    //private CapsuleController controller;
    [SerializeField] private MazeModel mazeModel;
    private MazeController mazeController;
    [SerializeField] private LightManager lightManager;
    //[SerializeField] private GameScreen gameScreen;


    private void Awake()
    {
        pathwayToExit = new List<Vector2Int>();
        allTileVectors = new List<Vector2Int>();
        deadEndsCoor = new List<Vector2Int>();
        //m_lineRenderers = new List<LineRenderer>();
    }



    public void StartGeneration()
    {
        if (mazeGenerated)
            return;
        if (gameMode == 1)
        {
            controller.gameObject.SetActive(true);
        }
        else if(gameMode == 2)
        {
            mazeModel.gameObject.SetActive(true);
        }
        
        mazeController = mazeModel.GetComponent<MazeController>();
        mazeController.gameUI.gameObject.SetActive(true);
        localPtZero = new Vector3(-0.5f, charHeight, -0.5f);
        if (!options.Block) blockChance = 0;
        if (!options.Guard) enemyChance = 0;
        if (!options.Pickup) healthPickupChance = 0;
        if (!options.Pickup) abilityPickupChance = 0;

        for (int i = 0; i < Tile.contentPositions.Length; i++)
            Tile.contentPositions[i].y = charHeight;

        if (gameMode == 1)
        {
            lightManager.AddLight(controller.transform.GetChild(0).GetComponent<Light>());
            lightManager.AddLight(controller.transform.GetChild(1).GetComponent<Light>());
        }
        else if(gameMode == 2)
        {
            lightManager.AddLight(controller2.transform.GetChild(0).GetComponent<Light>());
            lightManager.AddLight(controller2.transform.GetChild(1).GetComponent<Light>());
        }

        SetupGrid();

        lightManager.ResizeNSetIntensity(tileScale.x / 0.3f * mazeModel.Size, PlayerPrefs.GetFloat("SliderBrightness"));

        AudioListener.volume = PlayerPrefs.GetFloat("SliderVolume");
        if(gameMode == 1)
        {
            mazeController.gameMode = 1;
        }
        else if (gameMode == 2)
        {
            mazeController.gameMode = 2;

            mazeModel.GetComponent<PlayerCentering>().CenterToThePlayer();
            //interactablesParent.SetParent(controller.transform);
            //tilesParent.SetParent(controller.transform);
            //pilarsParent.SetParent(controller.transform);
            //controller.transform.localPosition = Vector3.zero;
        }
        //gameScreen.InitializePlayerUI(controller.health, controller.chakra);
        mazeGenerated = true;
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

        //Deal with scale of the player
        if (gameMode == 1)
            controller.transform.localScale = new Vector3(controller.transform.localScale.x * tileScale.x,
                    controller.transform.localScale.y * tileScale.y,
                    controller.transform.localScale.z * tileScale.z);
        else if(gameMode==2)
            controller2.transform.localScale = new Vector3(controller2.transform.localScale.x * tileScale.x,
                    controller2.transform.localScale.y * tileScale.y,
                    controller2.transform.localScale.z * tileScale.z);

        mazeModel.GetComponent<MazeController>().InnerTileLength = tileScale.x / 0.3f * mazeModel.Size;
        ClearTiles();
        //DeleteLineRenderers();
        mazeModel.CenterPos = new Vector3(0, charHeight, 0);
        mazeModel.grid = new Tile[mazeModel.Column, mazeModel.Row];
        mazeModel.impassibles = new bool[mazeModel.Column * 2 + 1, mazeModel.Row + 1];
        //mazeModel.innerGrid = new Tile[mazeModel.Column * 4, mazeModel.Row * 4];
        Vector3 newScale = new Vector3(mazeModel.Size * mazeModel.Column, mazeModel.Size * Mathf.Sqrt(mazeModel.Row * mazeModel.Column), mazeModel.Size * mazeModel.Row);
        //Vector3 scaleDiff = new Vector3(newScale.x / mazeModel.transform.localScale.x,
        //    newScale.y / mazeModel.transform.localScale.y,
        //    newScale.z / mazeModel.transform.localScale.z);

        mazeModel.transform.localScale = newScale;
        

        //controller.transform.GetChild(0).GetComponent<Light>().range *= tileScale.x;
        //controller.transform.GetChild(1).GetComponent<Light>().range *= tileScale.x;

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
        if (isLowest)
        {
            do
            {
                mainEntranceCCWOrder = Random.Range(0, mazeModel.Row * mazeModel.Column); //Starting from bottom-left
                int x = 0, y = 0;
                if (mainEntranceCCWOrder < mazeModel.Column) //Toward -z
                {
                    x = mainEntranceCCWOrder; y = 0;
                }
                else if (mainEntranceCCWOrder < mazeModel.Column + mazeModel.Row) //Toward x
                {
                    x = mazeModel.Column - 1; y = mainEntranceCCWOrder - mazeModel.Column;
                }
                else if (mainEntranceCCWOrder < mazeModel.Column * 2 + mazeModel.Row) //Toward z
                {
                    x = -mainEntranceCCWOrder + mazeModel.Column * 2 + mazeModel.Row - 1;
                    y = mazeModel.Row - 1;
                }
                else  //if (mainEntranceCWOrder < mazeModel.Column * 2 + mazeModel.Row * 2) //Toward -x
                {
                    x = 0;
                    y = -mainEntranceCCWOrder + mazeModel.Column * 2 + mazeModel.Row * 2 - 1;
                }
                exit = new Vector2Int(x, y);
            } while (Mathf.Abs((exit - start).magnitude) <= minDistBetweenStartNExit);
        }
        else
        {
            exit = new Vector2Int(Random.Range(0, mazeModel.Column), Random.Range(0, mazeModel.Row));

            //Re-roll the exit position if the distance from the start point is too small
            if (mazeModel.Row + mazeModel.Column > 4)
            {
                while (Mathf.Abs((exit - start).magnitude) <= minDistBetweenStartNExit)
                {
                    exit = new Vector2Int(Random.Range(0, mazeModel.Column), Random.Range(0, mazeModel.Row));
                }
            }
        }

        //Generate a path to exit
        GenerateExitPathway(start, exit);
        print("Pathway generated");

        //Fill in the walls
        DistributeWalls();

        //Instantiate Tiles 
        for (int j = 0; j < mazeModel.Row; j++)
        {
            for (int i = 0; i < mazeModel.Column; i++)
            {
                //Instantiate tiles
                GameObject tempTile = Instantiate(floorTilePrefab, tilesParent);
                tempTile.transform.localPosition = new Vector3(localPtZero.x + i * localsquareLengthX + localsquareLengthX / 2,
                    tileHeight, localPtZero.z + j * localsquareLengthY + localsquareLengthY / 2);
                tempTile.transform.localRotation = Quaternion.identity;
                tempTile.transform.localScale = tileScale;
                mazeModel.grid[i, j] = tempTile.GetComponent<Tile>();
                mazeModel.grid[i, j].Set(tempTile.transform.localPosition); //isOuter, cantPassFwd, cantPassBk, cantPassLft, cantPassRht, 
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
        //ClearBlockingSquare();

        InstantiateWalls();

        if (isLowest)
            InstantiateMainEntrance();

        print("Walls distributed");
    }

    void InstantiateContents(int i, int j)
    {
        //GameObject tempContent = null;
        bool hasStart = (start.x == i && start.y == j);
        bool hasExit = (exit.x == i && exit.y == j);
        bool hasStair = !isLowest;
        bool isDeadEnd = (deadEndsCoor.Count>0 && deadEndsCoor[0].x == i && deadEndsCoor[0].y == j);

        if (isDeadEnd) deadEndsCoor.RemoveAt(0);

        mazeModel.grid[i, j].CompleteRandomize(hasStart, hasExit, isDeadEnd, hasStair, ref healthPickupChance, ref abilityPickupChance, ref enemyChance, ref blockChance, ref deadEndTreasureBonus, ref exitBonusEnemyChance);
        for (int k = 0; k < Tile.contentPositions.Length; k++)
        {
            GameObject tempContent = null;
            switch (mazeModel.grid[i, j].contents[k].content)
            {
                case Tile.Content.Guard:
                    tempContent = Instantiate(guardPrefab, interactablesParent);
                    tempContent.GetComponent<EnemyAI>().mazeController = mazeModel.GetComponent<MazeController>();
                    tempContent.GetComponent<EnemyAI>().mazeModel = mazeModel;
                    tempContent.GetComponent<EnemyAI>().CurrentCoor = new Vector2Int(i * 4 + k % 4, j * 4 + k / 4);
                    tempContent.name = "Guard " + i+", " + j;
                    mazeModel.GetComponent<MazeController>().AddEnemy(tempContent);
                    break;
                case Tile.Content.Block:
                    tempContent = Instantiate(blockPrefab, interactablesParent); //mazeModel.outerGrid[i, j].transform);
                    tempContent.name = "Block " + i + ", " + j;
                    if (Random.value > 0.5f)
                        tempContent.transform.localEulerAngles = new Vector3(0, 90, 0);
                    break;
                case Tile.Content.HealthPickup:
                    tempContent = Instantiate(healthPickUpPrefab, interactablesParent);
                    tempContent.name = "Pickup " + i + ", " + j;
                    break;
                case Tile.Content.AbilityPickup:
                    tempContent = Instantiate(abilityPickUpPrefab, interactablesParent);
                    tempContent.name = "Pickup " + i + ", " + j;
                    
                    break;
                case Tile.Content.Start:
                    tempContent = Instantiate(startPtPrefab, interactablesParent);
                    tempContent.name = "Start " + i + ", " + j;
                    if(gameMode==1)
                        controller.CurrentCoor = new Vector2Int(i * 4 + k % 4, j * 4 + k / 4);
                    else if(gameMode==2)
                        controller2.CurrentCoor = new Vector2Int(i * 4 + k % 4, j * 4 + k / 4);
                    break;
                case Tile.Content.Exit:
                    tempContent = Instantiate(exitPrefab, interactablesParent);
                    tempContent.name = "Exit " + i + ", " + j;
                    break;
            }
            if (tempContent != null)
            {
                tempContent.transform.localPosition = mazeModel.GetPosition(new Vector2Int(i * 4 + k % 4, j * 4 + k / 4)); //Tile.contentPositions[k]);
                tempContent.transform.localScale = new Vector3(tempContent.transform.localScale.x * tileScale.x,
                    tempContent.transform.localScale.y * tileScale.y,
                    tempContent.transform.localScale.z * tileScale.z);
            }
            if (mazeModel.grid[i, j].contents[k].content == Tile.Content.Start)
            {
                if(gameMode==1)
                    controller.transform.position = tempContent.transform.position;
                else if (gameMode == 2)
                    controller2.transform.position = tempContent.transform.position;
            }
        }
    }

    //void ChangeStartFloorColor()
    //{
    //    mazeModel.grid[exit.x, exit.y].transform.GetChild(1).GetComponent<MeshRenderer>().material = guardPrefab.GetComponent<MeshRenderer>().sharedMaterial;
    //}

    void GenerateExitPathway(Vector2Int start, Vector2Int exit)
    {
        List<Vector2Int> unmanagedTileVectors = new List<Vector2Int>();
        for (int i = 0; i < mazeModel.Column; i++)
            for (int j = 0; j < mazeModel.Row; j++)
            {
                unmanagedTileVectors.Add(new Vector2Int(i, j));
                allTileVectors.Add(unmanagedTileVectors[unmanagedTileVectors.Count - 1]);
            }
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

    void DistributeWalls()
    {
        for (int j = 0; j < mazeModel.impassibles.GetLength(1); j++)
        {
            for(int i = 0; i < mazeModel.impassibles.GetLength(0); i++) 
            {
                //Check if it is on boundary
                if (i == 0 || i == mazeModel.impassibles.GetLength(0) - 1 || (j == 0 && i % 2 == 1) || j == mazeModel.impassibles.GetLength(1) - 1)
                    mazeModel.impassibles[i, j] = true;
                else
                {
                    bool isWallInThePathway = pathwayToExit.Contains(allTileVectors[mazeModel.Row * (i / 2) + j]);
                    int curIndexInPathway = pathwayToExit.IndexOf(allTileVectors[mazeModel.Row * (i / 2) + j]);
                    bool isNotStartOrExit = (curIndexInPathway > 0 && curIndexInPathway!= pathwayToExit.Count-1);
                    if (i % 2 == 1) //wall facing fwd/back
                    {
                        if (isWallInThePathway && isNotStartOrExit && ( //check if the concerning wall is blocking the pathway
                            (((new Vector2Int(i / 2, j)) - pathwayToExit[curIndexInPathway - 1]) == Vector2Int.up) //Is previous step move forward to reach the curent step?
                            || (pathwayToExit[curIndexInPathway + 1] - new Vector2Int(i / 2, j) == Vector2Int.down))) //Or is curent step move backward to reach the next step?
                        {
                            mazeModel.impassibles[i, j] = false;
                        }
                        else
                            mazeModel.impassibles[i, j] = (Random.value > 0.5f) ? true : false;
                    }
                    else //wall facing left/right
                    {
                        if (isWallInThePathway && isNotStartOrExit && //check if the concerning wall is blocking the pathway
                            (((new Vector2Int(i / 2, j)) - pathwayToExit[curIndexInPathway - 1] == Vector2Int.right) //Is previous step move right to reach the curent step?
                            || (pathwayToExit[curIndexInPathway + 1] - new Vector2Int(i / 2, j) == Vector2Int.left))) //Or is curent step move left to reach the next step?
                        {
                            mazeModel.impassibles[i, j] = false;
                        }
                        else
                            mazeModel.impassibles[i, j] = (Random.value > 0.5f) ? true : false;
                    }
                }
            }
        }

        //Break wall surrounding one tile
        for (int j = 0; j < mazeModel.Row; j++)
        {
            for (int i = 0; i < mazeModel.Column; i++)
            {
                if (mazeModel.impassibles[i * 2, j] && mazeModel.impassibles[i * 2 + 1, j] 
                    && mazeModel.impassibles[(i+1) * 2, j] && mazeModel.impassibles[i * 2 + 1, j])
                {
                    print("breaking wall at " + i + ", " + j);
                    List<Vector2Int> wallToBreak = new List<Vector2Int>();
                    if (j != 0)
                        wallToBreak.Add(new Vector2Int(i * 2 + 1, j)); //Add back wall
                    else if (i != 0)
                        wallToBreak.Add(new Vector2Int(i * 2, j));  //Add left wall
                    else if (j != mazeModel.impassibles.GetLength(1) - 2)
                        wallToBreak.Add(new Vector2Int(i * 2 + 1, j)); //Add front wall
                    else if (i == mazeModel.impassibles.GetLength(0) - 3)
                        wallToBreak.Add(new Vector2Int((i + 1) * 2, j)); //Add right wall
                    int index = Random.Range(0, wallToBreak.Count - 1);
                    mazeModel.impassibles[wallToBreak[index].x, wallToBreak[index].y] = false;
                }
                //find the deadends (i.e. block from three sides)
                else if((mazeModel.impassibles[i * 2, j] && mazeModel.impassibles[i * 2 + 1, j] && mazeModel.impassibles[(i + 1) * 2, j]) ||
                    (mazeModel.impassibles[i * 2, j] && mazeModel.impassibles[i * 2 + 1, j] && mazeModel.impassibles[i * 2 + 1, j]) ||
                    (mazeModel.impassibles[i * 2, j] && mazeModel.impassibles[(i + 1) * 2, j] && mazeModel.impassibles[i * 2 + 1, j]) ||
                    (mazeModel.impassibles[i * 2 + 1, j] && mazeModel.impassibles[(i + 1) * 2, j] && mazeModel.impassibles[i * 2 + 1, j]))
                {
                    deadEndsCoor.Add(new Vector2Int(i, j));
                }
            }
        }

        //Zone Mapping
        int zoneCount = 0;
        string zoneEquivStr = "";
        mazeModel.zone = new int[mazeModel.Column * 2 - 1, mazeModel.Row * 2 - 1];
        for (int j = mazeModel.zone.GetLength(1) - 1; j >= 0; j--)
        {
            for (int i = 0; i < mazeModel.zone.GetLength(0); i++)
            {
                if (zoneCount == 0) //new zone
                {
                    zoneCount++;
                    mazeModel.zone[i, j] = zoneCount;
                    mazeModel.zoneEquivalencyArray.Add(0); mazeModel.zoneEquivalencyArray.Add(1);
                }
                else if (i % 2 == 0 && j % 2 == 0) //tile spaze always passable
                {
                    if (j < mazeModel.zone.GetLength(1) - 1 && mazeModel.zone[i, j + 1] != 0) //if not the most upper zone and the upper zone is passable
                    {
                        mazeModel.zone[i, j] = mazeModel.zone[i, j + 1]; //use the same zone as the above zone
                        if (i > 0 && mazeModel.zone[i - 1, j] != 0)  //if not the leftmost zone and the left zone is passable
                            mazeModel.zoneEquivalencyArray[mazeModel.zone[i - 1, j]] = mazeModel.zone[i, j];
                    }
                    else if (i > 0 && mazeModel.zone[i - 1, j] != 0)  //if not the leftmost zone and the left zone is passable
                        mazeModel.zone[i, j] = mazeModel.zone[i - 1, j]; //use the same zone as the left zone
                    else //new zone
                    {
                        zoneCount++;
                        mazeModel.zone[i, j] = zoneCount;
                        mazeModel.zoneEquivalencyArray.Add(zoneCount);
                    }
                }
                else if (j % 2 == 1) //walls/doors facing down
                {
                    if (i % 2 == 1) //pillars must be impassable
                        mazeModel.zone[i, j] = 0;
                    else if (mazeModel.impassibles[i + 1, j - j / 2]) //check the walls/doors impassable value
                        mazeModel.zone[i, j] = 0;
                    else //passable zone
                        mazeModel.zone[i, j] = mazeModel.zone[i, j + 1]; //use the same zone as the above zone
                }
                else if (i % 2 == 1) //walls/doors facing right
                {
                    if (mazeModel.impassibles[i + 1, j - j / 2]) //check the walls/doors impassable value
                        mazeModel.zone[i, j] = 0;
                    else //passable zone
                        mazeModel.zone[i, j] = mazeModel.zone[i - 1, j]; //use the same zone as the left zone
                }

                zoneEquivStr += mazeModel.zone[i, j];
            }
            zoneEquivStr += "\n";
        }
        print("Zone Map: \n" + zoneEquivStr);

        for (int k = 2; k < mazeModel.zoneEquivalencyArray.Count; k++)
        {
            if (mazeModel.zoneEquivalencyArray[k] == k) //Not connecting zone
            {
                //Search for first element of that zone
                for (int j = mazeModel.zone.GetLength(1) - 1; j >= 0; j--)
                {
                    for (int i = 0; i < mazeModel.zone.GetLength(0); i++)
                    {
                        if(mazeModel.zone[i, j] == k)
                        {
                            if(i>0 && mazeModel.zone[i-1, j] == 0) //If there is a impassable next to the zone
                            {
                                //Do zone mapping for that impassable
                                if (j < mazeModel.zone.GetLength(1) - 1 && mazeModel.zone[i - 1, j + 1] != 0 && mazeModel.zone[i - 1, j + 1] != k) //if there is a passable of different zone above it
                                {
                                    mazeModel.zone[i - 1, j] = mazeModel.zone[i - 1, j + 1];
                                    mazeModel.zoneEquivalencyArray[k] = mazeModel.zone[i - 1, j];
                                    mazeModel.impassibles[(i - 1) + 1, j - j / 2] = false;
                                }
                                else if (i - 1 > 0 && mazeModel.zone[i - 2, j] != 0 && mazeModel.zone[i - 2, j] != k) //if there is a passable of different zone next to it
                                {
                                    mazeModel.zone[i - 1, j] = mazeModel.zone[i - 2, j];
                                    mazeModel.zoneEquivalencyArray[k] = mazeModel.zone[i - 1, j];
                                    mazeModel.impassibles[(i - 1) + 1, j - j / 2] = false;
                                }
                            }
                            else if(j< mazeModel.zone.GetLength(1) - 1 && mazeModel.zone[i, j + 1] == 0) //If there is a impassable above the zone
                            {
                                //Do zone mapping for that impassable
                                if (j + 1< mazeModel.zone.GetLength(1) - 1 && mazeModel.zone[i, j + 2] != 0 && mazeModel.zone[i, j + 2] != k) //if there is a passable of different zone above it
                                {
                                    mazeModel.zone[i, j + 1] = mazeModel.zone[i, j + 2];
                                    mazeModel.zoneEquivalencyArray[k] = mazeModel.zone[i, j + 1];
                                    mazeModel.impassibles[i + 1, (j + 1) - (j + 1) / 2] = false;
                                }
                                else if (i > 0 && mazeModel.zone[i - 1, j + 1] != 0 && mazeModel.zone[i - 1, j + 1] != k) //if there is a passable of different zone next to it
                                {
                                    mazeModel.zone[i, j + 1] = mazeModel.zone[i - 1, j + 1];
                                    mazeModel.zoneEquivalencyArray[k] = mazeModel.zone[i, j + 1];
                                    mazeModel.impassibles[i + 1, (j + 1) - (j + 1) / 2] = false;
                                }
                            }
                        }
                    }
                }
            }
        } 


        //print impassibles
        string output="";
        for (int j = 0; j < mazeModel.impassibles.GetLength(1) - 1; j++)
        {
            string rowV = "   ";
            string rowH = "";
            for (int i = 0; i < mazeModel.impassibles.GetLength(0); i++)
            {
                if (i % 2 == 0)
                    rowH += ((mazeModel.impassibles[i, j])? "W":"P") + ((i< mazeModel.impassibles.GetLength(0)-1)?" R ":""); //W = Wall, P = Passible
                else 
                    rowV += ((mazeModel.impassibles[i, j]) ? "W" : " P") + "   ";
            }
            output = rowH + "\n" + rowV + "\n" + output;
        }
        output = "   W   W   W\n" + output;
        print("Finalized impassibles:\n" + output);
    }

    void InstantiateWalls()
    {
        GameObject tempWall = null;
        int randomIndex;

        //Outer walls
        for (int i = 0; i < mazeModel.Column; i++)
        {
            for (int j = 0; j < mazeModel.Row; j++)
            {
                if (j == 0)
                {
                    randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                    tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                    tempWall.name = "Back Wall";
                    tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    mazeModel.grid[i, j].walls[1] = tempWall;
                }
                else if(j == mazeModel.Row - 1)
                {
                    randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                    tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                    tempWall.name = "Fwd Wall";
                    tempWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    mazeModel.grid[i, j].walls[0] = tempWall;
                }
                tempWall = null;
                if (i == 0)
                {
                    randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                    tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                    tempWall.name = "Left Wall";
                    tempWall.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    mazeModel.grid[i, j].walls[2] = tempWall;
                }
                else if (i == mazeModel.Column - 1)
                {
                    randomIndex = Random.Range(0, outdoorWallPrefabs.Length);
                    tempWall = Instantiate(outdoorWallPrefabs[randomIndex], mazeModel.grid[i, j].transform);
                    tempWall.name = "Right Wall";
                    tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    mazeModel.grid[i, j].walls[3] = tempWall;
                }
                tempWall = null;
            }
        }
        //Indoor walls/doors
        for (int i = 1; i < mazeModel.impassibles.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < mazeModel.impassibles.GetLength(1) - 1; j++)
            {
                if(j!=0 || i % 2 != 1) //not the outer walls in the the first row
                {
                    if (i % 2 == 0)
                    {
                        if (mazeModel.impassibles[i, j])
                        {
                            //randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[1], mazeModel.grid[i/2 - 1, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            mazeModel.totalCollectableNum += tempWall.GetComponent<WallCollectables>().RandomizedCollectables(collectableChance);
                            tempWall.name = "Right Wall";
                            lightManager.AddLight(tempWall.transform.GetChild(0).GetComponent<Light>());
                            tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            mazeModel.grid[i / 2 - 1, j].walls[3] = tempWall;
                        }
                        else if (Random.value > doorChance / 100f)
                        {
                            //randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[0], mazeModel.grid[i/2 - 1, j].transform);
                            //tempWall.GetComponent<Animator>().SetBool("toRight", true);
                            tempWall.name = "Right Door";
                            lightManager.AddLight(tempWall.transform.GetChild(0).GetComponent<Light>());
                            tempWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            mazeModel.grid[i / 2 - 1, j].walls[3] = tempWall;
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        if (mazeModel.impassibles[i, j])
                        {
                            //randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[1], mazeModel.grid[i/2, j].transform);
                            tempWall.GetComponent<Animator>().enabled = false;
                            mazeModel.totalCollectableNum += tempWall.GetComponent<WallCollectables>().RandomizedCollectables(collectableChance);
                            tempWall.name = "Back Wall";
                            lightManager.AddLight(tempWall.transform.GetChild(0).GetComponent<Light>());
                            tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            mazeModel.grid[i / 2, j].walls[1] = tempWall;
                        }
                        else if (Random.value > doorChance / 100f)
                        {
                            //randomIndex = Random.Range(0, indoorWallPrefabs.Length);
                            tempWall = Instantiate(indoorWallPrefabs[0], mazeModel.grid[i/2, j].transform);
                            //tempWall.GetComponent<Animator>().SetBool("toRight", false);
                            tempWall.name = "Back Door";
                            lightManager.AddLight(tempWall.transform.GetChild(0).GetComponent<Light>());
                            tempWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                            mazeModel.grid[i / 2, j].walls[1] = tempWall;
                        }
                    }
                    tempWall = null;
                }
            }
        }       
    }

    void InstantiateMainEntrance()
    {
        int x, y;
        if (mainEntranceCCWOrder < mazeModel.Column) //Toward -z
        {
            x = mainEntranceCCWOrder; y = 0;
            print(x +", " + y);
            Destroy(mazeModel.grid[x, y].walls[1]);
            mazeModel.grid[x, y].walls[1] = Instantiate(mainEntrancePrefab, mazeModel.grid[x, y].transform);
            mazeModel.grid[x, y].walls[1].transform.localEulerAngles = new Vector3(0, 180, 0);
            lightManager.AddLight(mazeModel.grid[x, y].walls[1].transform.GetChild(0).GetComponent<Light>());
            mazeModel.mainEntranceFacing = MazeModel.Direction.Back;
            lightManager.AddLight(mazeModel.grid[x, y].walls[1].transform.GetChild(1).GetComponent<Light>());
        }
        else if (mainEntranceCCWOrder < mazeModel.Column + mazeModel.Row) //Toward x
        {
            x = mazeModel.Column - 1; y = mainEntranceCCWOrder - mazeModel.Column;
            print(x + ", " + y);

            Destroy(mazeModel.grid[x, y].walls[3]);
            mazeModel.grid[x, y].walls[3] = Instantiate(mainEntrancePrefab, mazeModel.grid[x, y].transform);
            mazeModel.grid[x, y].walls[3].transform.localEulerAngles = new Vector3(0, 90, 0);
            mazeModel.mainEntranceFacing = MazeModel.Direction.Right;
            lightManager.AddLight(mazeModel.grid[x, y].walls[3].transform.GetChild(0).GetComponent<Light>());
            lightManager.AddLight(mazeModel.grid[x, y].walls[3].transform.GetChild(1).GetComponent<Light>());
        }
        else if (mainEntranceCCWOrder < mazeModel.Column * 2 + mazeModel.Row) //Toward z
        {
            x = -mainEntranceCCWOrder + mazeModel.Column * 2 + mazeModel.Row - 1;
            y = mazeModel.Row - 1;
            print(x + ", " + y);

            Destroy(mazeModel.grid[x, y].walls[0]);
            mazeModel.grid[x, y].walls[0] = Instantiate(mainEntrancePrefab, mazeModel.grid[x, y].transform);
            mazeModel.mainEntranceFacing = MazeModel.Direction.Forward;
            lightManager.AddLight(mazeModel.grid[x, y].walls[0].transform.GetChild(0).GetComponent<Light>());
            lightManager.AddLight(mazeModel.grid[x, y].walls[0].transform.GetChild(1).GetComponent<Light>());
        }
        else if (mainEntranceCCWOrder < mazeModel.Column * 2 + mazeModel.Row * 2) //Toward -x
        {
            x = 0;
            y = -mainEntranceCCWOrder + mazeModel.Column * 2 + mazeModel.Row * 2 - 1;
            print(x + ", " + y);

            Destroy(mazeModel.grid[x, y].walls[2]);
            mazeModel.grid[x, y].walls[2] = Instantiate(mainEntrancePrefab, mazeModel.grid[x, y].transform);
            mazeModel.grid[x, y].walls[2].transform.localEulerAngles = new Vector3(0, -90, 0);
            mazeModel.mainEntranceFacing = MazeModel.Direction.Left;
            lightManager.AddLight(mazeModel.grid[x, y].walls[2].transform.GetChild(0).GetComponent<Light>());
            lightManager.AddLight(mazeModel.grid[x, y].walls[2].transform.GetChild(1).GetComponent<Light>());
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
        lightManager.AddLight(temp.transform.GetChild(0).GetComponent<Light>());
        temp = Instantiate(cornerPrefab, pilarsParent);  //left-top corner
        temp.transform.localPosition = new Vector3(-0.5f, tileHeight, 0.5f);
        temp.transform.localEulerAngles = Vector3.up * -90;
        temp.transform.localScale = new Vector3(tileScale.z, tileScale.y, tileScale.x);
        lightManager.AddLight(temp.transform.GetChild(0).GetComponent<Light>());
        temp = Instantiate(cornerPrefab, pilarsParent); //right-bottom corner
        temp.transform.localPosition = new Vector3(0.5f, tileHeight, -0.5f);
        temp.transform.localEulerAngles = Vector3.up * 90;
        temp.transform.localScale = new Vector3(tileScale.z, tileScale.y, tileScale.x);
        lightManager.AddLight(temp.transform.GetChild(0).GetComponent<Light>());
        temp = Instantiate(cornerPrefab, pilarsParent);  //left-bottom corner
        temp.transform.localPosition = new Vector3(-0.5f, tileHeight, -0.5f);
        temp.transform.localEulerAngles = Vector3.up * 180;
        temp.transform.localScale = tileScale;
        lightManager.AddLight(temp.transform.GetChild(0).GetComponent<Light>());
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