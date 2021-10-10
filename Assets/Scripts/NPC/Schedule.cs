using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule
{
    private float[] dayProgress;
    private Citizen.CitizenAction[] allocatedAction;

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

            if (i >= 23 && i < 7)
            {
                action = Citizen.CitizenAction.Sleep;
            }
            else if (i >= 9 && i < 17)
            {
                action = Citizen.CitizenAction.Work;
            }
            else if ((i >= 7 && i < 9) || (i >= 17 && i < 23))
            {
                action = Citizen.CitizenAction.Play;
            }

            allocatedAction[i] = action;
        }
    }
}
