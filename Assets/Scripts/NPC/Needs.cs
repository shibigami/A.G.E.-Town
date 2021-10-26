using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needs
{
    public enum NeedsList
    {
        Food,
        Water,
        Sleep,
        Entertainment
    }

    public float hydration;
    public float food;
    public float sleep;
    public float entertainment;

    public Needs()
    {
        hydration = 100;
        food = 100;
        sleep = 100;
        entertainment = 100;
    }

    public void UpdateNeeds(float worldDeltaTime)
    {
        //wdt = world delta time: amount of time passed in the world since the last update
        //full cycle of world delta time = 1440 units (wdt)

        //4 L a day (need citizen action to check for drinking)
        //decrease: 100% = 4L // 4L = 1440wdt // 1wdt = 0.069
        hydration = Mathf.Clamp(hydration -= worldDeltaTime * 0.069f, 0, 100);

        //4 times a day - every 4 hours (inactive hours don't count, need citizen action to check for eating)
        //decrease: 8h = 100% // 8*60 = 480wdt // 100/480 = 0.208333
        food = Mathf.Clamp(food -= worldDeltaTime * 0.2083f, 0, 100);

        //1 time a day - 8 hours to increase (needs citizen action to check for in/activity)
        //decrease: 20h = 100% // 20*60 = 1200wdt // 100/1200 = 0.0833
        sleep = Mathf.Clamp(sleep -= worldDeltaTime * 0.0833f, 0, 100);

        //1 time a day - 1 hour to increase (needs citizen action to check for work/free time)
        //decrease: 20h = 100% // 20*60 = 1200wdt // 100/1200 = 0.0833
        entertainment = Mathf.Clamp(entertainment -= worldDeltaTime * 0.0833f, 0, 100);
    }
}