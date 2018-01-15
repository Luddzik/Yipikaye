using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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
    private bool isShining;
    [SerializeField] private Vector2Int currentCoor;
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
    private float tForLerp;
    private bool moving;
    [SerializeField] private bool dead;
    [SerializeField] private bool canPushEntrance;

    [Header("References")]
    [SerializeField] private GameScreen gameScreen;
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
        curHealth = iniHealth;
        curChakra = iniChakra;
        //chakraTxt.text = "Chakra: " + chakra;
        //healthTxt.text = "Health: " + health;
        animator = GetComponent<Animator>();
        gameScreen.InitializePlayerUI(iniHealth, iniChakra);
    }

    //public Text debugText1;
    //public Text debugText2;
    //public GameObject target;

    private void Update()
    {
        if (!dead)
        {
            if (!moving)
            {
                animator.SetInteger("Do", 0);
                playerFeet.Stop();
#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    m_StartPos = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 swipeVector = new Vector3(Input.mousePosition.x - m_StartPos.x, 0, Input.mousePosition.y - m_StartPos.y);

                    if (swipeVector.magnitude < minSwipeDistance)
                        return;

                    swipeVector = Camera.main.transform.TransformDirection(swipeVector.normalized);
                    swipeVector = Vector3.ProjectOnPlane(swipeVector, transform.parent.up).normalized;

                    relAngle[0] = (int)Vector3.Angle(swipeVector, transform.parent.forward);
                    relAngle[1] = (int)Vector3.Angle(swipeVector, -transform.parent.forward);
                    relAngle[2] = (int)Vector3.Angle(swipeVector, -transform.parent.right);
                    relAngle[3] = (int)Vector3.Angle(swipeVector, transform.parent.right);

                    maxRelAngle = Mathf.Min(relAngle[0], relAngle[1], relAngle[2], relAngle[3]);

                    if (maxRelAngle == relAngle[0])
                    {
                        moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                        if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Forward)
                        {
                            OpenTheDoor();
                        }
                    }
                    else if (maxRelAngle == relAngle[1])
                    {
                        moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                        if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Back)
                        {
                            OpenTheDoor();
                        }
                    }
                    else if (maxRelAngle == relAngle[2])
                    {
                        moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                        if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Left)
                        {
                            OpenTheDoor();
                        }
                    }
                    else if (maxRelAngle == relAngle[3])
                    {
                        moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                        if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Right)
                        {
                            OpenTheDoor();
                        }
                    }
                    else
                        return;

                    tForLerp = 0;
                }
#endif
                //debugText1.text = Camera.main.transform.position+"";
                //debugText2.text = target.transform.position+"";
#if UNITY_ANDROID
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            m_StartPos = touch.position;
                            break;
                        case TouchPhase.Ended:
                            Vector3 swipeVector = new Vector3(touch.position.x - m_StartPos.x, touch.position.y - m_StartPos.y, 0);
                            //debugText1.text = swipeVector + "";
                            if (swipeVector.magnitude < minSwipeDistance)
                                return;
                            swipeVector = Camera.main.transform.TransformDirection(swipeVector.normalized);
                            //debugText1.text = swipeVector + "";

                            swipeVector = Vector3.ProjectOnPlane(swipeVector, transform.parent.up).normalized;
                            //debugText2.text = swipeVector + "";
                            relAngle[0] = (int)Vector3.Angle(swipeVector, transform.parent.forward);
                            //print(relAngle[(int)MazeModel.Direction.Forward]);
                            relAngle[1] = (int)Vector3.Angle(swipeVector, -transform.parent.forward);
                            //print(relAngle[(int)MazeModel.Direction.Back]);
                            relAngle[2] = (int)Vector3.Angle(swipeVector, -transform.parent.right);
                            //print(relAngle[(int)MazeModel.Direction.Left]);
                            relAngle[3] = (int)Vector3.Angle(swipeVector, transform.parent.right);
                            //print(relAngle[(int)MazeModel.Direction.Right]);

                            maxRelAngle = Mathf.Min(relAngle[0], relAngle[1], relAngle[2], relAngle[3]);

                             if (maxRelAngle == relAngle[0])
                            {
                                moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Forward, ref targetPos, false);
                                if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Forward)
                                {
                                    OpenTheDoor();
                                }
                            }
                            else if (maxRelAngle == relAngle[1])
                            {
                                moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Back, ref targetPos, false);
                                if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Back)
                                {
                                    OpenTheDoor();
                                }
                            }
                            else if (maxRelAngle == relAngle[2])
                            {
                                moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Left, ref targetPos, false);
                                if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Left)
                                {
                                    OpenTheDoor();
                                }
                            }
                            else if (maxRelAngle == relAngle[3])
                            {
                                moving = mazeController.MoveCharacter(transform, eyePos, ref currentCoor, MazeModel.Direction.Right, ref targetPos, false);
                                if (canPushEntrance && mazeModel.mainEntranceFacing == MazeModel.Direction.Right)
                                {
                                    OpenTheDoor();
                                }
                            }
                            else
                                return;

                            tForLerp = 0;
                            break;
                    }
                }
#endif
            }
            else
            {
                animator.SetInteger("Do", 1);
                playerFeet.Play();
                tForLerp += Time.deltaTime / timeToTarget;
                transform.localPosition = Vector3.Lerp(currentPos, targetPos, tForLerp);
                if (tForLerp >= 1)
                {
                    moving = false;
                    currentPos = targetPos;
                    print("reach target");
                }
            }
        }
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
        print("Use SHine");
        curChakra -= shineChakraCost;
        if (curChakra >= 0 && !isShining)
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
        isShining = true;
        transform.GetChild(0).GetComponent<Light>().range *= 4;
        yield return new WaitForSeconds(shiningTime);
        transform.GetChild(0).GetComponent<Light>().range /= 4;
        isShining = false;
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
                other.GetComponent<AudioSource>().PlayOneShot(other.GetComponent<AudioSource>().clip);
                Destroy(other.gameObject);
            }
            else if (other.gameObject.CompareTag("Heart"))
            {
                print("get Heart");
                GetHeart();
                other.GetComponent<AudioSource>().PlayOneShot(other.GetComponent<AudioSource>().clip);
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


