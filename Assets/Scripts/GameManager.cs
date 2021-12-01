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

    //pool of awaiting citizens
    public List<Citizen> socialCitizens { get; private set; }
    //pool of replied citizens
    public List<Citizen> senders { get; private set; }
    //pool of citizens that made the request
    public List<Citizen> requesters { get; private set; }

    public bool procriationOn;
    public float oddsOfProcriating;
    private List<bool> procriated;
    private bool procriationForTheDayComplete;
    private bool procriationListReset;

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

        procriated = new List<bool>();
        procriationForTheDayComplete = false;
        procriationListReset = false;

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

        //setup aging scene
        if (procriationOn)
        {
            SetupAging();
            ResetProcriationList();
        }
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

        if (procriationOn)
        {
            if (procriated.Contains(false))
            {
                if (worldTime.GetCurrentWorldTimeInHours() < 12)
                {
                    for (int i = 0; i < procriated.Count; i++)
                    {
                        if (!procriated[i] && RollForProcriation())
                        {
                            Procriate();
                        }

                        procriated[i] = true;
                    }
                }
                procriationForTheDayComplete = true;
                procriationListReset = false;
            }
            else
            {
                if (worldTime.GetCurrentWorldTimeInHours() > 12)
                {
                    //if (!procriationListReset)
                    //{
                        ResetProcriationList();
                        procriationListReset = true;
                    //}
                    procriationForTheDayComplete = false;
                }
            }
        }
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
        var available = socialCitizens.Count;
        for (int i = 0; i < socialCitizens.Count; i++)
        {
            if (socialCitizens[i] == requester)
            {
                available--;
                continue;
            }

            //if (requesters.Contains(socialCitizens[i]))
            //{
            //    available--;
            //}
            //if (senders.Contains(socialCitizens[i]))
            //{
            //    available--;
            //}
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
                        socialCitizens.Remove(senders[request]);
                        socialCitizens.Remove(requester);
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
                        socialCitizens.Remove(cit);
                        socialCitizens.Remove(requester);
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

    private void SetupAging()
    {
        foreach (var cit in citizens)
        {
            cit.gameObject.SetActive(false);
        }
        foreach (var home in buildings.houses.Keys)
        {
            home.gameObject.SetActive(false);
        }
        foreach (var entertainment in buildings.entertainmentFacilities.Keys)
        {
            entertainment.gameObject.SetActive(false);
        }
        foreach (var eating in buildings.eatingFacilities.Keys)
        {
            eating.gameObject.SetActive(false);
        }
        foreach (var work in buildings.eatFacilities.Keys)
        {
            work.gameObject.SetActive(false);
        }

        Procriate();
        Procriate();
    }

    public void Procriate()
    {
        bool home = false;
        bool work = false;
        bool eat = false;
        bool entertainment = false;
        foreach (var cit in citizens)
        {
            //citizen
            if (!cit.gameObject.activeSelf)
            {
                //home
                foreach (var house in buildings.houses.Keys)
                {
                    foreach (var houseCit in buildings.houses[house])
                    {
                        if (houseCit == cit)
                        {
                            house.SetActive(true);
                            home = true;
                            break;
                        }
                    }
                    if (home)
                    {
                        break;
                    }
                }
                //work
                foreach (var workF in buildings.eatFacilities.Keys)
                {
                    foreach (var workCit in buildings.eatFacilities[workF])
                    {
                        if (workCit == cit)
                        {
                            workF.SetActive(true);
                            home = true;
                            break;
                        }
                    }
                    if (work)
                    {
                        break;
                    }
                }
                //eat
                foreach (var eatF in buildings.eatFacilities.Keys)
                {
                    foreach (var eatCit in buildings.eatFacilities[eatF])
                    {
                        if (eatCit == cit)
                        {
                            eatF.SetActive(true);
                            eat = true;
                            break;
                        }
                    }
                    if (eat)
                    {
                        break;
                    }
                }
                //entertainment
                foreach (var enterF in buildings.entertainmentFacilities.Keys)
                {
                    foreach (var enterCit in buildings.entertainmentFacilities[enterF])
                    {
                        if (enterCit == cit)
                        {
                            enterF.SetActive(true);
                            entertainment = true;
                            break;
                        }
                    }
                    if (entertainment)
                    {
                        break;
                    }
                }
                cit.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void ResetProcriationList()
    {
        procriated = new List<bool>();
        var amount = 0;
        foreach (var cit in citizens)
        {
            if (!cit.activeSelf)
            {
                continue;
            }
            if (cit.GetComponent<Citizen>().GetAge() >= cit.GetComponent<Citizen>().ageForMaxGrowth)
            {
                amount++;
            }
        }

        amount = Mathf.FloorToInt(amount);
        for (int i = 0; i < amount; i++)
        {
            procriated.Add(false);
        }
    }

    private bool RollForProcriation()
    {
        return Random.Range(1, 100) <= oddsOfProcriating;
    }
}
