using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class QuestSolvedWinCondition : GameWinCondition
{
    public List<Quest> Quests;

    void Start()
    {
        foreach(var quest in Quests)
        {
            quest.QuestCompleted += Quest_QuestCompleted;
        }
        
    }

    private void Quest_QuestCompleted(object sender, EventArgs e)
    {
        bool allCompleted = true;

        foreach(var quest in Quests)
        {
            allCompleted &= quest.IsCompleted;
        }

        if(allCompleted)
        {
            GameManager.Instance.SetGameWon();
        }  
    }
}