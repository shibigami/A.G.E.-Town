using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    public enum CitizenStates
    {
        Waiting,
        Moving,
        Action
    }

    public enum CitizenAction
    {
        Work,
        Sleep,
        Eat,
        Drink,
        Play
    }

    public Node[] moveToPoints { get; private set; }
    private Animator animator;
    private CitizenStates myState;
    public float movementSpeed;

    private CitizenAction myAction;


    private WorldTime worldTime;
    public Needs needs { get; private set; }
    private float lastWorldTime;
    public Schedule schedule { get; private set; }

    private UI ui;

    private PathFindingJob pathFindingJob;
    private int pathIndex;
    private bool isCalculatingPath;

    // Start is called before the first frame update
    void Start()
    {
        //pathfinder
        isCalculatingPath = false;

        //animator allocation
        try
        {
            animator = GetComponent<Animator>();
        }
        catch
        {
            Debug.Log(name + ": I have no animator... I'll just stand here I guess.");
        }

        //Ai related
        myState = CitizenStates.Waiting;

        worldTime = Constants.gameManager.worldTime;

        needs = new Needs();
        schedule = new Schedule();

        //ui controls
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UI>();
    }

    private IEnumerator FindPath()
    {
        yield return StartCoroutine(pathFindingJob.WaitFor());
    }

    private void CalculatePath()
    {
        if (!isCalculatingPath)
        {
            isCalculatingPath = true;
            pathFindingJob = new PathFindingJob();
            var startNode = new Node(transform.position.x, transform.position.z);
            var endNode = new Node(GameObject.Find("target").transform.position.x, GameObject.Find("target").transform.position.z);
            pathFindingJob.startNode = startNode;
            pathFindingJob.endNode = endNode;
            pathFindingJob.Start();
            StartCoroutine("FindPath");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (needs == null)
        {
            needs = new Needs();
        }
        else if (worldTime == null)
        {
            worldTime = Constants.gameManager.worldTime;
        }
        else
        {
            needs.UpdateNeeds(worldTime.currentTime - lastWorldTime);
        }

        //state machine
        switch (myState)
        {
            case CitizenStates.Waiting:
                {
                    if (Input.GetButtonDown("Fire1") && !isCalculatingPath && (pathFindingJob == null || !pathFindingJob.IsDone))
                    {
                        CalculatePath();
                    }

                    //if it needs to move somewhere for its action
                    if (pathFindingJob!=null && pathFindingJob.IsDone)
                    {
                        moveToPoints = pathFindingJob.path;
                        //pathFindingJob = null;
                        isCalculatingPath = false;
                        pathIndex = 0;
                        myState = CitizenStates.Moving;
                    }
                    break;
                }
            case CitizenStates.Moving:
                {
                    if (!animator.GetBool("run"))
                    {
                        animator.SetBool("run", true);
                    }

                    var distanceToPoint = new Vector2(
                        (moveToPoints[pathIndex].location.x - transform.position.x),
                        (moveToPoints[pathIndex].location.y - transform.position.z));

                    //move to next point
                    if (distanceToPoint.magnitude > 0.2f)
                    {
                        var targetPoint = new Vector3(moveToPoints[pathIndex].location.x, transform.position.y, moveToPoints[pathIndex].location.y);
                        transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * movementSpeed);
                        transform.LookAt(targetPoint);
                    }
                    //reached point
                    else
                    {
                        pathIndex++;

                        //reached end
                        if (moveToPoints.Length - 1 < pathIndex)
                        {
                            moveToPoints = null;
                            myState = CitizenStates.Waiting;
                        }

                    }
                    break;
                }
            case CitizenStates.Action:
                {
                    break;
                }
        }

        //save last world time for calculations
        if (worldTime == null)
        {
            worldTime = Constants.gameManager.worldTime;
        }
        else
        {
            lastWorldTime = worldTime.currentTime;
        }
    }

    private void OnMouseDown()
    {
        ui.SelectCharacter(gameObject);
    }
}
