using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float timeStep;
    public WorldTime worldTime { get; private set; }
    public WorldMapNodes worldMapNodes { get; private set; }

    public void Awake()
    {
        worldTime = new WorldTime();
        worldMapNodes = WorldMapNodes.Instance;
        worldMapNodes.CreateNodes();
    }

    private void FixedUpdate()
    {
        worldTime.UpdateTime(Time.deltaTime * timeStep);
    }
}
