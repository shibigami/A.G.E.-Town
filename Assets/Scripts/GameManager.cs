using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float timeStep;
    public WorldTime worldTime { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        worldTime = new WorldTime();
    }

    private void FixedUpdate()
    {
        worldTime.UpdateTime(Time.deltaTime * timeStep);
    }
}
