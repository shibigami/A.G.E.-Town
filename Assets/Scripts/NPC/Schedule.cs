using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule
{
    public float[] dayProgress { get; private set; }
    public Citizen.CitizenAction[] allocatedAction { get; private set; }

    public Schedule()
    {
        dayProgress = new float[Mathf.CeilToInt(24 / Constants.TIMELSLICEUNIT)];
        allocatedAction = new Citizen.CitizenAction[dayProgress.Length];

        if (Constants.gameManager.procriationOn)
        {
            SetScheduleForChild();
        }
        else 
        {
            SetScheduleForAdult();
        }
    }

    public Citizen.CitizenAction getActionForTime(float timeInHours)
    {
        var slot = Mathf.FloorToInt(timeInHours / Constants.TIMELSLICEUNIT);
        return allocatedAction[Mathf.FloorToInt(timeInHours / Constants.TIMELSLICEUNIT)];
    }

    public void ReplaceInSchedule(Citizen.CitizenAction newAction, int timeSlot)
    {
        allocatedAction[timeSlot] = newAction;
    }

    public void SetScheduleForChild()
    {
        //initial setup of schedule
        //sleeping hours: 8
        //free time: 16
        for (int i = 0; i < dayProgress.Length; i++)
        {
            dayProgress[i] = 0;

            //initialize
            var action = Citizen.CitizenAction.Play;

            if ((i >= 0 && i < 7 / Constants.TIMELSLICEUNIT) || (i >= 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Sleep;
            }
            else if ((i >= 7 / Constants.TIMELSLICEUNIT && i < 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Play;
            }

            allocatedAction[i] = action;
        }
    }

    public void SetScheduleForAdult()
    {
        //initial setup of schedule
        //working hours: 8
        //sleeping hours: 8
        //free time: 8

        int workStartTime = Random.Range(7, 15);

        for (int i = 0; i < dayProgress.Length; i++)
        {
            dayProgress[i] = 0;

            var action = Citizen.CitizenAction.Play;

            if ((i >= 0 && i < 7 / Constants.TIMELSLICEUNIT) || (i >= 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Sleep;
            }
            else if (i >= workStartTime / Constants.TIMELSLICEUNIT && i < (workStartTime + 8) / Constants.TIMELSLICEUNIT)
            {
                action = Citizen.CitizenAction.Work;
            }
            else if ((i >= 7 / Constants.TIMELSLICEUNIT && i < workStartTime / Constants.TIMELSLICEUNIT) || (i >= (workStartTime + 8) / Constants.TIMELSLICEUNIT && i < 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Play;
            }

            allocatedAction[i] = action;
        }
    }
}
