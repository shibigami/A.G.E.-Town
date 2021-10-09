using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KillEnemyQuestTask : QuestTask
{
    public List<Enemy> Enemies;

    public override void Initialize(Quest quest)
    {
        base.Initialize(quest);

        foreach (var enemyGO in Enemies)
        {
            var enemy = enemyGO.GetComponent<Enemy>();
            enemy.Died += Enemy_Died;
        }
    }

    private void Enemy_Died(object sender, EventArgs e)
    {
        mQuest.CheckCompleted();
    }

    public override bool CheckCompleted(PlayerController player)
    {
        bool completed = true;
        foreach (var enemyGO in Enemies)
        {
            var enemy = enemyGO.GetComponent<Enemy>();
            completed &= enemy.IsDead;  
        }
        IsCompleted = completed;
        
        return IsCompleted;
    }
}
