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
    private Needs needs;
    private float lastWorldTime;
    private Schedule schedule;

    public GameObject testtarget;

    // Start is called before the first frame update
    void Start()
    {
        moveToPoints = new Node[45];
        try
        {
            animator = GetComponent<Animator>();
        }
        catch
        {
            Debug.Log(name + ": I have no animator... I'll just stand here I guess.");
        }
        myState = CitizenStates.Waiting;

        worldTime = Camera.main.GetComponent<GameManager>().worldTime;

        needs = new Needs();
        schedule = new Schedule();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        needs.UpdateNeeds(worldTime.currentTime - lastWorldTime);

        //state machine
        switch (myState)
        {
            case CitizenStates.Waiting:
                {
                    if (testtarget != null)
                    {
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

                    if (transform.position != testtarget.transform.position)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, testtarget.transform.position, Time.deltaTime * movementSpeed);
                        transform.LookAt(testtarget.transform);
                    }
                    break;
                }
            case CitizenStates.Action:
                {
                    break;
                }
        }

        //save last world time for calculations
        lastWorldTime = worldTime.currentTime;
    }
}
