using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    //public Quest Quest;

    public Text TextQuest;

    public Text TextTasks;

    public QuestSelectHandler QuestSelector;

    public GameObject TasksUI;

    public String No_Quest_Text = "- No Quest accepted -";

    private PlayerController mPlayer;

    public void Start()
    {
        mPlayer = GameManager.Instance.Player;

        AddQuest(null);

    }

    public void QuestSelected(int i)
    {
        var quests = mPlayer.GetQuests();
        var selectedQuest = quests[i];
        AddQuest(selectedQuest);
    }

    public void QuestSolved(Quest quest)
    {
        TextQuest.text = quest.Name + " solved!";
        TasksUI.SetActive(false);
        QuestSelector.gameObject.SetActive(false);
        Invoke("ClearQuest", 3);
    }

    private void ClearQuest()
    {
        var quests = mPlayer.GetQuests();
        var quest = quests.Count > 0 ? quests[0] : null;

        AddQuest(quest);
    }

    public void AddQuest(Quest quest)
    {
        int height = 70;

        if(quest!= null)
        {
            TextQuest.text = quest.Name;

            TasksUI.SetActive(true);

            TextTasks.text = "";
            
            foreach (QuestTask task in quest.Tasks)
            {
                TextTasks.text += task.Name + Environment.NewLine;
                height += 20;
            }

            var rect_task_UI = TasksUI.GetComponent<RectTransform>();
            rect_task_UI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        else
        {
            TasksUI.SetActive(false);
            TextTasks.text = "";
            TextQuest.text = No_Quest_Text;
        }

        QuestSelector.UpdateQuestDropDown();
        QuestSelector.SelectQuest(quest);
    }
}
