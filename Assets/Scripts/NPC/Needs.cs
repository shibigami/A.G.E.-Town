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
        Fun
    }

    public float hydration;
    public float food;
    public float sleep;
    public float fun;

    public Needs()
    {
        hydration = 100;
        food = 100;
        sleep = 100;
        fun = 100;
    }

    public void UpdateNeeds(float worldDeltaTime)
    {
        //4 L a day (need citizen action to check for drinking)
        //decrease: 100% = 4L // 4L = 1440wdt // 1wdt = 0.069
        hydration = Mathf.Clamp(hydration -= worldDeltaTime * 0.069f, 0, 100);

        //4 times a day - every 4 hours (inactive hours don't count, need citizen action to check for eating)
        //decrease: 8h = 100% // 8*60 = 480wdt // 100/480 = 20.8333
        food = Mathf.Clamp(food -= worldDeltaTime * 20.8333f, 0, 100);

        //1 time a day - 8 hours to increase (needs citizen action to check for in/activity)
        //decrease: 16h = 100% // 16*60 = 960wdt // 100/960 = 0.1041
        sleep = Mathf.Clamp(sleep -= worldDeltaTime * 0.1041f, 0, 100);

        //1 time a day - 1 hour to increase (needs citizen action to check for work/free time)
        //decrease: 12h = 100% // 12*60 = 720wdt // 100/720 = 0.1388
        fun = Mathf.Clamp(fun -= worldDeltaTime * 0.1388f, 0, 100);
    }
}