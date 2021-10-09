using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestItem
{
    public InteractableItemBase Item;

    public int Goal = 0;
}

[Serializable]
public class CollectQuestTask : QuestTask
{
    public List<QuestItem> ItemsToCollect;

    public override bool CheckCompleted(PlayerController player)
    {
        // Loop over items to collect and 
        // set IsCompleted to true if all items are collected
        foreach (QuestItem questItem in ItemsToCollect)
        {
            int count = player.Inventory.GetItemCount(questItem.Item);
            if(count >= questItem.Goal)
            {
                IsCompleted = true;
                return true;
            }
            break;
        }
        return false;

    }
}
