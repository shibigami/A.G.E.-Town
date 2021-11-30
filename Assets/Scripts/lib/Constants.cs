using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float TIMELSLICEUNIT = 0.25f;
    public const int THREADSPERCITIZEN = 2;
    public const int CITIZENLOADTIME = 1;

    public const int CITIZENSPERHOME = 2;
    public const int CITIZENSPERWORK = 10;
    public const int CITIZENSPERFOOD = 10;
    public const int CITIZENSPERENTERTAINMENT = 10;

    public const float SOCIALIZINGTIME = 0.5f;

    public enum Tags 
    {
        Player,
        Houses,
        WorkFacilities,
        EatingFacilities,
        EntertainmentFacilities
    }

    public static GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    public static UI ui = gameManager.transform.gameObject.GetComponent<UI>();
}
