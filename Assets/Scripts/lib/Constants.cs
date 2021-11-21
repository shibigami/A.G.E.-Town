using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float TIMELSLICEUNIT = 0.25f;
    public const int THREADSPERCITIZEN = 2;
    public const int CITIZENLOADTIME = 0;

    public const int CITIZENSPERHOME = 12;
    public const int CITIZENSPERWORK = 60;
    public const int CITIZENSPERFOOD = 125;
    public const int CITIZENSPERENTERTAINMENT = 60;

    public enum Tags 
    {
        Player,
        Houses,
        WorkFacilities,
        EatingFacilities,
        EntertainmentFacilities
    }

    public static GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
}
