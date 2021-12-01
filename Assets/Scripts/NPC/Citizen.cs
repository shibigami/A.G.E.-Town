using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    public enum CitizenStates
    {
        Loading,
        Waiting,
        Moving,
        Action,
        Socialize
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

    public enum SocializeStates
    {
        Move,
        Reposition,
        Talk,
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
    public GameObject actionObject { get; private set; }

    private int birthDay;
    private WorldTime worldTime;
    public Needs needs { get; private set; }
    private float lastWorldTime;
    private float deltaWorldTime;
    public Schedule schedule { get; private set; }
    private float LoadingFinishedTime;
    private int pathIndex;
    private bool needsPathing;

    public float oddsOfSocializing;
    public SocializeStates socializingState { get; private set; }
    private bool needsEmptyPoint;
    public bool waitingForResponse { get; private set; }
    public float secondsForBreakoutIfWaitingForPath;
    private float breakoutFromWaitingForPathTick;
    //social point is also used as a flag for when socializing point is needed
    //it is discarded (set to null) when after reaching it
    public Node socialPoint { get; private set; }
    [Tooltip("When in free time, roll every 'value' in world time")]
    public float socialRollTime;
    private float socialRollTick;
    private Vector3 lookAtPoint;
    //gathering point stores social point value before this is discarded
    //for use during social gatherings logic
    private Vector3 gatheringPoint;
    private Vector3 noCollisionGatheringPoint;
    private bool finalGatheringPointSet;
    private float distanceRatio;
    [Tooltip("Lower and Upper limit of randomized social time")]
    public Vector2 socialTimeRange;
    private float socialTimeTick;
    public GameObject bubble;
    private bool socialCitizenNearby;

    public float birthSize;
    public float maxSize;
    public int ageForMaxGrowth;
    private bool grewToday;

    private UI ui;

    // Start is called before the first frame update
    void Start()
    {
        LoadingFinishedTime = Time.time + Constants.CITIZENLOADTIME;
        lineRenderer = GetComponent<LineRenderer>();

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
        needs = new Needs();
        schedule = new Schedule();

        myState = CitizenStates.Loading;
        queuedAction = schedule.getActionForTime(0);
        currentAction = CitizenAction.None;
        previousBuildingAction = CitizenAction.None;
        needsPathing = true;

        worldTime = Constants.gameManager.worldTime;
        birthDay = worldTime.daysElapsed;

        socialRollTick = Time.time + socialRollTime;
        socializingState = SocializeStates.None;
        needsEmptyPoint = false;
        waitingForResponse = false;
        distanceRatio = 0.1f;
        bubble.SetActive(false);

        grewToday = false;

        //ui controls
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UI>();
    }

    private IEnumerator BuildLineRendererPath()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = moveToPoints.Length - pathIndex;
        for (int i = pathIndex; i < moveToPoints.Length; i++)
        {
            Vector3 lineCorners = new Vector3(moveToPoints[i].location.x, 0.1f, moveToPoints[i].location.y);
            lineRenderer.SetPosition(i, lineCorners);
        }

        yield return new WaitForEndOfFrame();
    }

    public Node GetNodeBasedOnAction()
    {
        try
        {
            //actionObject = null;

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
        if (Constants.gameManager.procriationOn)
        {
            if (!grewToday && worldTime.GetCurrentWorldTimeInHours() < 12.0f)
            {
                Grow();
                grewToday = true;
            }
            if (worldTime.GetCurrentWorldTimeInHours() > 12.0f)
            {
                grewToday = false;
            }
        }

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
            case CitizenStates.Loading:
                {
                    if (LoadingFinishedTime < Time.time)
                    {
                        myState = CitizenStates.Waiting;
                    }
                    break;
                }
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

                    if (queuedAction == CitizenAction.Play && !waitingForResponse && socialRollTick < Time.time)
                    {
                        //roll for socializing
                        var roll = Random.Range(1, 100);
                        if (roll >= oddsOfSocializing)
                        {
                            waitingForResponse = true;
                            Constants.gameManager.QueueCitizenForSocializing(this);
                        }
                        socialRollTick = Time.time + socialRollTime / Time.timeScale;
                    }

                    if (waitingForResponse)
                    {
                        if (Constants.gameManager.CitizenAvailableForSocializingExists(this))
                        {
                            moveToPoints = null;
                            pathIndex = 0;
                            needsEmptyPoint = true;
                            myState = CitizenStates.Socialize;
                            socializingState = SocializeStates.Move;
                            breakoutFromWaitingForPathTick = Time.time + secondsForBreakoutIfWaitingForPath;
                            break;
                        }
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

                        needsPathing = true;

                        //check if there is a correct path
                        if (moveToPoints != null && moveToPoints.Length > 1)
                        {
                            var startNode = new Vector2(Mathf.FloorToInt(transform.position.x / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE,
                                Mathf.FloorToInt(transform.position.z / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE);

                            if ((moveToPoints[0].location - startNode).magnitude < WorldMapNodes.NODEDISTANCE * 2.0f)
                            {
                                pathIndex = 0;
                                myState = CitizenStates.Moving;

                                StartCoroutine("BuildLineRendererPath");
                                break;
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
                    needsPathing = false;

                    if (moveToPoints == null || moveToPoints.Length <= 0)
                    {
                        myState = CitizenStates.Waiting;
                        break;
                    }

                    Walk(true);

                    var distanceToPoint = new Vector2(
                        (moveToPoints[pathIndex].location.x - transform.position.x),
                        (moveToPoints[pathIndex].location.y - transform.position.z));

                    //move to next point
                    if (distanceToPoint.magnitude > 0.2f)
                    {
                        var targetPoint = new Vector3(moveToPoints[pathIndex].location.x, transform.position.y, moveToPoints[pathIndex].location.y);
                        transform.position = Vector3.MoveTowards(transform.position, targetPoint, GetMovementSpeed());
                        transform.LookAt(targetPoint);
                    }
                    //reached point
                    else
                    {
                        pathIndex++;

                        //reached end
                        if (moveToPoints.Length - 1 < pathIndex)
                        {
                            //moveToPoints = null;
                            var myLocation = new Vector2(transform.position.x, transform.position.z);
                            if ((GetNodeBasedOnAction().location - myLocation).magnitude < 2.0f)
                            {
                                currentAction = CitizenAction.GoInside;
                                myState = CitizenStates.Action;
                            }
                            else 
                            {
                                ResetToWait();
                            }
                        }
                    }
                    break;
                }
            case CitizenStates.Action:
                {
                    needsPathing = false;

                    switch (currentAction)
                    {
                        case CitizenAction.GoInside:
                            {
                                Walk(true);

                                var targetPosition = actionObject.transform.parent.transform.position;
                                targetPosition.y = transform.position.y;
                                transform.position = Vector3.MoveTowards(transform.position, targetPosition, GetMovementSpeed());

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
                                transform.position = Vector3.MoveTowards(transform.position, targetPosition, GetMovementSpeed());

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
            case CitizenStates.Socialize:
                {
                    switch (socializingState)
                    {
                        case SocializeStates.Move:
                            {
                                needsEmptyPoint = moveToPoints == null;
                                needsPathing = false;
                                if (Time.time > breakoutFromWaitingForPathTick)
                                {
                                    ResetToWait();
                                    break;
                                }
                                if (moveToPoints == null || moveToPoints.Length == 0)
                                {
                                    needsPathing = true;
                                    break;
                                }

                                Walk(true);

                                //reached end
                                if (moveToPoints.Length - 1 < pathIndex)
                                {
                                    gatheringPoint = new Vector3(moveToPoints[moveToPoints.Length - 1].location.x, transform.position.y, moveToPoints[moveToPoints.Length - 1].location.y);
                                    //moveToPoints = null;
                                    //socialPoint = null;
                                    distanceRatio = 0.25f;
                                    finalGatheringPointSet = false;
                                    socializingState = SocializeStates.Reposition;
                                    break;
                                }

                                var distanceToPoint = new Vector2(
                                    (moveToPoints[pathIndex].location.x - transform.position.x),
                                    (moveToPoints[pathIndex].location.y - transform.position.z));

                                //move to next point
                                if (distanceToPoint.magnitude > WorldMapNodes.NODEDISTANCE)
                                {
                                    var targetPoint = new Vector3(moveToPoints[pathIndex].location.x, transform.position.y, moveToPoints[pathIndex].location.y);
                                    transform.position = Vector3.MoveTowards(transform.position, targetPoint, GetMovementSpeed());
                                    transform.LookAt(targetPoint);
                                }
                                //reached point
                                else
                                {
                                    if (pathIndex < moveToPoints.Length)
                                        pathIndex++;
                                }
                                break;
                            }
                        case SocializeStates.Reposition:
                            {
                                //do not move if already in place - should avoid citizen scramble mosh pit
                                Walk(false);

                                bool move = false;
                                if (!finalGatheringPointSet)
                                {
                                    //distance themselves if citizen in the same position
                                    var colliders = Physics.OverlapSphere(transform.position, 0.75f);
                                    Vector3 gatheringDistanceVetor = new Vector3();
                                    Vector3 collisionDistanceVector = new Vector3();
                                    Vector3 unitVector = new Vector3();
                                    foreach (var col in colliders)
                                    {
                                        if (!col.gameObject.GetComponent<Terrain>() || col.gameObject.GetComponent<BoxCollider>())
                                        {
                                            gatheringDistanceVetor = gatheringPoint - transform.position;
                                            collisionDistanceVector = col.transform.position - transform.position;

                                            unitVector = (gatheringDistanceVetor + collisionDistanceVector +
                                                //added random vector in case of 0 values
                                                new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f))
                                                ).normalized;
                                            unitVector.y = 0;
                                            move = true;
                                            distanceRatio = Mathf.Clamp(distanceRatio + 0.25f, 0.1f, 3f);
                                            break;
                                        }
                                    }

                                    if (move)
                                    {
                                        noCollisionGatheringPoint = gatheringPoint + unitVector * distanceRatio;
                                    }
                                    else
                                    {
                                        noCollisionGatheringPoint = gatheringPoint;
                                    }
                                    finalGatheringPointSet = true;
                                }

                                if (transform.position != noCollisionGatheringPoint)
                                {
                                    Walk(true);
                                    transform.position = Vector3.MoveTowards(transform.position, noCollisionGatheringPoint, GetMovementSpeed());
                                    transform.LookAt(new Vector3(noCollisionGatheringPoint.x, transform.position.y, noCollisionGatheringPoint.z));
                                    break;
                                }

                                if (!move && finalGatheringPointSet)
                                {
                                    finalGatheringPointSet = false;
                                    socializingState = SocializeStates.Talk;

                                    //wrong calculation but serves the purpose
                                    //to be replaced with actual world time
                                    socialTimeTick = Time.time + (Random.Range(socialTimeRange.x, socialTimeRange.y) * Constants.TIMELSLICEUNIT) / Time.timeScale * Constants.gameManager.timeStep;

                                    //get closest citizen to look at
                                    var colliders = Physics.OverlapSphere(transform.position, 2.0f);
                                    foreach (var col in colliders)
                                    {
                                        if (col.gameObject != gameObject &&
                                            !col.gameObject.GetComponent<Terrain>() &&
                                            col.gameObject.tag == Constants.Tags.Player.ToString())
                                        {
                                            if ((col.gameObject.transform.position - transform.position).magnitude < (transform.position - lookAtPoint).magnitude)
                                            {
                                                lookAtPoint = col.gameObject.transform.position;
                                                socialCitizenNearby = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case SocializeStates.Talk:
                            {
                                if (socialCitizenNearby)
                                {
                                    if (!bubble.activeSelf)
                                    {
                                        bubble.SetActive(true);
                                    }
                                }

                                Walk(false);

                                transform.LookAt(lookAtPoint);

                                var timeAction = schedule.getActionForTime(Constants.gameManager.worldTime.GetCurrentWorldTimeInHours());

                                if ((transform.position - lookAtPoint).magnitude > 2.0f || Time.time > socialTimeTick || timeAction != CitizenAction.Play)
                                {
                                    ResetToWait();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != Constants.Tags.Player.ToString() && !collision.transform.GetComponent<Terrain>())
        {
            ResetToWait();
        }
    }

    private void ResetToWait()
    {
        finalGatheringPointSet = false;
        waitingForResponse = false;
        needsPathing = false;
        Constants.gameManager.RemoveCitizenFromSocialQueue(this);
        myState = CitizenStates.Waiting;
        bubble.SetActive(false);
        socialCitizenNearby = false;
    }

    private void Walk(bool walk)
    {
        if (animator.GetBool("run") != walk)
        {
            animator.SetBool("run", walk);
        }
    }

    private float GetMovementSpeed()
    {
        return Time.deltaTime * movementSpeed * Constants.gameManager.timeStep;
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

    public bool NeedsPathing()
    {
        return needsPathing;
    }

    public void SetPath(Node[] calculatedPath)
    {
        moveToPoints = calculatedPath;
    }

    public Node GetCurrentNode()
    {
        Node currentLocation = new Node(transform.position.x, transform.position.z);
        return currentLocation;
    }

    public int GetAge()
    {
        if (worldTime == null)
        {
            return 0;
        }
        return worldTime.daysElapsed - birthDay;
    }

    public bool NeedsSocialMovePoint()
    {
        return needsEmptyPoint;
    }

    public void SocialMovePointObtained()
    {
        needsEmptyPoint = false;
    }

    public void SetSocialResponseSuccess()
    {
        waitingForResponse = false;
    }

    public void SetSocialPoint(Node target)
    {
        socialPoint = target;
        SocialMovePointObtained();
    }

    public void SetLookAtPoint(Vector3 point)
    {
        lookAtPoint = point;
    }

    private void OnEnable()
    {
        if (worldTime == null || !Constants.gameManager.procriationOn)
        {
            return;
        }
        birthDay = worldTime.daysElapsed;
        transform.localScale = new Vector3(birthSize, birthSize, birthSize);
    }

    private void Grow()
    {
        //if (!Constants.gameManager.procriationOn || transform.localScale.x>=maxSize)
        //{
        //    return;
        //}
        var newSize = Mathf.Clamp(GetGrowthPerDay() * GetAge(), birthSize, maxSize);
        transform.localScale = Vector3.one * newSize;
        if (transform.localScale.x >= maxSize)
        {
            schedule.SetScheduleForAdult();
        }
    }

    private float GetGrowthPerDay()
    {
        return (maxSize - birthSize) / ageForMaxGrowth;
    }

    private void OnValidate()
    {
        oddsOfSocializing = Mathf.Clamp(oddsOfSocializing, 0, 100);
        socialRollTime = Mathf.Clamp(socialRollTime, 0.2f, 2.0f);
        socialTimeRange = new Vector2(Mathf.Clamp(socialTimeRange.x, 1.0f, socialTimeRange.x),
            Mathf.Clamp(socialTimeRange.y, socialTimeRange.x, float.MaxValue));
        secondsForBreakoutIfWaitingForPath = Mathf.Clamp(secondsForBreakoutIfWaitingForPath, 5, 40);

        birthSize = Mathf.Clamp(birthSize, 0.2f, 2.0f);
        maxSize = Mathf.Clamp(maxSize, birthSize, 2.0f);
        ageForMaxGrowth = Mathf.Clamp(ageForMaxGrowth, 1, 18);
    }
}