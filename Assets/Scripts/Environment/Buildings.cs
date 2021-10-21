using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings
{
    public GameObject housesParent;
    public Dictionary<GameObject, GameObject[]> houses;

    // Start is called before the first frame update
    public Buildings()
    {
        houses = new Dictionary<GameObject, GameObject[]>();
        housesParent = GameObject.FindGameObjectWithTag(Constants.Tags.Houses.ToString());
        for (int i = 0; i < housesParent.transform.childCount; i++)
        {
            houses.Add(housesParent.transform.GetChild(i).gameObject, new GameObject[2]);
        }
    }

    public GameObject getHomeForCitizen(GameObject citizen)
    {
        foreach (GameObject building in houses.Keys)
        {
            for (int i = 0; i < houses[building].Length; i++)
            {
                if (houses[building][i] == citizen) 
                {
                    return building;
                }
            }
        }
        return null;
    }
}
