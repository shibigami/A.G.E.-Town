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

        //distribute houmes and facilities
        foreach (GameObject citizen in citizens)
        {
            buildings.AssignFacility(citizen, Buildings.FacilityTypes.Home);
            buildings.AssignFacility(citizen, Buildings.FacilityTypes.Work);
            buildings.AssignFacility(citizen, Buildings.FacilityTypes.Eat);
            buildings.AssignFacility(citizen, Buildings.FacilityTypes.Entertainment);
        }
    }

    private void FixedUpdate()
    {
        worldTime.UpdateTime(Time.deltaTime * timeStep);
    }
}
