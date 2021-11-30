using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitizenPanel : MonoBehaviour
{
    private GameObject targetCitizen;
    private Citizen citizenClass;
    public float refreshRate;
    public Text age, action, queuedAction;
    public Slider drink, food, sleep, fun;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("UpdateUI", 0.01f, refreshRate);
    }

    public void UpdateUI()
    {
        if (targetCitizen == null)
        {
            return;
        }

        if (age.text != "Age: " + citizenClass.GetAge().ToString())
        {
            age.text = "Age: " + citizenClass.GetAge().ToString();
        }
        if (action.text != citizenClass.GetCurrentAction().ToString())
        {
            action.text = citizenClass.GetCurrentAction() != Citizen.CitizenAction.GoInside &&
                citizenClass.GetCurrentAction() != Citizen.CitizenAction.GoOutside &&
                citizenClass.GetCurrentAction() != Citizen.CitizenAction.None ?
                citizenClass.GetCurrentAction().ToString() + "ing" : citizenClass.GetCurrentAction().ToString();
        }
        if (queuedAction.text != "Going to " + citizenClass.GetQueuedAction().ToString().ToLower())
        {
            queuedAction.text = "Going to " + citizenClass.GetQueuedAction().ToString().ToLower();
        }

        if (drink.value != citizenClass.needs.hydration)
        {
            drink.value = citizenClass.needs.hydration;
        }
        if (food.value != citizenClass.needs.food)
        {
            food.value = citizenClass.needs.food;
        }
        if (sleep.value != citizenClass.needs.sleep)
        {
            sleep.value = citizenClass.needs.sleep;
        }
        if (fun.value != citizenClass.needs.entertainment)
        {
            fun.value = citizenClass.needs.entertainment;
        }
    }

    public void SetTargetCitizen(GameObject citizen)
    {
        targetCitizen = citizen;
        citizenClass = targetCitizen.GetComponent<Citizen>();
    }

    public void SelectTargetCitizen()
    {
        if (targetCitizen == null)
        {
            return;
        }

        Constants.ui.SelectCharacter(targetCitizen);
    }

    private void OnValidate()
    {
        refreshRate = Mathf.Clamp(refreshRate, 0.1f, 1.0f);
    }
}
