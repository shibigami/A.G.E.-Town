using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float TIMELSLICEUNIT = 0.25f;

    public enum Tags 
    {
        Player,
        Houses
    }

    public static GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
}
