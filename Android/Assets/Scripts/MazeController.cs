using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{

    private MazeModel mazeModel;
    
    private Vector3 localPtZero;
    //private float localsquareLengthX;
    //private float localsquareLengthY;
    private float innerTileLength;
    public float InnerTileLength
    {
        get { return innerTileLength; }
        set { innerTileLength = value; }
    }

    [SerializeField] private MazeGenerator mazeGen;
    [SerializeField] private GameObject exitScreen;
    [SerializeField] private GameObject deadScreen;
    [SerializeField] private LightManager lightManager;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject winCam;
    [SerializeField] private LayerMask obstacleMask;

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

    

    public bool MoveCharacter(Transform charTransform, Transform eyePos, ref Vector2Int curCoor, MazeModel.Direction direction, ref Vector3 position, bool restrictInTheSquare)
    {
        RaycastHit hitted;
        switch (direction)
        {
            case MazeModel.Direction.Forward:
                charTransform.localEulerAngles = Vector3.zero;

                //if (character is on the boundary) or (Forward wall in current square is impassible)
                //  don't move
                if (curCoor.y >= mazeModel.Row * 4 - 1 || (curCoor.y % 4 == 3 && mazeModel.grid[curCoor.x/4, curCoor.y/4 + 1].impassable[1]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.y % 4 == 3)
                    return false;
                //if there is a block in the front
                if (mazeModel.grid[curCoor.x / 4, (curCoor.y + 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y + 1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                //Obstacle check
                if (Physics.Raycast(eyePos.position, charTransform.forward, out hitted, innerTileLength, obstacleMask, QueryTriggerInteraction.Collide))
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
                if (mazeModel.grid[curCoor.x / 4, (curCoor.y - 1) / 4].contents[(curCoor.x % 4) + ((curCoor.y - 1) % 4) * 4].content == Tile.Content.Block)
                    return false;
                //Obstacle check
                if (Physics.Raycast(eyePos.position, charTransform.forward, out hitted, innerTileLength, obstacleMask, QueryTriggerInteraction.Collide))
                    return false;
                
                curCoor += Vector2Int.down;
                //print("Move back");
                break;
            case MazeModel.Direction.Left:
                charTransform.localEulerAngles = Vector3.up * -90;

                //if (character is on the boundary) or (Left wall in current square is impassible)
                //  don't move
                if (curCoor.x <= 0 || (curCoor.x % 4 == 0 && mazeModel.grid[curCoor.x / 4 - 1, curCoor.y / 4].impassable[3]))
                    return false;
                //For char. who move only in the same square
                if (restrictInTheSquare && curCoor.x % 4 == 0)
                    return false;
                //if there is a block on the left
                if (mazeModel.grid[(curCoor.x - 1) / 4, (curCoor.y) / 4].contents[((curCoor.x - 1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                //Obstacle check
                if (Physics.Raycast(eyePos.position, charTransform.forward, out hitted, innerTileLength, obstacleMask, QueryTriggerInteraction.Collide))
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
                if (mazeModel.grid[(curCoor.x + 1) / 4, (curCoor.y) / 4].contents[((curCoor.x + 1) % 4) + (curCoor.y % 4) * 4].content == Tile.Content.Block)
                    return false;
                //Obstacle check
                if (Physics.Raycast(eyePos.position, charTransform.forward, out hitted, innerTileLength, obstacleMask, QueryTriggerInteraction.Collide))
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
        //transform.GetChild(0).GetChild(0).GetComponent<Light>().range *= change;
        //transform.GetChild(0).GetChild(1).GetComponent<Light>().range *= change;

        lightManager.Resize(change);
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void OnExitEnter()
    {
        //exitScreen.SetActive(true);
        for (int i = 0; i < enemies.Count; i++)
            Destroy(enemies[i]);
        //Camera.main.gameObject.SetActive(false);
        //winCam.SetActive(false); ;
        Invoke("CallChangeSceneToWin", 8);
    }

    public void CallChangeSceneToWin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("WinScreen", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void OnDeath()
    {
        //deadScreen.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}