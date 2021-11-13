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
        Play,
        GoInside,
        GoOutside,
        None
    }

    public Node[] moveToPoints { get; private set; }
    private Animator animator;
    private CitizenStates myState;
    public float movementSpeed;
    private LineRenderer lineRenderer;

    private CitizenAction currentAction;
    private CitizenAction previousBuildingAction;
    private CitizenAction queuedAction;
    private GameObject actionObject;

    public int age { get; private set; }
    private WorldTime worldTime;
    public Needs needs { get; private set; }
    private float lastWorldTime;
    private float deltaWorldTime;
    public Schedule schedule { get; private set; }

    private UI ui;

    //currently 2 threads are used per path finding calculations
    //this should avoid citizens awaiting for a long time
    private PathFindingJob[] pathFindingJob;
    private int pathIndex;
    private bool[] isCalculatingPath;
    private Node[] pathResult;

    // Start is called before the first frame update
    void Start()
    {
        //pathfinder
        isCalculatingPath = new bool[2] { false, false };
        lineRenderer = GetComponent<LineRenderer>();
        pathFindingJob = new PathFindingJob[2];

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
        currentAction = CitizenAction.None;
        previousBuildingAction = CitizenAction.None;

        worldTime = Constants.gameManager.worldTime;

        needs = new Needs();
        schedule = new Schedule();

        //ui controls
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UI>();
    }

    private IEnumerator FindPath(int index)
    {
        //check if finished
        yield return StartCoroutine(pathFindingJob[index].WaitFor());
    }

    private IEnumerator BuildLineRendererPath()
    {
        yield return new WaitForEndOfFrame();

        lineRenderer.positionCount = moveToPoints.Length;
        for (int i = 0; i < moveToPoints.Length; i++)
        {
            Vector3 lineCorners = new Vector3(moveToPoints[i].location.x, 0.1f, moveToPoints[i].location.y);
            lineRenderer.SetPosition(i, lineCorners);
        }

    }

    //call coroutine and await for each unsuccessful path finding operation needed
    private IEnumerator CalculatePath(int index)
    {
        if (!isCalculatingPath[index])
        {
            pathFindingJob[index] = Constants.gameManager.GetNextAvailablePathfindingJob();

            bool callback = false;

            if (pathFindingJob[index] == null)
            {
                callback = true;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            if (!callback)
            {
                //decide on end node based on current need or schedule
                pathFindingJob[index].endNode = GetNodeBasedOnAction();

                if (pathFindingJob[index].endNode != null)
                {
                    pathFindingJob[index].startNode = new Node(new Vector2(transform.position.x, transform.position.z));
                    //run thread
                    pathFindingJob[index].Start();
                    //listen for finished
                    StartCoroutine("FindPath", index);
                    isCalculatingPath[index] = true;
                }
            }
        }
    }

    private Node GetNodeBasedOnAction()
    {
        try
        {
            switch (queuedAction)
            {
                case CitizenAction.Sleep:
                    {
                        actionObject = Constants.gameManager.buildings.getFacilityForCitizen(gameObject, Buildings.FacilityTypes.Home).transform.gameObject;
                        break;
                    }
                case CitizenAction.Work:
                    {
                        actionObject = Constants.gameManager.buildings.getFacilityForCitizen(gameObject, Buildings.FacilityTypes.Work).transform.gameObject;
                        break;
                    }
                case CitizenAction.Play:
                    {
                        actionObject = Constants.gameManager.buildings.getFacilityForCitizen(gameObject, Buildings.FacilityTypes.Entertainment).transform.gameObject;
                        break;
                    }
                case CitizenAction.Eat:
                    {
                        actionObject = Constants.gameManager.buildings.getFacilityForCitizen(gameObject, Buildings.FacilityTypes.Eat).transform.transform.gameObject;
                        break;
                    }
                case CitizenAction.Drink:
                    {
                        actionObject = Constants.gameManager.buildings.getFacilityForCitizen(gameObject, Buildings.FacilityTypes.Eat).transform.transform.gameObject;
                        break;
                    }
            }

            Vector3 targetPosition = new Vector3();

            if (actionObject != null)
            {
                targetPosition = actionObject.transform.position;
            }

            return new Node(new Vector2(targetPosition.x, targetPosition.z));
        }
        catch
        {
            return null;
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
            deltaWorldTime = worldTime.currentTime - lastWorldTime;
            needs.UpdateNeedsDecrease(deltaWorldTime);
        }

        //state machine
        switch (myState)
        {
            case CitizenStates.Waiting:
                {
                    Walk(false);
                    lineRenderer.positionCount = 0;

                    //assign action according to schedule
                    queuedAction = schedule.getActionForTime(Constants.gameManager.worldTime.GetCurrentWorldTimeInHours());


                    //reassign action in case of need priority
                    needs.CheckNeeds(queuedAction);
                    if (needs.isPrioritisingNeeds())
                    {
                        queuedAction = needs.GetPriority();
                    }

                    //assign path
                    if (currentAction != queuedAction)
                    {
                        //if inside a building
                        if (previousBuildingAction == CitizenAction.GoInside && currentAction != CitizenAction.GoOutside)
                        {
                            myState = CitizenStates.Action;
                            currentAction = CitizenAction.GoOutside;
                            break;
                        }

                        for (int i = 0; i < isCalculatingPath.Length; i++)
                        {
                            if (!isCalculatingPath[i])
                            {
                                StartCoroutine("CalculatePath", i);
                                break;
                            }

                            //if there is a path already calculated
                            pathResult = pathFindingJob[i].getPath();

                            if (pathResult != null)
                            {
                                isCalculatingPath[i] = false;
                            }
                        }

                        //check if there is a correct path, otherwise re-calculate on the next frame
                        if (pathResult != null && pathResult.Length > 0)
                        {
                            var startNode = new Vector2(Mathf.FloorToInt(transform.position.x / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE,
                                Mathf.FloorToInt(transform.position.z / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE);

                            if (pathResult[0].location == startNode)
                            {
                                moveToPoints = pathResult;
                                pathIndex = 0;
                                myState = CitizenStates.Moving;
                                StartCoroutine("BuildLineRendererPath");
                            }
                        }
                    }
                    //if no path to assign then increase need according to current action and delta world time
                    else
                    {
                        needs.UpdateNeedIncrease(deltaWorldTime, currentAction);
                    }
                    break;
                }
            case CitizenStates.Moving:
                {
                    Walk(true);

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

                        //reached end (door)
                        if (moveToPoints.Length - 1 < pathIndex)
                        {
                            moveToPoints = null;
                            pathFindingJob = null;
                            currentAction = CitizenAction.GoInside;
                            myState = CitizenStates.Action;
                        }

                    }
                    break;
                }
            case CitizenStates.Action:
                {
                    switch (currentAction)
                    {
                        case CitizenAction.GoInside:
                            {
                                Walk(true);

                                var targetPosition = actionObject.transform.parent.transform.position;
                                targetPosition.y = transform.position.y;
                                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);

                                transform.LookAt(targetPosition);

                                var distanceToPoint = new Vector2(
                                    (targetPosition.x - transform.position.x),
                                    (targetPosition.z - transform.position.z));

                                if (distanceToPoint.magnitude < 0.2f)
                                {
                                    currentAction = queuedAction;
                                    myState = CitizenStates.Waiting;
                                    previousBuildingAction = CitizenAction.GoInside;
                                }
                                break;
                            }
                        case CitizenAction.GoOutside:
                            {
                                Walk(true);

                                var targetPosition = actionObject.transform.position;
                                targetPosition.y = transform.position.y;
                                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);

                                transform.LookAt(targetPosition);

                                var distanceToPoint = new Vector2(
                                    (targetPosition.x - transform.position.x),
                                    (targetPosition.z - transform.position.z));

                                if (distanceToPoint.magnitude < 0.1f)
                                {
                                    currentAction = CitizenAction.None;
                                    myState = CitizenStates.Waiting;
                                    previousBuildingAction = CitizenAction.GoOutside;
                                }
                                break;
                            }
                    }
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

    private void Walk(bool walk)
    {
        if (animator.GetBool("run") != walk)
        {
            animator.SetBool("run", walk);
        }
    }

    private void OnMouseDown()
    {
        ui.SelectCharacter(gameObject);
    }

    public CitizenAction GetCurrentAction()
    {
        return currentAction;
    }

    public CitizenAction GetQueuedAction()
    {
        return queuedAction;
    }
}