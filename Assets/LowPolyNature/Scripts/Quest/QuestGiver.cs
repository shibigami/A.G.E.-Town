using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class QuestGiver : MonoBehaviour
{
    public Quest Quest;

    public PlayerController Player;

    public QuestAcceptUI AcceptUI;

    public void OpenAcceptUI()
    {
        if (AcceptUI != null)
        {
            AcceptUI.Open(this);
        }
    }

    public UnityEvent QuestAccepted;

    public void Accept()
    {
        if (Player != null && Quest != null)
        {
            Player.AcceptQuest(Quest);

            if (QuestAccepted != null)
            {
                QuestAccepted.Invoke();
            }
        }
    }
}
