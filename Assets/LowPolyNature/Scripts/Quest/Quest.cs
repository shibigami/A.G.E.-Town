using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QuestReward
{
    public int Count = 0;

    // item for the inventory...
    public InventoryItemBase Item;
}

[Serializable]
public class QuestTask : MonoBehaviour
{
    private bool mIsCompleted = false;

    public String Name;

    public UnityEvent TaskCompleted;

    protected Quest mQuest;

    public virtual void Initialize(Quest quest)
    {
        mQuest = quest;
    }

    public virtual bool CheckCompleted(PlayerController player)
    {
        return false;
    }

    public bool IsCompleted
    {
        get { return mIsCompleted; }
        set
        {
            if (mIsCompleted != value)
            {
                mIsCompleted = value;
                if (mIsCompleted)
                {
                    if (TaskCompleted != null)
                    {
                        TaskCompleted.Invoke();
                    }
                }
            }
        }
    }
}

[Serializable]
public class Quest : MonoBehaviour
{
    private bool mIsCompleted = false;

    public string Name;

    public string Description;

    public List<QuestReward> Rewards;

    public List<QuestTask> Tasks;

    public event EventHandler QuestCompleted;

    private PlayerController mPlayer;

    public void Initialize(PlayerController player)
    {
        mPlayer = player;

        foreach (var task in Tasks)
        {
            task.Initialize(this);   
        }
    }

    public void CheckCompleted()
    {
        bool completed = true;
        foreach (var task in Tasks)
        {
            completed &= task.CheckCompleted(mPlayer);
        }

        IsCompleted = completed;

        if (completed)
        {
            mPlayer.SetQuestCompleted(this);

            if (QuestCompleted != null)
                QuestCompleted(this, EventArgs.Empty);
        }
    }


    public bool IsCompleted
    {
        get { return mIsCompleted; }
        set
        {
            mIsCompleted = value;
        }
    }
}
