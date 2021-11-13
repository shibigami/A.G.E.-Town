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

    private PathFindingJob[] pathFindingJobsPool;
    private int currentPathfindingIndex;

    public void Awake()
    {
        worldTime = new WorldTime();
        worldMapNodes = WorldMapNodes.Instance;
        worldMapNodes.CreateNodes();
        buildings = new Buildings();
        citizens = new List<GameObject>();

        QualitySettings.vSyncCount = 1;
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

        //initialize threadpool according amount of citizens
        //this is initialized with a higher value in order to recalculate paths that return as invalid
        //this should fix "waiting" citizens behavior
        pathFindingJobsPool = new PathFindingJob[citizens.Count * 2];
        currentPathfindingIndex = -1; //start at -1 due to initial increment
    }

    private void FixedUpdate()
    {
        worldTime.UpdateTime(Time.deltaTime * timeStep);

        //game speed update
        var speedup = true;
        foreach (GameObject go in citizens)
        {
            if (go.GetComponent<Citizen>().GetCurrentAction() == Citizen.CitizenAction.None)
            {
                speedup = false;
                break;
            }
        }

        Time.timeScale = speedup ? 15 : 1;
    }

    public PathFindingJob GetNextAvailablePathfindingJob()
    {
        int nextIndex = currentPathfindingIndex + 1 < pathFindingJobsPool.Length ? currentPathfindingIndex + 1 : 0;

        if (pathFindingJobsPool[nextIndex] == null)
        {
            pathFindingJobsPool[nextIndex] = new PathFindingJob();
        }

        if ((!pathFindingJobsPool[nextIndex].IsDone && !pathFindingJobsPool[nextIndex].pathObtained))
        {
            currentPathfindingIndex = nextIndex;
            return pathFindingJobsPool[currentPathfindingIndex];
        }

        return null;
    }
}
