using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings
{
    public enum FacilityTypes
    {
        Home,
        Work,
        Eat,
        Entertainment
    }

    public GameObject housesParent { get; private set; }
    public Dictionary<GameObject, GameObject[]> houses { get; private set; }
    public GameObject workParent { get; private set; }
    public Dictionary<GameObject, GameObject[]> eatFacilities { get; private set; }
    public GameObject eatingParent { get; private set; }
    public Dictionary<GameObject, GameObject[]> eatingFacilities { get; private set; }
    public GameObject entertainmentParent { get; private set; }
    public Dictionary<GameObject, GameObject[]> entertainmentFacilities { get; private set; }

    // Start is called before the first frame update
    public Buildings()
    {
        housesParent = GameObject.FindGameObjectWithTag(Constants.Tags.Houses.ToString());
        if (housesParent != null)
        {
            houses = GetFacilityDictionary(housesParent, Constants.CITIZENSPERHOME);
        }

        workParent = GameObject.FindGameObjectWithTag(Constants.Tags.WorkFacilities.ToString());
        if (workParent != null)
        {
            eatFacilities = GetFacilityDictionary(workParent, Constants.CITIZENSPERWORK);
        }

        eatingParent = GameObject.FindGameObjectWithTag(Constants.Tags.EatingFacilities.ToString());
        if (eatingParent != null)
        {
            eatingFacilities = GetFacilityDictionary(eatingParent, Constants.CITIZENSPERFOOD);
        }

        entertainmentParent = GameObject.FindGameObjectWithTag(Constants.Tags.EntertainmentFacilities.ToString());
        if (entertainmentParent != null)
        {
            entertainmentFacilities = GetFacilityDictionary(entertainmentParent, Constants.CITIZENSPERENTERTAINMENT);
        }
    }

    public Transform getFacilityForCitizen(GameObject citizen, FacilityTypes facilityType)
    {
        var facilityList = GetFacilities(facilityType);
        if (facilityList == null || facilityList.Keys.Count <= 0)
        {
            return null;
        }

        foreach (GameObject facility in facilityList.Keys)
        {
            for (int i = 0; i < facilityList[facility].Length; i++)
            {
                if (facilityList[facility][i] == citizen)
                {
                    //return facilities door
                    for (int c = 0; c < facility.transform.childCount; c++)
                    {
                        if (facility.transform.GetChild(c).tag == "Door")
                        {
                            return facility.transform.GetChild(c);
                        }
                    }
                }
            }
        }
        return null;
    }

    public bool AssignFacility(GameObject citizenGameObject, FacilityTypes facilityType)
    {
        Dictionary<GameObject, GameObject[]> buildingCollection = GetFacilities(facilityType);

        if (buildingCollection == null || buildingCollection.Count <= 0)
        {
            return false;
        }

        bool changeMade = false;
        foreach (GameObject facility in buildingCollection.Keys)
        {
            for (int i = 0; i < buildingCollection[facility].Length; i++)
            {
                if (buildingCollection[facility].GetValue(i) == null)
                {
                    buildingCollection[facility].SetValue(citizenGameObject, i);
                    if (!changeMade)
                    {
                        changeMade = true;
                    }
                    return true;
                }
            }
        }

        if (changeMade)
        {
            var success = SetFacilities(buildingCollection, facilityType);
            return success;
        }

        //got here, facilities might be full
        return false;
    }

    private Dictionary<GameObject, GameObject[]> GetFacilityDictionary(GameObject facilitiesParent, int facilityCapacity)
    {
        Dictionary<GameObject, GameObject[]> temp = new Dictionary<GameObject, GameObject[]>();
        for (int i = 0; i < facilitiesParent.transform.childCount; i++)
        {
            temp.Add(facilitiesParent.transform.GetChild(i).gameObject, new GameObject[facilityCapacity]);
        }

        return temp.Keys.Count > 0 ? temp : null;
    }

    private Dictionary<GameObject, GameObject[]> GetFacilities(FacilityTypes facilityType)
    {
        switch (facilityType)
        {
            case FacilityTypes.Home:
                {
                    return houses;
                }
            case FacilityTypes.Work:
                {
                    return eatFacilities;
                }
            case FacilityTypes.Eat:
                {
                    return eatingFacilities;
                }
            case FacilityTypes.Entertainment:
                {
                    return entertainmentFacilities;
                }
        }

        return null;
    }


    private bool SetFacilities(Dictionary<GameObject, GameObject[]> facilityDictionary, FacilityTypes facilityType)
    {
        bool success = false;
        switch (facilityType)
        {
            case FacilityTypes.Work:
                {
                    eatFacilities = facilityDictionary;
                    success = true;
                    break;
                }
            case FacilityTypes.Eat:
                {
                    eatingFacilities = facilityDictionary;
                    success = true;
                    break;
                }
            case FacilityTypes.Entertainment:
                {
                    entertainmentFacilities = facilityDictionary;
                    success = true;
                    break;
                }
        }

        return success;
    }
}
