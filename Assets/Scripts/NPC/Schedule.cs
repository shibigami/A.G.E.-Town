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

        //initial setup of schedule
        //working hours: 9-17
        //sleeping hours: 23-7
        //free time: 7-9, 17-23
        for (int i = 0; i < dayProgress.Length; i++)
        {
            dayProgress[i] = 0;

            var action = Citizen.CitizenAction.Play;

            if ((i >= 0 && i < 7 / Constants.TIMELSLICEUNIT) || (i >= 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Sleep;
            }
            else if (i >= 9 / Constants.TIMELSLICEUNIT && i < 17 / Constants.TIMELSLICEUNIT)
            {
                action = Citizen.CitizenAction.Work;
            }
            else if ((i >= 7 / Constants.TIMELSLICEUNIT && i < 9 / Constants.TIMELSLICEUNIT) || (i >= 17 / Constants.TIMELSLICEUNIT && i < 23 / Constants.TIMELSLICEUNIT))
            {
                action = Citizen.CitizenAction.Play;
            }

            allocatedAction[i] = action;
        }
    }

    public Citizen.CitizenAction getActionForTime(float timeInHours)
    {
        return allocatedAction[Mathf.FloorToInt(timeInHours / Constants.TIMELSLICEUNIT)];
    }
}
