using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float timeStep;
    public WorldTime worldTime { get; private set; }
    public WorldMapNodes worldMapNodes { get; private set; }
    public Buildings buildings { get; private set; }
    public List<GameObject> citizens { get; private set; }

    public void Awake()
    {
        worldTime = new WorldTime();
        worldMapNodes = WorldMapNodes.Instance;
        worldMapNodes.CreateNodes();
        buildings = new Buildings();
        citizens = new List<GameObject>();
    }

    public void Start()
    {
        //populate citizens list
        var citizenstemp = GameObject.FindGameObjectsWithTag(Constants.Tags.Player.ToString());
        foreach (GameObject cit in citizenstemp)
        {
            citizens.Add(cit);
        }

        var citizenCount = citizenstemp.Length-1;
        //distribute houses
        foreach (GameObject building in buildings.houses.Keys)
        {
            if (citizenCount < 0) break;
            for (int i = 0; i < buildings.houses[building].Length; i++)
            {
                if (citizenCount < 0) break;
                //gameobject list associated with each house is null for that index?
                //then no citizen gameobject is assigned to it
                if (buildings.houses[building][i]==null) 
                {
                    buildings.houses[building][i] = citizenstemp[citizenCount];
                    citizenCount--;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        worldTime.UpdateTime(Time.deltaTime * timeStep);
    }
}
