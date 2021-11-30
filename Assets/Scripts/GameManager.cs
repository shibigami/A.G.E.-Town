using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool initialized;

    public float timeStep;
    public WorldTime worldTime { get; private set; }
    public WorldMapNodes worldMapNodes { get; private set; }
    public Buildings buildings { get; private set; }
    public List<GameObject> citizens { get; private set; }

    private PathFindingJob[] pathFindingJobsPool;
    private int currentPathfindingIndex;

    private bool canManipulateLists;

    //public float decreaseNodesMoveCostInterval;
    //private float nextNodeMoveCostDecreaseTime;

    public List<Citizen> socialCitizens { get; private set; }
    public List<Citizen> senders { get; private set; }
    public List<Citizen> requesters { get; private set; }

    public void Awake()
    {
        initialized = false;

        worldTime = new WorldTime();
        worldMapNodes = WorldMapNodes.Instance;
        worldMapNodes.CreateNodes();
        buildings = new Buildings();
        citizens = new List<GameObject>();

        QualitySettings.vSyncCount = 1;

        canManipulateLists = true;

        //nextNodeMoveCostDecreaseTime = Time.time + decreaseNodesMoveCostInterval;
    }

    public void Start()
    {
        if (initialized)
        {
            return;
        }

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

        //initialize threadpool according to amount of citizens
        //this is initialized with a higher value in order to recalculate paths that return as invalid
        //this should fix "waiting" citizens behavior
        pathFindingJobsPool = new PathFindingJob[citizens.Count * Constants.THREADSPERCITIZEN];
        currentPathfindingIndex = -1; //start at -1 due to initial increment

        initialized = true;

        socialCitizens = new List<Citizen>();
        requesters = new List<Citizen>();
        senders = new List<Citizen>();
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

        //if (nextNodeMoveCostDecreaseTime < Time.time)
        //{
        //    DecreaseNodeCosts();
        //}
    }

    public PathFindingJob PathFindingJobsPool
    {
        get
        {
            int nextIndex = currentPathfindingIndex + 1 < pathFindingJobsPool.Length ? currentPathfindingIndex + 1 : 0;

            pathFindingJobsPool[nextIndex] = new PathFindingJob();
            currentPathfindingIndex = nextIndex;
            return pathFindingJobsPool[currentPathfindingIndex];
        }
        set
        {
            PathFindingJobsPool = value;
        }
    }

    private void DecreaseNodeCosts()
    {
        worldMapNodes.DecreaseNodeCosts();
        //nextNodeMoveCostDecreaseTime = Time.time + decreaseNodesMoveCostInterval;
    }

    public void QueueCitizenForSocializing(Citizen target)
    {
        if (!socialCitizens.Contains(target))
        {
            socialCitizens.Add(target);
        }
    }

    public void RemoveCitizenFromSocialQueue(Citizen target)
    {
        try
        {
            if (canManipulateLists)
            {
                canManipulateLists = false;
                socialCitizens.Remove(target);
                requesters.Remove(target);
                senders.Remove(target);
            }
            canManipulateLists = true;
        }
        catch
        {
            canManipulateLists = true;
        }
    }

    public bool CitizenAvailableForSocializingExists(Citizen requester)
    {
        var tempList = socialCitizens;
        var available = tempList.Count;
        for (int i = 0; i < socialCitizens.Count; i++)
        {
            if (socialCitizens[i] == requester)
            {
                available--;
                continue;
            }

            if (requesters.Contains(socialCitizens[i]))
            {
                available--;
            }
            if (senders.Contains(socialCitizens[i]))
            {
                available--;
            }
        }

        return available > 0;
    }

    public Citizen GetNextAvailableCitizenForSocializing(Citizen requester)
    {
        try
        {
            if (canManipulateLists)
            {
                canManipulateLists = false;
                //go through list of requesters and return the associated sender
                for (int request = 0; request < requesters.Count; request++)
                {
                    if (requesters[request] == requester)
                    {
                        senders[request].SetSocialResponseSuccess();
                        canManipulateLists = true;
                        return senders[request];
                    }
                }

                foreach (var cit in socialCitizens)
                {
                    if (cit == requester)
                    {
                        continue;
                    }

                    if (cit.waitingForResponse)
                    {
                        senders.Add(cit);

                        requester.SetSocialResponseSuccess();
                        requesters.Add(requester);

                        canManipulateLists = true;
                        return cit;
                    }
                }
            }
        }
        catch
        {
            canManipulateLists = true;
        }
        canManipulateLists = true;
        return null;
    }
}
