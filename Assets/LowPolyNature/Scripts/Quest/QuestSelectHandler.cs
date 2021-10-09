using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSelectHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var dropDown = transform.GetComponent<Dropdown>();

        dropDown.onValueChanged.AddListener(delegate
        {
            QuestSelected(dropDown);
        });
    }

    public void UpdateQuestDropDown()
    {
        var player = GameManager.Instance.Player;

        var dropDown = transform.GetComponent<Dropdown>();

        dropDown.options.Clear();

        var quests = player.GetQuests();

        foreach (var quest in quests)
        {
            dropDown.options.Add(new Dropdown.OptionData() { text = quest.Name });
        }

        dropDown.gameObject.SetActive(quests.Count > 0);

    }

    public void SelectQuest(Quest quest)
    {
        if (quest != null)
        {
            var dropDown = transform.GetComponent<Dropdown>();
            int index = dropDown.options.FindIndex((i) => { return i.text.Equals(quest.Name); });
            dropDown.value = index;
            dropDown.RefreshShownValue();
        }
    }

    // Update is called once per frame
    void QuestSelected(Dropdown questSelector)
    {
        int selectedIndex = questSelector.value;

        var questUI = questSelector.transform.parent.GetComponent<QuestUI>();
        questUI.QuestSelected(selectedIndex);
    
    }
}
