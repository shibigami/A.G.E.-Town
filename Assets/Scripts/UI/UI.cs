using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text time;
    private WorldTime worldTime;

    public GameObject CharacterSheetPanel;
    public Text characterInfo;
    private GameObject targetCharacter;

    // Start is called before the first frame update
    void Start()
    {
        worldTime = Constants.gameManager.worldTime;
        InvokeRepeating("UpdateTime", 0.0f, 0.5f);

        CharacterSheetPanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSheet", 0.01f, 0.5f);
    }

    private void Update()
    {
        if (targetCharacter != null)
        {
            var moveToPoints = targetCharacter.GetComponent<Citizen>().moveToPoints;

            if (moveToPoints != null)
            {
                //draw path for debugging purposes
                for (int i = 1; i < moveToPoints.Length; i++)
                {
                    var firstpoint = new Vector3(moveToPoints[i - 1].location.x, 1, moveToPoints[i - 1].location.y);
                    var secondpoint = new Vector3(moveToPoints[i].location.x, 1, moveToPoints[i].location.y);
                    Debug.DrawLine(firstpoint, secondpoint, Color.white);
                }
            }
        }
    }

    private void UpdateTime()
    {
        time.text = ConvertTime();
    }

    private string ConvertTime()
    {
        var hours = Mathf.FloorToInt(worldTime.currentTime / 60);
        var minutes = worldTime.currentTime - hours * 60;
        return hours.ToString("00") + ":" + minutes.ToString("00");
    }

    public void SelectCharacter(GameObject target)
    {
        targetCharacter = target;
    }

    public void ClearSelectedCharacter()
    {
        if (targetCharacter != null)
        {
            targetCharacter = null;
        }
    }

    private void UpdateCharacterSheet()
    {
        if (targetCharacter == null || !targetCharacter.GetComponent<Citizen>())
        {
            if (CharacterSheetPanel.activeInHierarchy)
            {
                CharacterSheetPanel.SetActive(false);
            }
            return;
        }

        CharacterSheetPanel.SetActive(true);

        var citizen = targetCharacter.GetComponent<Citizen>();

        string schedule = "";
        //for (int i = 0; i < citizen.schedule.allocatedAction.Length; i++)
        //{
        //    schedule += (0.25f * i).ToString() + ":" + citizen.schedule.allocatedAction[i].ToString() + " ";
        //}


        characterInfo.text = string.Format("Food: {0}   Water: {1}  Fun: {2}    Sleep: {3}\nAllocated actions: {4}\nHome: {5}",
            citizen.needs.food.ToString("."),
            citizen.needs.hydration.ToString("."),
            citizen.needs.fun.ToString("."),
            citizen.needs.sleep.ToString("."),
            schedule,
            Constants.gameManager.buildings.getHomeForCitizen(citizen.gameObject) != null ? Constants.gameManager.buildings.getHomeForCitizen(citizen.gameObject).name : "None");
    }
}
