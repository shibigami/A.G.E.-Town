using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class QuestListener : MonoBehaviour
{
    public Quest quest;

    public UnityEvent OnQuestCompleted;

    public UnityEvent OnQuestCompletedDelayed;

    public float Delay = 0;

    void Start()
    {
        if(quest != null)
        {
            quest.QuestCompleted += Quest_QuestCompleted;
        }
    }

    private void Quest_QuestCompleted(object sender, System.EventArgs e)
    {
        if (OnQuestCompleted != null)
        {
            OnQuestCompleted.Invoke();
        }

        if(Delay != 0)
        {
            StartCoroutine(InvokeDelayedQuestCompleted());
        }
    }

    IEnumerator InvokeDelayedQuestCompleted()
    {
        yield return new WaitForSeconds(Delay);

        if (OnQuestCompletedDelayed != null)
            OnQuestCompletedDelayed.Invoke();
    }
}
