using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public float uiRefreshRate;
    public Text time;
    private WorldTime worldTime;

    public GameObject citizenInfoPanel;
    public GameObject citizenContentPanel;
    public GameObject citizenPanelPrefab;
    private int citizenPanelUpdateIndex;
    private bool citizenPanelPopulated;

    public GameObject characterSheetPanel;
    public Text characterInfo;
    private GameObject targetCharacter;
    public GameObject targetIndicator;

    public GameObject characterSchedulePanel;
    public Text ageLabel;
    public GameObject characterSchedule;
    public GameObject freeTime, workTime, sleepTime, selectedAction;
    private Citizen.CitizenAction selectedScheduleAction;
    private GameObject[] citizenScheduleSlices;

    public Text frameCountLabel;

    private bool focusOnTarget;
    public Vector3 cameraOffset;
    public float cameraMovementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        targetIndicator.SetActive(false);

        worldTime = Constants.gameManager.worldTime;
        InvokeRepeating("UpdateWorldTime", 0.0f, uiRefreshRate);

        characterSheetPanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSheet", 0.01f, uiRefreshRate);

        characterSchedulePanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSchedule", 0.02f, uiRefreshRate);

        InvokeRepeating("UpdateFrameCount", 0.03f, 0.1f);

        citizenInfoPanel.SetActive(false);
        citizenPanelUpdateIndex = 0;
        InvokeRepeating("UpdateCitizenPanels", 0.04f, uiRefreshRate);

        focusOnTarget = false;
    }

    private void FixedUpdate()
    {
        if (targetCharacter != null && targetIndicator.activeSelf)
        {
            //target indicator
            var tempVectorReposition = new Vector3();
            if (targetCharacter.GetComponent<Citizen>().GetQueuedAction() == targetCharacter.GetComponent<Citizen>().GetCurrentAction())
            {
                tempVectorReposition = new Vector3(0, 3.0f, 0);
            }

            targetIndicator.transform.position = targetCharacter.transform.position + tempVectorReposition;

            //camera focus
            if (focusOnTarget)
            {
                Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position,
                    targetCharacter.transform.position - new Vector3(0, targetCharacter.transform.position.y, 0) + new Vector3(0, Camera.main.transform.position.y, 0) + cameraOffset,
                    Time.deltaTime * cameraMovementSpeed);
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
        if (minutes == 60)
        {
            minutes = 0;
        }
        return hours.ToString("00") + ":" + minutes.ToString("00");
    }

    public void SelectCharacter(GameObject target)
    {
        targetCharacter = target;

        targetIndicator.SetActive(true);

        //close citizen list on citizen select
        if (citizenInfoPanel.activeSelf)
        {
            citizenInfoPanel.SetActive(false);
        }

        ChangeOrPopulateCharacterSchedule(targetCharacter.GetComponent<Citizen>());
    }

    public void DeselectCharacter()
    {
        targetCharacter = null;
        targetIndicator.SetActive(false);
    }

    //not used anymore
    public void ClearSelectedCharacter()
    {
        if (targetCharacter != null)
        {
            targetCharacter = null;

            ClearCharacterSchedule();
        }
    }

    public void UpdateCitizenPanel()
    {
        if (citizenContentPanel.transform.childCount != Constants.gameManager.citizens.Count)
        {
            citizenPanelPopulated = false;
            foreach (Transform child in citizenContentPanel.transform)
            {
                Destroy(child);
            }

            for (int i = 0; i < Constants.gameManager.citizens.Count; i++)
            {
                var citizenPanel = Instantiate(citizenPanelPrefab, citizenContentPanel.transform);
                citizenPanel.GetComponent<CitizenPanel>().SetTargetCitizen(Constants.gameManager.citizens[i]);
            }
        }
        citizenPanelPopulated = true;
    }

    private void UpdateCitizenPanels()
    {
        if (/*!citizenContentPanel.activeSelf ||*/ !citizenPanelPopulated)
        {
            return;
        }
        citizenContentPanel.transform.GetChild(citizenPanelUpdateIndex).GetComponent<CitizenPanel>().UpdateUI();
        citizenPanelUpdateIndex = citizenPanelUpdateIndex + 1 >= citizenContentPanel.transform.childCount ? 0 : citizenPanelUpdateIndex + 1;
    }

    private void UpdateCharacterSheet()
    {
        if (targetCharacter == null || !targetCharacter.GetComponent<Citizen>())
        {
            if (characterSheetPanel.activeSelf)
            {
                characterSheetPanel.SetActive(false);
            }
            return;
        }

        characterSheetPanel.SetActive(true);

        var citizen = targetCharacter.GetComponent<Citizen>();

        characterInfo.text = string.Format("Food: {0}   Water: {1}   Entertainment: {2}   Sleep: {3}\nHome: {4}   Action: {5}   Queued action: {6}",
            citizen.needs.food.ToString(".00"),
            citizen.needs.hydration.ToString(".00"),
            citizen.needs.entertainment.ToString(".00"),
            citizen.needs.sleep.ToString(".00"),
            Constants.gameManager.buildings.getFacilityForCitizen(citizen.gameObject, Buildings.FacilityTypes.Home) != null ?
            Constants.gameManager.buildings.getFacilityForCitizen(citizen.gameObject, Buildings.FacilityTypes.Home).parent.name : "None",
            citizen.GetCurrentAction().ToString(),
            citizen.GetQueuedAction().ToString());
    }

    private void UpdateFrameCount()
    {
        frameCountLabel.text = ((int)(1f / Time.unscaledDeltaTime)).ToString() + " Fps";
    }

    public void ShowCharacterSchedule()
    {
        if (targetCharacter != null)
        {
            characterSchedulePanel.SetActive(true);
        }
    }
    public void SpeedUp(float time)
    {
        Time.timeScale = time;
    }

    //not used anymore
    private void ClearCharacterSchedule()
    {
        foreach (Transform child in characterSchedule.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ChangeOrPopulateCharacterSchedule(Citizen citizen)
    {
        bool populate = false;
        if (characterSchedule.transform.childCount <= 0)
        {
            citizenScheduleSlices = new GameObject[citizen.schedule.allocatedAction.Length];
            populate = true;
        }

        for (int timeSlot = 0; timeSlot < citizen.schedule.allocatedAction.Length; timeSlot++)
        {
            switch (citizen.schedule.allocatedAction[timeSlot])
            {
                case (Citizen.CitizenAction.Sleep):
                    {
                        if (populate)
                        {
                            citizenScheduleSlices[timeSlot] = GameObject.Instantiate(sleepTime, characterSchedule.transform);
                        }
                        else
                        {
                            citizenScheduleSlices[timeSlot].GetComponent<RawImage>().color = sleepTime.GetComponent<RawImage>().color;
                        }
                        break;
                    }
                case (Citizen.CitizenAction.Work):
                    {
                        if (populate)
                        {
                            citizenScheduleSlices[timeSlot] = GameObject.Instantiate(workTime, characterSchedule.transform);
                        }
                        else
                        {
                            citizenScheduleSlices[timeSlot].GetComponent<RawImage>().color = workTime.GetComponent<RawImage>().color;
                        }
                        break;
                    }
                case (Citizen.CitizenAction.Play):
                    {
                        if (populate)
                        {
                            citizenScheduleSlices[timeSlot] = GameObject.Instantiate(freeTime, characterSchedule.transform);
                        }
                        else
                        {
                            citizenScheduleSlices[timeSlot].GetComponent<RawImage>().color = freeTime.GetComponent<RawImage>().color;
                        }
                        break;
                    }
            }

            if (populate)
            {
                citizenScheduleSlices[timeSlot].GetComponent<ScheduleSlot>().SetTimeSliceUnit(timeSlot * Constants.TIMELSLICEUNIT);
            }
        }

        if (!populate)
        {
            int slot = 0;
            foreach (Transform trans in characterSchedule.transform)
            {
                if (trans.gameObject != citizenScheduleSlices[slot])
                {
                    trans.gameObject.GetComponent<RawImage>().color = citizenScheduleSlices[slot].GetComponent<RawImage>().color;
                }
                slot++;
            }
        }
    }

    private void UpdateCharacterSchedule()
    {
        if (!characterSchedulePanel.activeSelf)
        {
            return;
        }

        if (targetCharacter == null || !targetCharacter.GetComponent<Citizen>())
        {
            if (characterSchedulePanel.activeSelf)
            {
                characterSchedulePanel.SetActive(false);
            }

            return;
        }

        ageLabel.text = string.Format("{0} Days", targetCharacter.GetComponent<Citizen>().GetAge());
    }

    public void SelectActionForSchedule(int actionSelected)
    {
        switch (actionSelected)
        {
            case 0:
                {
                    selectedScheduleAction = Citizen.CitizenAction.Sleep;
                    selectedAction.GetComponent<RawImage>().color = sleepTime.GetComponent<RawImage>().color;
                    break;
                }
            case 1:
                {
                    selectedScheduleAction = Citizen.CitizenAction.Work;
                    selectedAction.GetComponent<RawImage>().color = workTime.GetComponent<RawImage>().color;
                    break;
                }
            case 2:
                {
                    selectedScheduleAction = Citizen.CitizenAction.Play;
                    selectedAction.GetComponent<RawImage>().color = freeTime.GetComponent<RawImage>().color;
                    break;
                }
        }
    }

    public void ReplaceActionInScheduleForSelectedCharacter()
    {
        int index = -1;
        for (int trans = 0; trans < characterSchedule.transform.childCount; trans++)
        {
            if (characterSchedule.transform.GetChild(trans) == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform)
            {
                index = trans;
                break;
            }
        }

        if (index == -1)
        {
            return;
        }

        switch (selectedScheduleAction)
        {
            case Citizen.CitizenAction.Sleep:
                {
                    targetCharacter.GetComponent<Citizen>().schedule.ReplaceInSchedule(Citizen.CitizenAction.Sleep, index);
                    break;
                }
            case Citizen.CitizenAction.Work:
                {
                    targetCharacter.GetComponent<Citizen>().schedule.ReplaceInSchedule(Citizen.CitizenAction.Work, index);
                    break;
                }
            case Citizen.CitizenAction.Play:
                {
                    targetCharacter.GetComponent<Citizen>().schedule.ReplaceInSchedule(Citizen.CitizenAction.Play, index);
                    break;
                }
        }

        ChangeOrPopulateCharacterSchedule(targetCharacter.GetComponent<Citizen>());
    }

    public void FocusOnCitizen()
    {
        if (targetCharacter == null)
        {
            focusOnTarget = false;
            return;
        }

        focusOnTarget = !focusOnTarget;
    }

    void OnValidate()
    {
        uiRefreshRate = Mathf.Clamp(uiRefreshRate, 0.05f, 1f);
        cameraMovementSpeed = Mathf.Clamp(cameraMovementSpeed, 0.5f, 50.0f);
    }

}
