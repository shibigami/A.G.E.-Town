using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public float uiRefreshRate;
    public Text time;
    private WorldTime worldTime;

    public GameObject CharacterSheetPanel;
    public Text characterInfo;
    private GameObject targetCharacter;

    public GameObject CharacterSchedulePanel;

    //editor on change
    public void OnGUI()
    {
        //check later how to set editor variables on change properly
        //OnGui is not the correct method
        uiRefreshRate = Mathf.Clamp(uiRefreshRate, 0.05f, 1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        worldTime = Constants.gameManager.worldTime;
        InvokeRepeating("UpdateWorldTime", 0.0f, uiRefreshRate);

        CharacterSheetPanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSheet", 0.01f, uiRefreshRate);

        CharacterSchedulePanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSchedule", 0.02f, uiRefreshRate);
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

    private void UpdateWorldTime()
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

        characterInfo.text = string.Format("Food: {0}   Water: {1}  Entertainment: {2}    Sleep: {3}\nHome: {4}",
            citizen.needs.food.ToString(".00"),
            citizen.needs.hydration.ToString(".00"),
            citizen.needs.entertainment.ToString(".00"),
            citizen.needs.sleep.ToString(".00"),
            Constants.gameManager.buildings.getFacilityForCitizen(citizen.gameObject, Buildings.FacilityTypes.Home) != null ?
            Constants.gameManager.buildings.getFacilityForCitizen(citizen.gameObject, Buildings.FacilityTypes.Home).name : "None");
    }
}
