using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTime
{
    public float currentTime { get; private set; }

    public WorldTime()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    public void UpdateTime(float dt)
    {
        currentTime += dt;
        if (currentTime >= 1440)
        {
            currentTime = 0;
        }
    }

    public float GetCurrentWorldTimeInHours()
    {
        return currentTime / 60;
    }
}
