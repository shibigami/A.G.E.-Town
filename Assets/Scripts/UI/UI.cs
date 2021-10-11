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
        worldTime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().worldTime;
        InvokeRepeating("UpdateTime", 0.0f, 0.5f);

        CharacterSheetPanel.SetActive(false);
        InvokeRepeating("UpdateCharacterSheet", 0.01f, 0.5f);
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


        characterInfo.text = string.Format("Food: {0}   Water: {1}  Fun: {2}    Sleep: {3}\nAllocated actions: {4}\nDay progress: {5}",
            citizen.needs.food.ToString("00.0"),
            citizen.needs.hydration.ToString("00.0"),
            citizen.needs.fun.ToString("00.0"),
            citizen.needs.sleep.ToString("00.0"),
            schedule,
            citizen.schedule.dayProgress.ToString());
    }
}
