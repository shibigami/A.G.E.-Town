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

    private Node[] moveToPoints;
    private Animator animator;
    private CitizenStates myState;
    public float movementSpeed;

    private CitizenAction myAction;


    private WorldTime worldTime;
    public Needs needs { get; private set; }
    private float lastWorldTime;
    public Schedule schedule { get; private set; }

    private UI ui;

    private PathFinder pathFinder;
    private int pathIndex;

    // Start is called before the first frame update
    void Start()
    {
        //test path
        pathFinder = new PathFinder();
        moveToPoints = pathFinder.FindPath(new Node(transform.position.x, transform.position.z),
            new Node(GameObject.Find("house1").transform.position.x, GameObject.Find("house1").transform.position.z));
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

        worldTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().worldTime;

        needs = new Needs();
        schedule = new Schedule();

        //ui controls
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UI>();
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
            worldTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().worldTime;
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
                    if (moveToPoints != null && moveToPoints.Length > 0)
                    {
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

                    //move to next point
                    if ((transform.position.x != moveToPoints[pathIndex].location.x) && (transform.position.y != moveToPoints[pathIndex].location.y))
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
            worldTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().worldTime;
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
