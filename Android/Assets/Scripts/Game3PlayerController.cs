using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Game3PlayerController : MonoBehaviour
{
    [Header("Game Balance")]
    public int iniHealth;
    public int iniChakra;

    [SerializeField] private float timeToTarget = 0.5f;
    [SerializeField] private float minSwipeDistance = 150;
    [SerializeField] private float shiningTime;
    [SerializeField] private int shineChakraCost;

    [Header("Variables")]
    public int collectableCount = 0;
    public int curHealth;
    public int curChakra;
    [SerializeField]
    private Vector2Int currentCoor;
    public Vector2Int CurrentCoor
    {
        get
        {
            return currentCoor;
        }
        set
        {
            currentCoor = value;
            currentPos = mazeModel.GetPosition(currentCoor);
            
            targetPos = currentPos;
        }
    }
    [SerializeField] private Vector3 currentPos;
    [SerializeField] private Vector3 targetPos;
    private Vector3 mazeCurrentPos;
    private Vector3 mazeTargetPos;
    private float tForLerp;
    private bool moving;
    [SerializeField] private bool dead;
    [SerializeField] private bool canPushEntrance;

    [Header("References")]
    [SerializeField]
    private GameScreen gameScreen;
    [SerializeField] private MazeController mazeController;
    [SerializeField] private MazeModel mazeModel;
    //[SerializeField] private Text healthTxt;
    //[SerializeField] private Text chakraTxt;
    private Animator animator;
    private Animator entranceAnimator;
    [SerializeField] private AudioSource playerFeet;
    private Vector2 m_StartPos;
    [SerializeField] private Transform eyePos;

    private int maxRelAngle;
    private int[] relAngle;
    //private GridSystem.Direction direction;

    // Use this for initialization
    void Start()
    {
        m_StartPos = Vector2.zero;
        relAngle = new int[4];
        tForLerp = 0;
        moving = false;
        canPushEntrance = false;
        dead = false;
        curChakra = iniChakra;
        curHealth = iniHealth;
        //chakraTxt.text = "Chakra: " + chakra;
        //healthTxt.text = "Health: " + health;
        animator = GetComponent<Animator>();
        gameScreen.InitializePlayerUI(iniHealth, iniChakra);
    }

    private void Update()
    {
        if (!dead)
        {
            if (!moving)
            {
                animator.SetInteger("Do", 0);
                playerFeet.Stop();
            }
            else
            {
                animator.SetInteger("Do", 1);
                playerFeet.Play();
                tForLerp += Time.deltaTime / timeToTarget;
                transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
                mazeController.transform.position = Vector3.Lerp(mazeCurrentPos, mazeTargetPos, tForLerp);
                if (tForLerp >= 1)
                {
                    moving = false;
                    currentPos = targetPos;
                    mazeCurrentPos = mazeTargetPos;
                    print("reach target");
                }
            }
        }
    }

    public void Move(int dir)
    {
        if (moving)
            return;

        Vector3 dpadVector;
        if (dir == 0)
            dpadVector = Vector3.forward;
        else if (dir == 1)
            dpadVector = Vector3.back;
        else if (dir == 2)
            dpadVector = Vector3.left;
        else if (dir == 3)
            dpadVector = Vector3.right;
        else
            return;

        dpadVector = Vector3.ProjectOnPlane(dpadVector, transform.parent.up).normalized;

        relAngle[0] = (int)Vector3.Angle(dpadVector, transform.parent.forward);
        relAngle[1] = (int)Vector3.Angle(dpadVector, -transform.parent.forward);
        relAngle[2] = (int)Vector3.Angle(dpadVector, -transform.parent.right);
        relAngle[3] = (int)Vector3.Angle(dpadVector, transform.parent.right);

        maxRelAngle = Mathf.Min(relAngle[0], relAngle[1], relAngle[2], relAngle[3]);
        if (maxRelAngle == relAngle[0])
        {

            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
            if (moving)
            {
                Vector3 rel = transform.TransformDirection(currentPos - targetPos);

                print((currentPos - targetPos).x + ", " + (currentPos - targetPos).y + ", " + (currentPos - targetPos).z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                rel = new Vector3(rel.x * mazeController.transform.localScale.x, rel.y * mazeController.transform.localScale.y,
                    rel.z * mazeController.transform.localScale.z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                mazeCurrentPos = mazeController.transform.position;
                mazeTargetPos = rel + mazeCurrentPos;

                //Rotate the maze to player's forward
                //mazeController.transform.localEulerAngles = new Vector3(0,
                //    -transform.localEulerAngles.y, 0);
                //mazeController.GetComponent<PlayerCentering>().CenterToThePlayer();
            }

            if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Forward)
            {
                OpenTheDoor();
            }
        }
        else if (maxRelAngle == relAngle[1])
        {
            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
            if (moving)
            {
                Vector3 rel = transform.TransformDirection(currentPos - targetPos);
                rel = Quaternion.AngleAxis(180, transform.up) * rel;
                print((currentPos - targetPos).x + ", " + (currentPos - targetPos).y + ", " + (currentPos - targetPos).z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                rel = new Vector3(rel.x * mazeController.transform.localScale.x, rel.y * mazeController.transform.localScale.y,
                    rel.z * mazeController.transform.localScale.z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                mazeCurrentPos = mazeController.transform.position;
                mazeTargetPos = rel + mazeCurrentPos;

                //Rotate the maze to player's forward
                //mazeController.transform.localEulerAngles = new Vector3(0,
                //    -transform.localEulerAngles.y, 0);
                //mazeController.GetComponent<PlayerCentering>().CenterToThePlayer();
            }
            if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Back)
            {
                OpenTheDoor();
            }
        }
        else if (maxRelAngle == relAngle[2])
        {
            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
            if (moving)
            {
                Vector3 rel = (currentPos - targetPos);
                print((currentPos - targetPos).x + ", " + (currentPos - targetPos).y + ", " + (currentPos - targetPos).z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                rel = new Vector3(rel.x * mazeController.transform.localScale.x, rel.y * mazeController.transform.localScale.y,
                    rel.z * mazeController.transform.localScale.z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                mazeCurrentPos = mazeController.transform.position;
                mazeTargetPos = rel + mazeCurrentPos;

                //Rotate the maze to player's forward
                //mazeController.transform.localEulerAngles = new Vector3(0,
                //    -transform.localEulerAngles.y, 0);
                //mazeController.GetComponent<PlayerCentering>().CenterToThePlayer();
            }
            if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Left)
            {
                OpenTheDoor();
            }
        }
        else if (maxRelAngle == relAngle[3])
        {
            moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
            if (moving)
            {
                Vector3 rel = (currentPos - targetPos);
                print((currentPos - targetPos).x + ", " + (currentPos - targetPos).y + ", " + (currentPos - targetPos).z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                rel = new Vector3(rel.x * mazeController.transform.localScale.x, rel.y * mazeController.transform.localScale.y,
                    rel.z * mazeController.transform.localScale.z);
                print(rel.x + ", " + rel.y + ", " + rel.z);
                mazeCurrentPos = mazeController.transform.position;
                mazeTargetPos = rel + mazeCurrentPos;

                //Rotate the maze to player's forward
                //mazeController.transform.localEulerAngles = new Vector3(0,
                 //   -transform.localEulerAngles.y, 0);
                //mazeController.GetComponent<PlayerCentering>().CenterToThePlayer();
            }
            if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Right)
            {
                OpenTheDoor();
            }
        }
        else
            return;
        tForLerp = 0;
    }

    public void Hitted()
    {
        curHealth--;
        //healthTxt.text = "Health: " + health;
        gameScreen.ReduceHealth(1);
        if (curHealth <= 0)
        {
            print("Dead...");
            dead = true;
            animator.SetInteger("Do", 2);
            Invoke("CallOnDeath", 5);
        }
    }

    private void CallOnDeath()
    {
        mazeController.OnDeath();
    }

    public void UseShine()
    {
        curChakra -= shineChakraCost;
        if (curChakra >= 0)
        {
            //chakraTxt.text = "Chakra: " + chakra;
            gameScreen.ReduceChakra(shineChakraCost);
            StartCoroutine(Shining());
        }
    }

    public void GetHeart()
    {
        curHealth++;
        gameScreen.AddHealth();
        //healthTxt.text = "Health: " + health;
    }

    public void GetScroll()
    {
        curChakra++;
        gameScreen.AddChakra();
        //chakraTxt.text = "Chakra: " + chakra;
    }

    void GetCollectable()
    {
        //incomplete
        collectableCount++;
    }

    private IEnumerator Shining()
    {
        print("Shine!");
        transform.GetChild(0).GetComponent<Light>().range *= 4;
        yield return new WaitForSeconds(shiningTime);
        transform.GetChild(0).GetComponent<Light>().range /= 4;
    }

    private void OpenTheDoor()
    {
        animator.SetInteger("Do", 3);
        entranceAnimator.enabled = true;
        canPushEntrance = false;
        mazeController.OnExitEnter();
        //mazeController.DestroyAllEnemies();
        //Invoke("CallOnExitEnter", 5);
    }

    private void CallOnExitEnter()
    {
        mazeController.OnExitEnter();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainEntrance"))
        {
            canPushEntrance = true;
            entranceAnimator = other.gameObject.GetComponent<Animator>();
        }
        else
        {
            canPushEntrance = false;
            entranceAnimator = null;
            if (other.gameObject.CompareTag("Scroll"))
            {
                print("get scroll");
                GetScroll();
                Destroy(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Heart"))
            {
                print("get Heart");
                GetHeart();
                Destroy(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                print("get hitted");
                Hitted();
                other.gameObject.GetComponent<EnemyAI>().OnAttackPlayer();
                Destroy(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Collectable"))
            {
                print("get collectable");
                GetCollectable();
                Destroy(other.gameObject);
            }
        }
    }
}


